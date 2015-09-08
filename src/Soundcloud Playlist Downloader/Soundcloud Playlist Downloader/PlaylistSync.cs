using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Soundcloud_Playlist_Downloader.JsonPoco;
using Soundcloud_Playlist_Downloader.Properties;
using System.Diagnostics;

namespace Soundcloud_Playlist_Downloader
{
    class PlaylistSync
    {
        public bool IsError { get; private set; }

        public enum DownloadMode { Playlist, Favorites, Artist };

        public IList<Track> SongsToDownload { get; private set; }
        public IList<Track> SongsDownloaded { get; private set; }

        private object SongsDownloadedLock = new object();

        public bool IsActive { get; set; }

        public PlaylistSync()
        {
            SongsToDownload = new List<Track>();
            SongsDownloaded = new List<Track>();
            ResetProgress();
        }

        private void verifyParameters(Dictionary<string, string> parameters)
        {
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                if (string.IsNullOrWhiteSpace(parameter.Value))
                {
                    IsError = true;
                    throw new Exception(string.Format("{0} must be specified", parameter.Key));
                }
            }
        }

        internal void Synchronize(string url, DownloadMode mode, string directory, bool deleteRemovedSongs, string clientId, bool foldersPerArtist)
        {
            verifyParameters(
                new Dictionary<string, string>()
                {
                    {"URL", url},
                    {"Directory", directory},
                    {"Client ID", clientId}
                }
            );
            ResetProgress();

            string apiURL = null;

            switch (mode)
            {
                case DownloadMode.Playlist:
                    // determine whether it is an api url or a normal url. if it is a normal url, get the api url from it
                    // and then call SynchronizeFromPlaylistAPIUrl. Otherwise just call that method directly
                    
                    if (!url.Contains("api.soundcloud.com"))
                    {
                        apiURL = determineAPIUrlForNormalUrl(url, clientId,"playlists");
                    }
                    else 
                    {
                        apiURL = url;
                    }
                    SynchronizeFromPlaylistAPIUrl(apiURL, clientId, directory, deleteRemovedSongs, foldersPerArtist);
                    break;
                case DownloadMode.Favorites:
                    // get the username from the url and then call SynchronizeFromProfile
                    string username = parseUserIdFromProfileUrl(url);
                    SynchronizeFromProfile(username, clientId, directory, deleteRemovedSongs, foldersPerArtist);
                    break;
                case DownloadMode.Artist:
                    
                    if (!url.Contains("api.soundcloud.com"))
                    {
                        apiURL = determineAPIUrlForNormalUrl(url, clientId,"tracks");
                    }
                    else 
                    {
                        apiURL = url;
                    }
                    SynchronizeFromArtistUrl(apiURL, clientId, directory, deleteRemovedSongs, foldersPerArtist);
                    break;
                default:
                    IsError = true;
                    throw new NotImplementedException("Unknown download mode");
            }
        }

        private string determineAPIUrlForNormalUrl(string url, string clientId,string resulttype)
        {

            // parse the username from the url
            string username = parseUserIdFromProfileUrl(url);
            string playlistName = null;
            try
            {
                // parse the playlist name from the url
                string startingPoint = "/sets/";
                int startingIndex = url.IndexOf(startingPoint) + startingPoint.Length;
                int endingIndex = url.Substring(startingIndex).Contains("/") ?
                    url.Substring(startingIndex).IndexOf("/") + startingIndex :
                    url.Length;
                playlistName = url.Substring(startingIndex, endingIndex - startingIndex);
            }
            catch (Exception e)
            {
                IsError = true;
                throw new Exception("Invalid playlist url: " + e.Message);
            }

            // hit the users/username/playlists endpoint and match the playlist on the permalink
            string userUrl = "https://api.soundcloud.com/users/" + username + "/" + resulttype;

            if (resulttype == "tracks")
            {
                return userUrl;
            }

            return "https://api.soundcloud.com/playlists/" +
                retrievePlaylistId(userUrl, playlistName, clientId);
        }

