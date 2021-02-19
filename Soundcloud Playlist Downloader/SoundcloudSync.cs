using System;
using System.Collections.Generic;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Utils;

namespace Soundcloud_Playlist_Downloader
{
    internal class SoundcloudSync
    {
        public SyncUtils _syncUtil;
        public JsonUtils JsonUtil;
        public bool MergePlaylists;

        public SoundcloudSync(SyncUtils syncUtil, bool mergePlaylists)
        {
            MergePlaylists = mergePlaylists;
            _syncUtil = syncUtil;
            JsonUtil = new JsonUtils(_syncUtil.ManifestUtil, _syncUtil.DownloadUtil.ClientIDsUtil);
        }
        private void VerifyParameters(Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrWhiteSpace(parameter.Value))
                {
                    _syncUtil.ManifestUtil.ProgressUtil.HasErrors = true;
                    throw new Exception($"{parameter.Key} must be specified");
                }
            }
        }
        internal void Synchronize(string url)
        {
            VerifyParameters(
                new Dictionary<string, string>
                {
                    {"URL", url},
                    {"Directory", _syncUtil.ManifestUtil.FileSystemUtil.Directory.FullName},
                    {"Client ID", _syncUtil.DownloadUtil.ClientIDsUtil.ClientIdCurrentValue}
                }
                );

            string apiUrl;
            switch (_syncUtil.ManifestUtil.DownloadMode)
            {
                case EnumUtil.DownloadMode.Playlist:
                    // determine whether it is an api url or a normal url. if it is a normal url, get the api url from it
                    // and then call SynchronizeFromPlaylistAPIUrl. Otherwise just call that method directly
                    apiUrl = url.Contains("api.soundcloud.com") ? url : "https://api.soundcloud.com/playlists/" + GetPlaylistId(url);
                    SynchronizeFromPlaylistApiUrl(apiUrl);
                    break;
                case EnumUtil.DownloadMode.UserPlaylists:
                    // get the username from the url and then call SynchronizeFromUserPlaylists
                    var user = _syncUtil.DownloadUtil.ParseUserIdFromProfileUrl(url);
                    SynchronizeFromUserPlaylists(user);
                    break;
                case EnumUtil.DownloadMode.Favorites:
                    // get the username from the url and then call SynchronizeFromProfile
                    var username = _syncUtil.DownloadUtil.ParseUserIdFromProfileUrl(url);
                    SynchronizeFromProfile(username);
                    break;
                case EnumUtil.DownloadMode.Artist:
                    apiUrl = url.Contains("api.soundcloud.com") ? url : ApiUrlForArtistTracks(url);
                    SynchronizeFromArtistUrl(apiUrl);
                    break;
                case EnumUtil.DownloadMode.Track:
                    Track track = JsonUtil.RetrieveTrackFromUrl(url);
                    SynchronizeSingleTrack(track);
                    break;
                default:
                    _syncUtil.ManifestUtil.ProgressUtil.HasErrors = true;
                    throw new NotImplementedException("Unknown download mode");
            }

            if (_syncUtil.ManifestUtil.FileSystemUtil.ErrorsLogged)
            {
                _syncUtil.ManifestUtil.ProgressUtil.HasErrors = true;
                _syncUtil.ManifestUtil.ProgressUtil.ThrowAllExceptionsWithMessage("Some tracks failed to download.You might need to try a few more times before they can download correctly. " +
                   "The following tracks were not downloaded: ");
            }
        }

        private string ApiUrlForArtistTracks(string url)
        {
            return "https://api.soundcloud.com/users/" +
                _syncUtil.DownloadUtil.ParseUserIdFromProfileUrl(url) + "/tracks";
        }

        private string GetPlaylistId(string url)
        {
            string playlistName;
            try
            {
                playlistName = PlaylistNameFromUrl(url);
            }
            catch (Exception e)
            {
                _syncUtil.ManifestUtil.ProgressUtil.HasErrors = true;
                throw new Exception("Invalid playlist url: " + e.Message);
            }

            var userUrl = "https://api.soundcloud.com/users/" +
                _syncUtil.DownloadUtil.ParseUserIdFromProfileUrl(url) + "/playlists";

            List<Exception> exceptions = new List<Exception>();
            try
            {
                return JsonUtil.RetrievePlaylistId(userUrl, playlistName);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            try
            {
                //try again using a different method for SoundCloud GO users, because at this time
                //the api doesn't function for these users.
                return DownloadUtils.GetPlaylistIdFromHTML(url);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
            return null;
        }

        internal string PlaylistNameFromUrl(string url)
        {
            var startingPoint = "/sets/";
            var startingIndex = url.IndexOf(startingPoint, StringComparison.Ordinal) + startingPoint.Length;
            var endingIndex = url.Substring(startingIndex).Contains("/")
                ? url.Substring(startingIndex).IndexOf("/", StringComparison.Ordinal) + startingIndex
                : url.Length;
            return url.Substring(startingIndex, endingIndex - startingIndex);
            //return url.Substring(url.IndexOf(startingPoint, StringComparison.Ordinal) + startingPoint.Length).Trim('/');
        }

        internal void SynchronizeFromUserPlaylists(string username)
        {
            // hit the /username/playlists endpoint for the username in the url, then get playlists.
            var playlists = JsonUtil.RetrievePlaylistsFromUrl("https://api.soundcloud.com/users/" + username + "/playlists");
            SynchronizeFromPlaylistApiUrls(playlists);
            _syncUtil.ManifestUtil.FileSystemUtil.ResetDirectoryInfo();
        }

        internal void SynchronizeFromProfile(string username)
        {
            // hit the /username/favorites endpoint for the username in the url, then download all the tracks
            var tracks = JsonUtil.RetrieveTracksFromUrl("https://api.soundcloud.com/users/" + username + "/favorites", true);
            _syncUtil.Synchronize(tracks);
        }
        internal void SynchronizeSingleTrack(Track track)
        {
            _syncUtil.ManifestUtil.ProgressUtil.SongsToDownload = 1;
            _syncUtil.FinalizePropertiesForTrack(track);
            _syncUtil.DownloadUtil.DownloadTrackAndTag(ref track);
        }

        internal void SynchronizeFromPlaylistApiUrl(string playlistApiUrl)
        {
            var tracks = JsonUtil.RetrieveTracksFromUrl(playlistApiUrl, false);
            _syncUtil.Synchronize(tracks);
        }

        internal void SynchronizeFromPlaylistApiUrls(IList<PlaylistItem> playlists)
        {
            if(MergePlaylists)
            {
                var tracks = new List<Track>();
                foreach (var playlist in playlists)
                {
                    tracks.AddRange(JsonUtil.RetrieveTracksFromUrl(playlist.uri, false));
                }
                _syncUtil.Synchronize(tracks);
            }
            else
            {
                foreach (var playlist in playlists)
                {
                    var tracks = (JsonUtil.RetrieveTracksFromUrl(playlist.uri, false));
                    _syncUtil.ManifestUtil.FileSystemUtil.ChangeDirectoryInfo(playlist.permalink);
                    _syncUtil.Synchronize(tracks);
                }
            }
        }

        internal void SynchronizeFromArtistUrl(string artistUrl)
        {
            var tracks = JsonUtil.RetrieveTracksFromUrl(artistUrl, true);
            _syncUtil.Synchronize(tracks);
        }
    }
}