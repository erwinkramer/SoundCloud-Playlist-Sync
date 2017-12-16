using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Soundcloud_Playlist_Downloader.JsonObjects;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class JsonUtils
    {
        private ManifestUtils _manifestUtil;
        private string _clientID;

        public JsonUtils(ManifestUtils manifestUtil, string clientID)
        {
            _manifestUtil = manifestUtil;
            _clientID = clientID;

        }

        public string RetrieveJson(string url, int? limit = null, int? offset = null)
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
                        url += (url.Contains("?") ? "&" : "?") + "client_id=" + _clientID;
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
                _manifestUtil.ProgressUtil.IsError = true;
                ExceptionHandlerUtils.HandleException(e);
            }

            return json;
        }

        public string RetrievePlaylistId(string userApiUrl, string playlistName)
        {
            // parse each playlist out, match the name based on the
            // permalink, and return the id of the matching playlist.           
            var playlistsJson = RetrieveJson(userApiUrl);

            var playlists = JArray.Parse(playlistsJson);
            IList<JToken> results = playlists.Children().ToList();
            IList<PlaylistItem> playlistsitems = new List<PlaylistItem>();

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
            _manifestUtil.ProgressUtil.IsError = true;
            throw new Exception("Unable to find a matching playlist");
        }

        public Track RetrieveTrackFromUrl(string url)
        {
            var trackJson = RetrieveJson("https://api.soundcloud.com/resolve.json?url=" + url);
            JObject track = JObject.Parse(trackJson);
            if (track?.GetValue("id") != null)
                return JsonConvert.DeserializeObject<Track>(track.ToString());

            return null;
        }

        public string RetrieveUserIdFromUserName(string username)
        {
            var userJson = RetrieveJson("https://api.soundcloud.com/resolve.json?url=http://soundcloud.com/" + username);
            JObject user = JObject.Parse(userJson);
            if (user == null)
                return null;
            if (user.TryGetValue("id", StringComparison.InvariantCultureIgnoreCase, out JToken userid))
                return (string)JsonConvert.DeserializeObject(userid.ToString(), typeof(string));
            return null;
        }

        public IList<PlaylistItem> RetrievePlaylistsFromUrl(string url)
        {
            // parse each playlist out, match the name based on the
            // permalink, and return the id of the matching playlist.           
            var playlistsJson = RetrieveJson(url);
            var playlists = JArray.Parse(playlistsJson);

            IList<JToken> results = playlists.Children().ToList();
            IList<PlaylistItem> playlistsitems = new List<PlaylistItem>();

            try
            {
                if (playlistsJson != null)
                {
                    foreach (var result in results)
                    {
                        var playlistsitem = JsonConvert.DeserializeObject<PlaylistItem>(result.ToString());
                        playlistsitems.Add(playlistsitem);
                    }
                }
            }
            catch (Exception e)
            {
                _manifestUtil.ProgressUtil.IsError = true;
                throw new Exception("Errors occurred retrieving the playlist list information. Double check your url.", e);
            }
            return playlistsitems;
        }


        public IList<Track> RetrieveTracksFromUrl(string url, bool isRawTracksUrl, bool ignoreSampleSongs)
        {
            var limit = isRawTracksUrl ? 200 : 0; //200 is the limit set by SoundCloud itself. Remember; limits are only with 'collection' types in JSON 
            IList<Track> tracks = new List<Track>();
            int index = 0;
            var lastStep = false;
            try
            {
                var tracksJson = RetrieveJson(url, limit);
                while (tracksJson != null)
                {
                    var JOBtracksJson = JObject.Parse(tracksJson);
                    IList<JToken> JTOKENcurrentTracks = isRawTracksUrl
                        ? JOBtracksJson["collection"].Children().ToList()
                        : JOBtracksJson["tracks"].Children().ToList();

                    foreach (var Jtrack in JTOKENcurrentTracks)
                    {
                        var track = JsonConvert.DeserializeObject<Track>(Jtrack.ToString());
                        if (track.policy == "SNIP") continue;
                        track.IndexFromSoundcloud = index++;
                        tracks.Add(track);
                    }
                    if (lastStep)
                        break;
                    var linkedPartitioningUrl = JsonConvert.DeserializeObject<NextInfo>(tracksJson).next_href;
                    tracksJson = RetrieveJson(linkedPartitioningUrl);
                    if (!string.IsNullOrEmpty(tracksJson)) continue;
                    lastStep = true;
                }
            }
            catch (Exception e)
            {
                _manifestUtil.ProgressUtil.IsError = true;
                throw new Exception("Errors occurred retrieving the tracks list information. Double check your url.", e);
            }
            return tracks;
        }
    }
}