        private string retrievePlaylistId(string userApiUrl, string playlistName, string clientId)
        {

            // grab the xml from the url, parse each playlist out, match the name based on the
            // permalink, and return the id of the matching playlist.
            // a method already exists for downloading xml, so use that and refactor this to not have
            // the client id embedded in the url
            string playlistsJson = RetrieveJson(userApiUrl, clientId);

            var playlistItems = JsonConvert.DeserializeObject<JsonPoco.PlaylistItem[]>(playlistsJson);

            var playListItem = playlistItems.FirstOrDefault(s => s.permalink == playlistName);

            if (playListItem != null)
            {
                return playListItem.id.ToString();
            }
            else
            {
                IsError = true;
                throw new Exception("Unable to find a matching playlist");

            }

        }

        private string parseUserIdFromProfileUrl(string url)
        {
            try
            {
                string startingPoint = "soundcloud.com/";
                int startingIndex = url.IndexOf(startingPoint) + startingPoint.Length;
                int endingIndex = url.Substring(startingIndex).Contains("/") ?
                    url.Substring(startingIndex).IndexOf("/") + startingIndex :
                    url.Length;

                return url.Substring(startingIndex, endingIndex - startingIndex);
            }
            catch (Exception e)
            {
                IsError = true;
                throw new Exception("Invalid profile url: " + e.Message);
            }
        }

        internal IList<Track> EnumerateTracksFromUrl(string url, string clientId, bool isRawTracksUrl)
        {
            // get the json associated with the playlist from the soundcloud api
            int limit = 200;
            int offset = 0;
            IList<Track> tracks = new List<Track>();

            try
            {
                // get the tracks embedded in the playlist
                bool tracksAdded = true;

                while (tracksAdded)
                {
                    string tracksJson = RetrieveJson(url, clientId, limit, offset);
                    
                    IList<Track> currentTracks = isRawTracksUrl ? JsonConvert.DeserializeObject<Track[]>(tracksJson) : 
                        JsonConvert.DeserializeObject<PlaylistItem>(tracksJson).tracks;

                    if (currentTracks != null && currentTracks.Any())
                    {
                        foreach (Track track in currentTracks)
                        {
                            tracks.Add(track);
                        }
                        tracksAdded = true;
                    }
                    else
                    {
                        tracksAdded = false;
                    }

                    offset += limit;
                }
                
            }
            catch (Exception)
            {
                IsError = true;
                throw new Exception("Errors occurred retrieving the tracks list information. Double check your url.");
            }

            return tracks;
        }

        internal void SynchronizeFromProfile(string username, string clientId, string directoryPath, bool deleteRemovedSongs, bool foldersPerArtist)
        {
            // hit the /username/favorites endpoint for the username in the url, then download all the tracks
            IList<Track> tracks = EnumerateTracksFromUrl("https://api.soundcloud.com/users/" + username + "/favorites", clientId, true);
            Synchronize(tracks, clientId, directoryPath, deleteRemovedSongs, foldersPerArtist);
        }

        public static bool IsPathWithinLimits(string fullPathAndFilename)
        {
            //In the Windows API the maximum length for a path is MAX_PATH, which is defined as 260 characters.
            //We'll make it 250 because there will be an extention and, in some cases, an HQ tag appended to the filename.  
            const int MAX_PATH_LENGTH = 250;
            return fullPathAndFilename.Length <= MAX_PATH_LENGTH;
        }

