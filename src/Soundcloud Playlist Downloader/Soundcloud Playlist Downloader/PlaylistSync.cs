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
using TagLib.Id3v2;

namespace Soundcloud_Playlist_Downloader
{
    class PlaylistSync
    {
        public bool IsError { get; protected set; }

        public enum DownloadMode { Playlist, Favorites, Artist };

        public IList<Track> SongsToDownload { get; private set; }
        public IList<Track> SongsDownloaded { get; private set; }

        private object SongsDownloadedLock = new object();
        private static object WriteManifestLock = new object();
        protected static object WritePlaylistLock = new object();

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

        internal void Synchronize(string url, DownloadMode mode, string directory, string clientId)
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
                    SynchronizeFromPlaylistAPIUrl(apiURL, clientId, directory);
                    break;
                case DownloadMode.Favorites:
                    // get the username from the url and then call SynchronizeFromProfile
                    string username = parseUserIdFromProfileUrl(url);
                    SynchronizeFromProfile(username, clientId, directory);
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
                    SynchronizeFromArtistUrl(apiURL, clientId, directory);
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

        internal void SynchronizeFromProfile(string username, string clientId, string directoryPath)
        {
            // hit the /username/favorites endpoint for the username in the url, then download all the tracks
            IList<Track> tracks = EnumerateTracksFromUrl("https://api.soundcloud.com/users/" + username + "/favorites", clientId, true);
            Synchronize(tracks, clientId, directoryPath);
        }

        public static bool IsPathWithinLimits(string fullPathAndFilename)
        {
            //In the Windows API the maximum length for a path is MAX_PATH, which is defined as 260 characters.
            //We'll make it 250 because there will be an extention and, in some cases, an HQ tag appended to the filename.  
            const int MAX_PATH_LENGTH = 250;
            return fullPathAndFilename.Length <= MAX_PATH_LENGTH;
        }

        private void Synchronize(IList<Track> tracks, string clientId, string directoryPath)
        {
            //define all local paths by combining the sanitzed artist (if checked by user) with the santized title
            foreach(Track track in tracks)
            {
                string validArtist = track.CoerceValidFileName(track.Artist, true);
                string validTitle = track.CoerceValidFileName(track.Title, true);
                string filenameWithArtist = validArtist + " - " + validTitle;

                if (Form1.FoldersPerArtist)
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

            IList<Track> cleantracks = tracks;

            // determine which tracks should be deleted or re-added
            DeleteOrAddRemovedTrack(directoryPath, tracks);

            // determine which tracks should be downloaded
            SongsToDownload = DetermineTracksToDownload(directoryPath, tracks);

            // download the relevant tracks and continuously update the manifest
            IList<Track> songsDownloaded = DownloadSongs(SongsToDownload, clientId, directoryPath);

            //Create playlist file
            PlaylistCreator playlistCreate = new PlaylistCreator();
            playlistCreate.createSimpleM3U(cleantracks, directoryPath);

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


        internal void SynchronizeFromPlaylistAPIUrl(string playlistApiUrl, string clientId, string directoryPath)
        {
            IList<Track> tracks = EnumerateTracksFromUrl(playlistApiUrl, clientId, false);
            Synchronize(tracks, clientId, directoryPath);
        }


        internal void SynchronizeFromArtistUrl(string artistUrl, string clientId, string directoryPath)
        {

            IList<Track> tracks = EnumerateTracksFromUrl(artistUrl, clientId, true);
            Synchronize(tracks, clientId, directoryPath);
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
                lock(WriteManifestLock)
                {
                    string manifestPath = DetermineManifestPath(directoryPath);
                    File.AppendAllLines(manifestPath, content); //if file does not exist, this function will create one                                 
                }
            }
            catch (Exception)
            {
                IsError = true;
                throw new Exception("Unable to update manifest");
            }
        }

        private void UpdateSyncManifest(Track trackDownloaded, string directoryPath)
        {
            string track = null;
            track = trackDownloaded.EffectiveDownloadUrl + "," + trackDownloaded.LocalPath;
            IList<string> content = new List<string>();
            content.Add(track);

            bool updateSuccesful = false;
            for (int attempts = 0; attempts < 5; attempts++)
            {
                try
                {
                    lock (WriteManifestLock)
                    {
                        string manifestPath = DetermineManifestPath(directoryPath);
                        File.AppendAllLines(manifestPath, content); //if file does not exist, this function will create one
                        updateSuccesful = true;
                        break;
                    }
                }
                catch (Exception)
                {                  
                }
                System.Threading.Thread.Sleep(50); // Pause 50ms before new attempt
            }
            if(!updateSuccesful)
            {
                IsError = true;
                throw new Exception("Unable to update manifest");
            }

        }

        private IList<Track> DownloadSongs(IList<Track> TracksToDownload, string apiKey, string directoryPath)
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
                            UpdateSyncManifest(track, directoryPath);
                        }
                    }

                }
                catch (Exception e)
                {
                    IsError = true;
                    ExceptionHandler.handleException(e);
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
                            WebRequest request = WebRequest.Create(song.EffectiveDownloadUrl + string.Format("?client_id={0}", apiKey));

                            request.Method = "HEAD";
                            using (WebResponse response = request.GetResponse())
                            {
                                extension = Path.GetExtension(response.Headers["Content-Disposition"]
                                    .Replace("attachment;filename=", "").Replace("\"", ""));
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionHandler.handleException(e);

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
                        var allowedFormats = new List<string>();
                        allowedFormats.AddRange(new string[] { ".wav", ".aiff", ".aif", ".m4a", ".aac"});
                        if(Form1.excludeAAC)
                        {
                            allowedFormats.Remove(".aac");
                        }
                        if(Form1.excludeM4A)
                        {
                            allowedFormats.Remove(".m4a");
                        }
                        if (Form1.ConvertToMp3 && Form1.Highqualitysong && (allowedFormats.Contains(extension)))
                        {
                            //get the wav song as byte data, as we won't store it just yet
                            byte[] soundbytes = client.DownloadData(song.EffectiveDownloadUrl + string.Format("?client_id={0}", apiKey));
                            //convert to mp3 & then write bytes to file
                            bool succesfulConvert = audioConverter.ConvertAllTheThings(soundbytes, ref song, extension);
                            if (!succesfulConvert) //something has gone wrong, just write original bytes then 
                            {
                                song.LocalPath += extension;
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

                    //tag the song
                    metadataTagging.tagIt(ref song);

                    lock (SongsDownloadedLock)
                    {
                        SongsDownloaded.Add(song);
                        downloaded = true;
                    }
                }
            }
            return downloaded;
        }

        private void DeleteOrAddRemovedTrack(string directoryPath, IList<Track> allTracks)
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

                        if (Form1.DeleteExternallyRemovedOrAlteredSongs && canBeDeleted == 0)
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

        private IList<Track> DetermineTracksToDownload(string directoryPath, IList<Track> allSongs)
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
            catch (Exception e)
            {
                IsError = true;
                ExceptionHandler.handleException(e);
            }

            return json;
        }

        public string DetermineManifestPath(string directoryPath)
        {
            return Path.Combine(directoryPath, Form1.ManifestName);
        }

        private string ParseTrackPath(string csv, int position)
        {           
            if(csv != null && csv.IndexOf(',') >= 0)
            {
                //only make 1 split, as a comma (,) can be found in a song name!
                return csv.Split(new[] { ',' }, 2)[position]; //position 0 is streampath, position 1 is local path
            }
            else
            {
                return csv;
            }
        }
    }  
}
