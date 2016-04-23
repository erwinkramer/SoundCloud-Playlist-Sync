using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Soundcloud_Playlist_Downloader.Properties;

namespace Soundcloud_Playlist_Downloader
{
    internal class PlaylistSync
    {
        public enum DownloadMode
        {
            Playlist,
            Favorites,
            Artist,
            Track
        }

        private static readonly object WriteManifestLock = new object();
        protected static object WritePlaylistLock = new object();

        private readonly object _songsDownloadedLock = new object();

        public PlaylistSync()
        {
            SongsToDownload = 0;
            SongsDownloaded = 0;
            ResetProgress();
        }

        public bool IsError { get; protected set; }

        public int SongsToDownload { get; private set; }
        public int SongsDownloaded { get; private set; }

        public bool IsActive { get; set; }
        public bool DownloadingSingleTrack { get; set; }

        private void VerifyParameters(Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrWhiteSpace(parameter.Value))
                {
                    IsError = true;
                    throw new Exception($"{parameter.Key} must be specified");
                }
            }
        }

        internal void Synchronize(string url, DownloadMode mode, string directory, string clientId)
        {
            VerifyParameters(
                new Dictionary<string, string>
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
                        apiURL = DetermineApiUrlForNormalUrl(url, clientId, "playlists");
                    }
                    else
                    {
                        apiURL = url;
                    }
                    SynchronizeFromPlaylistApiUrl(apiURL, clientId, directory);
                    break;
                case DownloadMode.Favorites:
                    // get the username from the url and then call SynchronizeFromProfile
                    var username = ParseUserIdFromProfileUrl(url);
                    SynchronizeFromProfile(username, clientId, directory);
                    break;
                case DownloadMode.Artist:

                    if (!url.Contains("api.soundcloud.com"))
                    {
                        apiURL = DetermineApiUrlForNormalUrl(url, clientId, "tracks");
                    }
                    else
                    {
                        apiURL = url;
                    }
                    SynchronizeFromArtistUrl(apiURL, clientId, directory);
                    break;
                case DownloadMode.Track:
                    Track track = RetrieveTrackFromUrl(url, clientId);
                    SynchronizeSingleTrack(track, clientId, directory);
                    break;
                default:
                    IsError = true;
                    throw new NotImplementedException("Unknown download mode");
            }
        }

        private string DetermineApiUrlForNormalUrl(string url, string clientId, string resulttype)
        {
            // parse the username from the url
            var username = ParseUserIdFromProfileUrl(url);
            string playlistName = null;
            try
            {
                // parse the playlist name from the url
                var startingPoint = "/sets/";
                var startingIndex = url.IndexOf(startingPoint, StringComparison.Ordinal) + startingPoint.Length;
                var endingIndex = url.Substring(startingIndex).Contains("/")
                    ? url.Substring(startingIndex).IndexOf("/", StringComparison.Ordinal) + startingIndex
                    : url.Length;
                playlistName = url.Substring(startingIndex, endingIndex - startingIndex);
            }
            catch (Exception e)
            {
                IsError = true;
                throw new Exception("Invalid playlist url: " + e.Message);
            }

            // hit the users/username/playlists endpoint and match the playlist on the permalink
            var userUrl = "https://api.soundcloud.com/users/" + username + "/" + resulttype;

            if (resulttype == "tracks")
            {
                return userUrl;
            }

            return "https://api.soundcloud.com/playlists/" +
                   RetrievePlaylistId(userUrl, playlistName, clientId);
        }

        private string RetrievePlaylistId(string userApiUrl, string playlistName, string clientId)
        {
            // grab the xml from the url, parse each playlist out, match the name based on the
            // permalink, and return the id of the matching playlist.
            // a method already exists for downloading xml, so use that and refactor this to not have
            // the client id embedded in the url
            var playlistsJson = RetrieveJson(userApiUrl, clientId);
            
            var playlists = JArray.Parse(playlistsJson);
            IList<JToken> results = playlists.Children().ToList();
            IList<PlaylistItem> playlistsitems = new List<PlaylistItem>();

            //var playlistItems = JsonConvert.DeserializeObject<JsonPoco.PlaylistRoot>(playlistsJson).PlaylistItems;
            foreach (var result in results)
            {
                var playlistsitem = JsonConvert.DeserializeObject<PlaylistItem>(result.ToString());
                playlistsitems.Add(playlistsitem);
            }

            var matchingPlaylistItem = playlistsitems.FirstOrDefault(s => s.permalink == playlistName);

            if (matchingPlaylistItem != null)
            {
                return matchingPlaylistItem.id.ToString();
            }
            IsError = true;
            throw new Exception("Unable to find a matching playlist");
        }

        private Track RetrieveTrackFromUrl(string url, string clientId)
        {
            var trackJson = RetrieveJson("https://api.soundcloud.com/resolve.json?url=" + url, clientId);
            JObject track = JObject.Parse(trackJson);
            if(track != null && track.GetValue("id") != null)
                return JsonConvert.DeserializeObject<Track>(track.ToString());
            
            return null;
        }

        private string ParseUserIdFromProfileUrl(string url)
        {
            try
            {
                var startingPoint = "soundcloud.com/";
                var startingIndex = url.IndexOf(startingPoint, StringComparison.Ordinal) + startingPoint.Length;
                var endingIndex = url.Substring(startingIndex).Contains("/")
                    ? url.Substring(startingIndex).IndexOf("/", StringComparison.Ordinal) + startingIndex
                    : url.Length;

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
            var limit = isRawTracksUrl ? 200 : 0;
                //200 is the limit set by SoundCloud itself. Remember; limits are only with 'collection' types in JSON 
            IList<Track> tracks = new List<Track>();

            try
            {
                // get the tracks embedded in the playlist
                var tracksAdded = true;

                var tracksJson = RetrieveJson(url, clientId, limit);
                var lastStep = false;

                while (tracksAdded && tracksJson != null)
                {
                    var JOBtracksJson = JObject.Parse(tracksJson);

                    IList<JToken> JTOKENcurrentTracks = isRawTracksUrl
                        ? JOBtracksJson["collection"].Children().ToList()
                        : JOBtracksJson["tracks"].Children().ToList();

                    IList<Track> currentTracks = new List<Track>();
                    foreach (var Jtrack in JTOKENcurrentTracks)
                    {
                        var currentTrack = JsonConvert.DeserializeObject<Track>(Jtrack.ToString());
                        currentTracks.Add(currentTrack);
                    }

                    if (currentTracks.Any())
                    {
                        foreach (var track in currentTracks)
                        {
                            tracks.Add(track);
                        }
                        tracksAdded = true;
                    }
                    else
                    {
                        tracksAdded = false;
                    }

                    if (lastStep)
                        break;

                    var linkedPartitioningUrl = JsonConvert.DeserializeObject<NextInfo>(tracksJson).next_href;
                    tracksJson = RetrieveJson(linkedPartitioningUrl, null);
                    if (string.IsNullOrEmpty(tracksJson))
                    {
                        lastStep = true;
                    }
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
            var tracks = EnumerateTracksFromUrl("https://api.soundcloud.com/users/" + username + "/favorites", clientId,
                true);
            Synchronize(tracks, clientId, directoryPath);
        }

        public static bool IsPathWithinLimits(string fullPathAndFilename)
        {
            //In the Windows API the maximum length for a path is MAX_PATH, which is defined as 260 characters.
            //We'll make it 250 because there will be an extention and, in some cases, an HQ tag appended to the filename.  
            const int maxPathLength = 250;
            return fullPathAndFilename.Length <= maxPathLength;
        }

        internal void SynchronizeSingleTrack(Track track, string clientId, string directoryPath)
        {
            track.LocalPath = GetTrackLocalPath(track, directoryPath);

            //Downloads track
            DownloadingSingleTrack = true;
            DownloadTrack(track, clientId);
            DownloadingSingleTrack = false;
        }

        private string GetTrackLocalPath(Track track, string directoryPath)
        {
            string path;
            var validArtist = track.CoerceValidFileName(track.Artist, true);
            var validArtistFolderName = Track.TrimDotsAndSpacesForFolderName(validArtist);
            var validTitle = track.CoerceValidFileName(track.Title, true);
            var filenameWithArtist = validArtist + " - " + validTitle;

            if (Form1.FoldersPerArtist)
            {
                if (Form1.IncludeArtistInFilename) //include artist name
                {
                    while (!IsPathWithinLimits(path = Path.Combine(directoryPath, validArtistFolderName,
                        filenameWithArtist)))
                    {
                        filenameWithArtist = filenameWithArtist.Remove(filenameWithArtist.Length - 2);
                        //shorten to fit into max size of path
                    }
                }
                else
                {
                    while (!IsPathWithinLimits(path = Path.Combine(directoryPath, validArtistFolderName,
                        validTitle)))
                    {
                        validTitle = validTitle.Remove(validTitle.Length - 2);
                        //shorten to fit into max size of path
                    }
                }
            }
            else
            {
                if (Form1.IncludeArtistInFilename) //include artist name
                {
                    while (!IsPathWithinLimits(path = Path.Combine(directoryPath, filenameWithArtist)))
                    {
                        filenameWithArtist = filenameWithArtist.Remove(filenameWithArtist.Length - 2);
                        //shorten to fit into max size of path
                    }
                }
                else
                {
                    while (!IsPathWithinLimits(path = Path.Combine(directoryPath, validTitle)))
                    {
                        validTitle = validTitle.Remove(validTitle.Length - 2);
                        //shorten to fit into max size of path
                    }
                }
            }
            if (track.IsHD)
            {
                path += " (HQ)";
            }

            return path;
        }

        private void Synchronize(IList<Track> tracks, string clientId, string directoryPath)
        {
            //define all local paths by combining the sanitzed artist (if checked by user) with the santized title
            foreach (var track in tracks)
            {
                track.LocalPath = GetTrackLocalPath(track, directoryPath);
            }
            
            // determine which tracks should be deleted or re-added
            DeleteOrAddRemovedTrack(directoryPath, tracks);

            // determine which tracks should be downloaded
            DetermineTracksToDownload(directoryPath, ref tracks);

            // download the relevant tracks and continuously update the manifest
            DownloadSongs(tracks, clientId, directoryPath);

            //Create playlist file
            var completed = PlaylistCreator.CreateSimpleM3U(tracks, directoryPath);

            var songstodownload = tracks.Count(x => x.HasToBeDownloaded);
            // validation
            if (songstodownload > 0 && IsActive)
            {
                IsError = true;
                throw new Exception(
                    "Some tracks failed to download. You might need to try a few more times before they can download correctly. " +
                    "The following tracks were not downloaded:" + Environment.NewLine +
                    string.Join(Environment.NewLine,
                        tracks.Where(x => x.HasToBeDownloaded)
                            .Select(x => "Title: " + x.Title + ", Artist: " + x.Artist)
                        ));
            }
        }


        internal void SynchronizeFromPlaylistApiUrl(string playlistApiUrl, string clientId, string directoryPath)
        {
            var tracks = EnumerateTracksFromUrl(playlistApiUrl, clientId, false);
            Synchronize(tracks, clientId, directoryPath);
        }


        internal void SynchronizeFromArtistUrl(string artistUrl, string clientId, string directoryPath)
        {
            var tracks = EnumerateTracksFromUrl(artistUrl, clientId, true);
            Synchronize(tracks, clientId, directoryPath);
        }


        private void ResetProgress()
        {
            SongsDownloaded = 0;
            SongsToDownload = 0;
            IsActive = true;
            IsError = false;
        }       

        private void UpdateSyncManifest(Track trackDownloaded, string directoryPath)
        {
            string track = null;
            track = trackDownloaded.EffectiveDownloadUrl + "," + trackDownloaded.LocalPath.Replace(directoryPath, "");
            IList<string> content = new List<string>();
            content.Add(track);

            var updateSuccesful = false;
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {
                    lock (WriteManifestLock)
                    {
                        var manifestPath = DetermineManifestPath(directoryPath);
                        File.AppendAllLines(manifestPath, content);
                            //if file does not exist, this function will create one
                        updateSuccesful = true;
                        break;
                    }
                }
                catch (Exception)
                {
                }
                Thread.Sleep(50); // Pause 50ms before new attempt
            }
            if (!updateSuccesful)
            {
                IsError = true;
                throw new Exception("Unable to update manifest");
            }
        }

        private void DownloadSongs(IList<Track> Alltracks, string apiKey, string directoryPath)
        {
            var trackLock = new object();
            SongsToDownload = Alltracks.Count(x => x.HasToBeDownloaded);
            Parallel.ForEach(Alltracks.Where(x => x.HasToBeDownloaded),
                new ParallelOptions { MaxDegreeOfParallelism = Settings.Default.ConcurrentDownloads },
                track =>
                {
                    try
                    {
                        if (DownloadTrack(track, apiKey))
                        {
                            lock (trackLock)
                            {
                                track.HasToBeDownloaded = false;
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
        }

        private bool DownloadTrack(Track song, string apiKey)
        {
            var downloaded = false;
            if (!IsActive) return false;

            using (var client = new WebClient())
            {
                if (song?.LocalPath != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(song.LocalPath));

                    if (song.IsHD)
                    {
                        string extension = null;
                        try
                        {
                            var request = WebRequest.Create(song.EffectiveDownloadUrl +
                                                            $"?client_id={apiKey}");

                            request.Method = "HEAD";
                            using (var response = request.GetResponse())
                            {
                                extension = "." + response.Headers["x-amz-meta-file-type"];
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionHandler.handleException(e);

                            //the download link might have been invalid, so we get the stream download instead
                            if (song.stream_url == null)
                                //all hope is lost when there is no stream url, return to safety
                                return false;

                            var request = WebRequest.Create(song.stream_url + $"?client_id={apiKey}");

                            request.Method = "HEAD";
                            using (var response = request.GetResponse())
                            {
                                extension = "." + response.Headers["x-amz-meta-file-type"];
                            }
                        }
                        var allowedFormats = new List<string>();
                        allowedFormats.AddRange(new[] {".wav", ".aiff", ".aif", ".m4a", ".aac"});
                        if (Form1.excludeAAC)
                        {
                            allowedFormats.Remove(".aac");
                        }
                        if (Form1.excludeM4A)
                        {
                            allowedFormats.Remove(".m4a");
                        }
                        if (Form1.ConvertToMp3 && Form1.Highqualitysong && allowedFormats.Contains(extension))
                        {
                            //get the wav song as byte data, as we won't store it just yet
                            var soundbytes = client.DownloadData(song.EffectiveDownloadUrl +
                                                                 $"?client_id={apiKey}");
                            //convert to mp3 & then write bytes to file
                            var succesfulConvert = AudioConverter.ConvertAllTheThings(soundbytes, ref song, extension);
                            soundbytes = null;
                            if (!succesfulConvert)
                                //something has gone wrong, download the stream url instead of download url 
                            {
                                song.LocalPath += ".mp3";
                                client.DownloadFile(song.stream_url + $"?client_id={apiKey}", song.LocalPath);
                            }
                        }
                        else if (extension == ".mp3") //get the high res mp3 without converting
                        {
                            song.LocalPath += extension;
                            client.DownloadFile(song.EffectiveDownloadUrl + $"?client_id={apiKey}", song.LocalPath);
                        }
                        else //get the low res mp3 if all above not possible
                        {
                            song.LocalPath += extension;
                            client.DownloadFile(song.stream_url + $"?client_id={apiKey}", song.LocalPath);
                        }
                    }
                    else
                    {
                        song.LocalPath += ".mp3";
                        client.DownloadFile(song.stream_url + $"?client_id={apiKey}", song.LocalPath);
                    }

                    //tag the song
                    try
                    {
                        MetadataTagging.TagIt(ref song);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Can't tag song:" + song.LocalPath);
                    }
                }


                lock (_songsDownloadedLock)
                {
                    SongsDownloaded++;
                    downloaded = true;
                }
            }
            return true;
        }

        private void DeleteOrAddRemovedTrack(string directoryPath, IList<Track> allTracks)
        {
            var manifestPath = DetermineManifestPath(directoryPath);
            try
            {
                if (File.Exists(manifestPath))
                {
                    var songsDownloaded = File.ReadAllLines(manifestPath);
                    IList<string> newManifest = new List<string>();

                    foreach (var songDownloaded in songsDownloaded)
                    {
                        var localTrackpath = Utils.ParseTrackPath(songDownloaded, 1);
                        var localPathDownloadedSongRelative = directoryPath + localTrackpath.Replace(directoryPath, "");
                        var songId =
                            new string(
                                Utils.ParseTrackPath(songDownloaded, 0)
                                    .ToCharArray()
                                    .Where(char.IsDigit)
                                    .ToArray());
                        var neutralPath = Path.ChangeExtension(localPathDownloadedSongRelative, null);
                        Track soundCloudTrack = null;
                        soundCloudTrack = allTracks.FirstOrDefault(song => song.stream_url.Contains("/" + songId + "/"));

                        var trackArtistOrNameChanged = false;
                        //WARNING      If we want to look if allTracks contains the downloaded file we need to trim the extention
                        //              because allTracks doesn't store the extention of the path                            
                        trackArtistOrNameChanged = !allTracks.Any(song => song.LocalPath.Contains(neutralPath));

                        //file does not exist anymore, it will be redownloaded by not adding it to the newManifest
                        if (!File.Exists(localPathDownloadedSongRelative))
                        {
                            continue;
                        }
                        ;
                        //song is changed on SoundCloud (only checks artist and filename), redownload and remove old one.
                        if (trackArtistOrNameChanged && soundCloudTrack != null)
                        {
                            var localIsHd = Utils.ParseTrackPath(songDownloaded, 0).EndsWith("download");
                            if (soundCloudTrack.IsHD || (soundCloudTrack.IsHD == false && localIsHd == false))
                                // do not download Low Quality if HQ is already downloaded, even if the track is changed!
                            {
                                if (File.Exists(localPathDownloadedSongRelative))
                                {
                                    File.Delete(localPathDownloadedSongRelative);
                                    DeleteEmptyDirectory(localPathDownloadedSongRelative);
                                }
                                continue;
                            }
                        }
                        //file exists locally but not externally and can be removed
                        if (Form1.SyncMethod == 2 && soundCloudTrack == null)
                        {
                            File.Delete(localPathDownloadedSongRelative);
                            DeleteEmptyDirectory(localPathDownloadedSongRelative);
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
            catch (Exception e)
            {
                IsError = true;
                throw new Exception("Unable to read manifest to determine tracks to delete; exception: " + e);
            }
        }

        public bool DeleteEmptyDirectory(string filenameWithPath)
        {
            if (!Form1.FoldersPerArtist)
                return false;
            var path = Path.GetDirectoryName(filenameWithPath);
            if (path != null && !Directory.EnumerateFileSystemEntries(path).Any()) //folder = empty
            {
                try
                {
                    Directory.Delete(path, false); //recursive not true because should be already empty
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        private void DetermineTracksToDownload(string directoryPath, ref IList<Track> allSongs)
        {
            var manifestPath = DetermineManifestPath(directoryPath);
            IList<string> streamUrls = new List<string>();
            IList<string> songsDownloaded = new List<string>();
            if (File.Exists(manifestPath))
            {
                songsDownloaded = File.ReadAllLines(manifestPath);
                foreach (var track in File.ReadAllLines(manifestPath))
                {
                    streamUrls.Add(Utils.ParseTrackPath(track, 0));
                }
            }
            foreach (var track in allSongs)
            {
                if (!streamUrls.Contains(track.EffectiveDownloadUrl))
                    track.HasToBeDownloaded = true;
                else if (songsDownloaded.Count > 0)
                {
                    // we need to add the extention to the local path for further use
                    // the only way we can know what the extention was when previously downloaded 
                    // is by checking the file directly, or by checking the manifest file, 
                    // we will do the latter
                    track.LocalPath += PlaylistCreator.GetExtension(songsDownloaded, track.LocalPath);
                }
            }
        }

        private string RetrieveJson(string url, string clientId = null, int? limit = null, int? offset = null)
        {
            string json = null;
            if (limit == 0)
                limit = null;

            if (string.IsNullOrEmpty(url))
                return null;
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    if (!url.Contains("client_id="))
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

                    if (limit != null)
                        url += "&linked_partitioning=1"; //will add next_href to the response

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
    }
}