        private void Synchronize(IList<Track> tracks, string clientId, string directoryPath, bool deleteRemovedSongs, bool foldersPerArtist)
        {
            //define all local paths by combining the sanitzed artist (if checked by user) with the santized title
            foreach(Track track in tracks)
            {
                string validArtist = track.CoerceValidFileName(track.Artist);
                string validTitle = track.CoerceValidFileName(track.Title);
                string filenameWithArtist = validArtist + " - " + validTitle;

                if (foldersPerArtist)
                {
                    if (Form1.IncludeArtistInFilename) //include artist name
                    {
                        while (!IsPathWithinLimits(track.LocalPath = Path.Combine(directoryPath, validArtist,
                            filenameWithArtist)))
                        {
                            filenameWithArtist = filenameWithArtist.Remove(filenameWithArtist.Length - 2); //shorten to fit into max size of path
                        };                
                    }
                    else
                    {
                        while (!IsPathWithinLimits(track.LocalPath = Path.Combine(directoryPath, validArtist,
                            validTitle)))
                        {
                            validTitle = validTitle.Remove(validTitle.Length - 2); //shorten to fit into max size of path
                        };
                    }
                }
                else
                {
                    if (Form1.IncludeArtistInFilename) //include artist name
                    {
                        while (!IsPathWithinLimits(track.LocalPath = Path.Combine(directoryPath, filenameWithArtist)))
                        {
                            filenameWithArtist = filenameWithArtist.Remove(filenameWithArtist.Length - 2); //shorten to fit into max size of path
                        };
                    }
                    else
                    {
                        while (!IsPathWithinLimits(track.LocalPath = Path.Combine(directoryPath, validTitle)))
                        {
                            validTitle = validTitle.Remove(validTitle.Length - 2); //shorten to fit into max size of path
                        };
                    }
                }
                if (track.IsHD)
                {
                    track.LocalPath += " (HQ)";
                }
            };

            //if (foldersPerArtist) //create folder structure
            //{
            //    if (Form1.IncludeArtistInFilename) //include artist name
            //    {
            //        tracks = tracks.Select(c => { c.LocalPath = Path.Combine(directoryPath, c.CoerceValidFileName(c.Artist), 
            //            c.CoerceValidFileName(c.Artist) + " - " + c.CoerceValidFileName(c.Title)); return c; }).ToList();
            //    }
            //    else //exclude artist name
            //    {
            //        tracks = tracks.Select(c => { c.LocalPath = Path.Combine(directoryPath, c.CoerceValidFileName(c.Artist), 
            //            c.CoerceValidFileName(c.Title)); return c; }).ToList();
            //    }
            //}
            //else //don't create folder structure
            //{
            //    if (Form1.IncludeArtistInFilename) //include artist name
            //    {
            //        tracks = tracks.Select(c => { c.LocalPath = Path.Combine(directoryPath, 
            //            c.CoerceValidFileName(c.Artist) + " - " + c.CoerceValidFileName(c.Title)); return c; }).ToList();
            //    }
            //    else //exclude artist name
            //    {
            //        tracks = tracks.Select(c => { c.LocalPath = Path.Combine(directoryPath, 
            //            c.CoerceValidFileName(c.Title)); return c; }).ToList();
            //    }
            

            // determine which tracks should be deleted or re-added
            DeleteOrAddRemovedTrack(directoryPath, tracks, deleteRemovedSongs);

            // determine which tracks should be downloaded
            SongsToDownload = DetermineTracksToDownload(directoryPath, tracks, foldersPerArtist);

            

            // download the relevant tracks
            IList<Track> songsDownloaded = DownloadSongs(SongsToDownload, clientId);

            // update the manifest
            UpdateSyncManifest(songsDownloaded, directoryPath);

            // validation
            if (songsDownloaded.Count != SongsToDownload.Count && IsActive)
            {
                IsError = true;
                throw new Exception(
                        "Some tracks failed to download. You might need to try a few more times before they can download correctly. " +
                        "The following tracks were not downloaded:" + Environment.NewLine +
                        string.Join(Environment.NewLine, SongsToDownload.Except(SongsDownloaded).Select(x => "Title: " + x.Title + ", Artist: " + x.Artist))
                    );
            }
        }


        internal void SynchronizeFromPlaylistAPIUrl(string playlistApiUrl, string clientId, string directoryPath, bool deleteRemovedSongs, bool foldersPerArtist)
        {
            IList<Track> tracks = EnumerateTracksFromUrl(playlistApiUrl, clientId, false);
            Synchronize(tracks, clientId, directoryPath, deleteRemovedSongs, foldersPerArtist);
        }


