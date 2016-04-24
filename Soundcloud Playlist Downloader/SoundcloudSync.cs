using System;
using System.Collections.Generic;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Utils;

namespace Soundcloud_Playlist_Downloader
{
    internal class SoundcloudSync
    {
        public SoundcloudSync()
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
                    var username = DownloadUtils.ParseUserIdFromProfileUrl(url);
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
                    Track track = JsonUtils.RetrieveTrackFromUrl(url, clientId);
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
            var username = DownloadUtils.ParseUserIdFromProfileUrl(url);
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
                   JsonUtils.RetrievePlaylistId(userUrl, playlistName, clientId);
        }                  

        internal void SynchronizeFromProfile(string username, string clientId, string directoryPath)
        {
            // hit the /username/favorites endpoint for the username in the url, then download all the tracks
            var tracks = JsonUtils.RetrieveTracksFromUrl("https://api.soundcloud.com/users/" + username + "/favorites", clientId,
                true);
            SyncUtils.Synchronize(tracks, clientId, directoryPath);
        } 
        internal void SynchronizeSingleTrack(Track track, string clientId, string directoryPath)
        {
            DownloadUtils.SongsToDownload = 1;
            track.LocalPath = FilesystemUtils.BuildTrackLocalPath(track, directoryPath);
            DownloadUtils.DownloadTrackAndTag(ref track, clientId);
        }             
        internal void SynchronizeFromPlaylistApiUrl(string playlistApiUrl, string clientId, string directoryPath)
        {
            var tracks = JsonUtils.RetrieveTracksFromUrl(playlistApiUrl, clientId, false);
            SyncUtils.Synchronize(tracks, clientId, directoryPath);
        }
        internal void SynchronizeFromArtistUrl(string artistUrl, string clientId, string directoryPath)
        {
            var tracks = JsonUtils.RetrieveTracksFromUrl(artistUrl, clientId, true);
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