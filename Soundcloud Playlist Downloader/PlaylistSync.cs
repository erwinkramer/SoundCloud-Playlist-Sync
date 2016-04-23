using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Soundcloud_Playlist_Downloader.Utils;

namespace Soundcloud_Playlist_Downloader
{
    internal class PlaylistSync
    {
        public PlaylistSync()
        {
            DownloadUtils.SongsToDownload = 0;
            DownloadUtils.SongsDownloaded = 0;
            ResetProgress();
        }

        public static bool IsError { get; set; }      
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

        internal void Synchronize(string url, EnumUtil.DownloadMode mode, string directory, string clientId)
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
                case EnumUtil.DownloadMode.Playlist:
                    // determine whether it is an api url or a normal url. if it is a normal url, get the api url from it
                    // and then call SynchronizeFromPlaylistAPIUrl. Otherwise just call that method directly

                    if (url.Contains("api.soundcloud.com"))
                        apiURL = url;
                    else
                        apiURL = DetermineApiUrlForNormalUrl(url, clientId, "playlists");
                    SynchronizeFromPlaylistApiUrl(apiURL, clientId, directory);
                    break;
                case EnumUtil.DownloadMode.Favorites:
                    // get the username from the url and then call SynchronizeFromProfile
                    var username = ParseUserIdFromProfileUrl(url);
                    SynchronizeFromProfile(username, clientId, directory);
                    break;
                case EnumUtil.DownloadMode.Artist:

                    if (url.Contains("api.soundcloud.com"))
                        apiURL = url;
                    else
                        apiURL = DetermineApiUrlForNormalUrl(url, clientId, "tracks");
                    SynchronizeFromArtistUrl(apiURL, clientId, directory);
                    break;
                case EnumUtil.DownloadMode.Track:
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
            var playlistsJson = JsonUtils.RetrieveJson(userApiUrl, clientId);
            
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
            var trackJson = JsonUtils.RetrieveJson("https://api.soundcloud.com/resolve.json?url=" + url, clientId);
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

                var tracksJson = JsonUtils.RetrieveJson(url, clientId, limit);
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
                    }
                    else
                    {
                        tracksAdded = false;
                    }

                    if (lastStep)
                        break;

                    var linkedPartitioningUrl = JsonConvert.DeserializeObject<NextInfo>(tracksJson).next_href;
                    tracksJson = JsonUtils.RetrieveJson(linkedPartitioningUrl, null);
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
            SyncUtils.Synchronize(tracks, clientId, directoryPath);
        } 
        internal void SynchronizeSingleTrack(Track track, string clientId, string directoryPath)
        {
            track.LocalPath = FilesystemUtils.GetTrackLocalPath(track, directoryPath);
            DownloadUtils.DownloadTrack(track, clientId);
        }             
        internal void SynchronizeFromPlaylistApiUrl(string playlistApiUrl, string clientId, string directoryPath)
        {
            var tracks = EnumerateTracksFromUrl(playlistApiUrl, clientId, false);
            SyncUtils.Synchronize(tracks, clientId, directoryPath);
        }
        internal void SynchronizeFromArtistUrl(string artistUrl, string clientId, string directoryPath)
        {
            var tracks = EnumerateTracksFromUrl(artistUrl, clientId, true);
            SyncUtils.Synchronize(tracks, clientId, directoryPath);
        }
        private void ResetProgress()
        {
            DownloadUtils.SongsDownloaded = 0;
            DownloadUtils.SongsToDownload = 0;
            DownloadUtils.IsActive = true;
            IsError = false;
        }
    }
}