        internal void SynchronizeFromArtistUrl(string artistUrl, string clientId, string directoryPath, bool deleteRemovedSongs, bool foldersPerArtist)
        {

            IList<Track> tracks = EnumerateTracksFromUrl(artistUrl, clientId, true);
            Synchronize(tracks, clientId, directoryPath, deleteRemovedSongs, foldersPerArtist);
        }


        private void ResetProgress()
        {
            SongsDownloaded.Clear();
            SongsToDownload.Clear();
            IsActive = true;
            IsError = false;
        }

        private void UpdateSyncManifest(IList<Track> tracksDownloaded, string directoryPath)
        {
            IList<string> content = new List<string>();

            foreach (Track track in tracksDownloaded)
            {
                content.Add(track.EffectiveDownloadUrl + "," + track.LocalPath);
            }

            try
            {
                string manifestPath = DetermineManifestPath(directoryPath);
                if (File.Exists(manifestPath))
                {
                    File.AppendAllLines(manifestPath, content);
                }
                else
                {
                    File.WriteAllLines(manifestPath, content);
                }
            }
            catch (Exception)
            {
                IsError = true;
                throw new Exception("Unable to update manifest");
            }

        }

        private IList<Track> DownloadSongs(IList<Track> TracksToDownload, string apiKey)
        {
            IList<Track> songsDownloaded = new List<Track>();
            object trackLock = new object();

            Parallel.ForEach(TracksToDownload, 
                new ParallelOptions() {MaxDegreeOfParallelism = Settings.Default.ConcurrentDownloads},
                track =>
            {
                try
                {
                    if (DownloadTrack(track, apiKey))
                    {

                        lock (trackLock)
                        {
                            songsDownloaded.Add(track);
                        }
                    }

                }
                catch (WebException e)
                {
                    //catching the webexception
                    // Song failed to download
                    using (WebResponse response = e.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        Debug.WriteLine("Error code: {0}", httpResponse.StatusCode);
                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            string text = reader.ReadToEnd();
                            Debug.WriteLine(text);
                        }
                    }
                }
                
            });
            return songsDownloaded;
        }

        private bool DownloadTrack(Track song, string apiKey)
        {
            bool downloaded = false;
            if (IsActive)
            {
                using (WebClient client = new WebClient())
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(song.LocalPath));
                    string extension = null;

                    if (song.IsHD)
                    {
                        try
                        {
                            WebRequest request = WebRequest.Create(song.EffectiveDownloadUrl + string.Format("?client_id={0}",apiKey));

                            request.Method = "HEAD";
                            using (WebResponse response = request.GetResponse())
                            {
                                extension = Path.GetExtension(response.Headers["Content-Disposition"]
                                    .Replace("attachment;filename=", "").Replace("\"", ""));
                            }
                        }
                        catch (Exception e)
                        {
                            if (e is WebException)
                            {
                                WebException w = (WebException)e;
                                //catching the webexception
                                using (WebResponse response = w.Response)
                                {
                                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                                    Debug.WriteLine("Error code: {0}", httpResponse.StatusCode);
                                    using (Stream data = response.GetResponseStream())
                                    using (var reader = new StreamReader(data))
                                    {
                                        string text = reader.ReadToEnd();
                                        Debug.WriteLine(text);
                                    }
                                }
                            }

                            //the download link might have been invalid, so we get the stream download instead
                            if (song.stream_url == null) //all hope is lost when there is no stream url, return to safety
                                return false;
                             
                                WebRequest request = WebRequest.Create(song.stream_url + string.Format("?client_id={0}", apiKey));

                                request.Method = "HEAD";
                                using (WebResponse response = request.GetResponse())
                                {
                                    extension = Path.GetExtension(response.Headers["Content-Disposition"]
                                        .Replace("attachment;filename=", "").Replace("\"", ""));
                                }                            
                        }

                        if (Form1.ConvertToMp3 && Form1.Highqualitysong && (extension == ".wav" || extension == ".aiff" || extension == ".aif"))
                        {
                            //already set filename extention to mp3, because conversion wil result in an mp3
                            song.LocalPath += ".mp3";
                            //get the wav song as byte data, as we won't store it just yet
                            byte[] soundbytes = client.DownloadData(song.EffectiveDownloadUrl + string.Format("?client_id={0}", apiKey));
                            //convert to mp3 & then write bytes to file
        
                            byte[] convertedbytes = audioConverter.ConvertAllTheThings(soundbytes, extension);
                            if (convertedbytes != null)
                            {
                                File.WriteAllBytes(song.LocalPath, convertedbytes);
                            }
                            else //file was not supported by converter, or something else has gone wrong, just write the bytes it can't convert 
                            {
                                File.WriteAllBytes(song.LocalPath, soundbytes);
                            }
                        }
                        else
                        {
                            song.LocalPath += extension;
                            client.DownloadFile(song.EffectiveDownloadUrl + string.Format("?client_id={0}", apiKey), song.LocalPath);
                        };
                    }
                    else
                    {
                        song.LocalPath += ".mp3";
                        client.DownloadFile(song.EffectiveDownloadUrl + string.Format("?client_id={0}", apiKey), song.LocalPath);
                    }                 
                                        
                    //Sets file creation time to creation time that matches with Soundcloud track.
                    //If somehow the datetime string can't be parsed it will just use the current (now) datetime. 
                    DateTime dt = DateTime.Now;
                    DateTime.TryParse(song.created_at, out dt);
                    File.SetCreationTime(song.LocalPath, dt);
                    
                    // metadata tagging
                    TagLib.File tagFile = null;                  

                    TagLib.Id3v2.Tag.DefaultVersion = 2;
                    TagLib.Id3v2.Tag.ForceDefaultVersion = true;
                    // Possible values for DefaultVersion are 2(id3v2.2), 3(id3v2.3) or 4(id3v2.4)
                    // it seems that id3v2.4 is more prone to misinterpret utf-8. id3v2.2 seems most stable. 
                    tagFile = TagLib.File.Create(song.LocalPath);

                    tagFile.Tag.Title = song.Title;
                    string artworkFilepath = null;
                    List<string> listGenreAndTags = new List<string>();

                    if (!String.IsNullOrEmpty(song.Username))
                    {
                        tagFile.Tag.AlbumArtists = new string[] { song.Username };
                        tagFile.Tag.Performers = new string[] { song.Username };
                    }

                    if (!String.IsNullOrEmpty(song.genre))
                    {
                        listGenreAndTags.Add(song.genre);
                        tagFile.Tag.Genres = listGenreAndTags.ToArray();
                    }
                    if (!String.IsNullOrEmpty(song.tag_list))
                    {
                        //NOTE      Tags behave very similar as genres in SoundCloud, 
                        //          so tags will be added to the genre part of the metadata
                        //WARNING   Tags are seperated by \" when a single tag includes a whitespace! (for instance: New Wave)
                        //          Single worded tags are seperated by a single whitespace, this has led me to make
                        //          this code longer than I initially thought it would be (could perhaps made easier)
                        //FEATURES  Rare occasions, where the artist uses tags that include the seperation tags SoundCloud uses;
                        //          like \" or \"Hip-Hop\", are handled, but NOT necessary, because the quote (") is an illegal sign to use in tags

                        string tag = "";
                        bool partOfLongertag = false;

                        foreach (string word in song.tag_list.Split(' '))
                        {
                            if (word.EndsWith("\""))
                            {
                                tag += " " + word.Substring(0, word.Length - 1);
                                partOfLongertag = false;
                                listGenreAndTags.Add(tag);
                                tag = "";
                                continue;
                            }
                            else if (word.StartsWith("\""))
                            {
                                partOfLongertag = true;
                                tag += word.Substring(1, word.Length - 1);
                            }
                            else if (partOfLongertag == true)
                            {
                                tag += " " + word;
                            }
                            else
                            {
                                tag = word;
                                listGenreAndTags.Add(tag);
                                tag = "";
                            }
                        }
                        tagFile.Tag.Genres = listGenreAndTags.ToArray();
                    }
                    if (!String.IsNullOrEmpty(song.description))
                    {
                        tagFile.Tag.Comment = song.description;
                    }
                    if (!String.IsNullOrEmpty(song.artwork_url)) 
                    {
                        // download artwork
                        artworkFilepath = Path.GetTempFileName();

                        string highResArtwork_url = song.artwork_url.Replace("large.jpg", "t500x500.jpg");

                        using (WebClient web = new WebClient()) 
                        {
                            web.DownloadFile(highResArtwork_url, artworkFilepath);
                        }
                        tagFile.Tag.Pictures = new[] { new TagLib.Picture(artworkFilepath) };
                    }
                    tagFile.Save();
                    tagFile.Dispose();

                    if (artworkFilepath != null && File.Exists(artworkFilepath))
                    {
                        File.Delete(artworkFilepath);
                    }

                    lock (SongsDownloadedLock)
                    {
                        SongsDownloaded.Add(song);
                        downloaded = true;
                    }
                }
            }
            return downloaded;
        }

        private void DeleteOrAddRemovedTrack(string directoryPath, IList<Track> allTracks, bool deleteTrack)
        {
            string manifestPath = DetermineManifestPath(directoryPath);
            try
            {
                if (File.Exists(manifestPath))
                {
                    string[] songsDownloaded = File.ReadAllLines(manifestPath);
                    IList<string> newManifest = new List<string>();

                    foreach (string songDownloaded in songsDownloaded)
                    {
                        string localPathDownloadedSong = ParseTrackPath(songDownloaded, 1);
                        //if file does not exist anymore, 
                        //it will be redownloaded by not adding it to the newManifest
                        if (Form1.RedownloadLocallyRemovedOrAltered && !File.Exists(localPathDownloadedSong))                
                        {
                            continue;
                        };
                        //WARNING      If we want to look if allTracks contains the downloaded file we need to trim the extention
                        //              because allTracks doesn't store the extention of the path
                        string neutralPath = Path.ChangeExtension(localPathDownloadedSong, null);
                        int canBeDeleted = allTracks.Count(song => song.LocalPath.Contains(neutralPath));

                        if (deleteTrack && canBeDeleted == 0)
                        {
                            File.Delete(localPathDownloadedSong);
                        }                                            
                        else
                        {
                            newManifest.Add(songDownloaded);
                        }                      
                    }                 
                    // the manifest is updated again later, but might as well update it here
                    // to save the deletions in event of crash or abort
                    File.WriteAllLines(manifestPath, newManifest);        
                }
            }
            catch (Exception)
            {
                IsError = true;
                throw new Exception("Unable to read manifest to determine tracks to delete");
            }      
        }

        private IList<Track> DetermineTracksToDownload(string directoryPath, IList<Track> allSongs, bool foldersPerArtist)
        {
              
            string manifestPath = DetermineManifestPath(directoryPath);

            IList<string> streamUrls = new List<string>();

            if (File.Exists(manifestPath))
            {
                foreach (string track in File.ReadAllLines(manifestPath))
                {
                    streamUrls.Add(ParseTrackPath(track,0));
                }               
            }
  
            return allSongs.Where(s => !streamUrls.Contains(s.EffectiveDownloadUrl)).ToList();
        }

        private string RetrieveJson(string url, string clientId, int? limit = null, int? offset = null)
        {        
            string json = null;
            
            try
            {
                using (WebClient client = new WebClient()) 
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    if (url != null && !url.Contains("client_id="))
                    {
                        url += (url.Contains("?") ? "&" : "?") + "client_id=" + clientId;
                    }
                    if (limit != null)
                    {
                        url += "&limit=" + limit;
                    }
                    if (offset != null)
                    {
                        url += "&offset=" + offset;
                    }

                    json = client.DownloadString(url);
                }
             
            }
            catch (Exception)
            {
                // Nothing to do here
            }

            return json;
        }

        private string DetermineManifestPath(string directoryPath)
        {
            return Path.Combine(directoryPath, "manifest");
        }

        private string ParseTrackPath(string csv, int position)
        {           
            if(csv != null && csv.IndexOf(',') >= 0)
            {
                return csv.Split(',')[position]; //position 0 is streampath, position 1 is local path
            }
            else
            {
                return csv;
            }
        }
    }  
}
