using System;
using System.IO;
using Newtonsoft.Json;
using Soundcloud_Playlist_Downloader.Utils;

namespace Soundcloud_Playlist_Downloader.JsonObjects
{
    public class Track : SoundcloudBaseTrack
    {    
        public string EffectiveDownloadUrl { get; set; }
        public string LocalPath { get; set; }
        public string LocalPathRelative { get; set; }
        public DateTime DownloadDateTimeUtc { get; set; }
        public DateTime ModifiedDateTimeUtc { get; set; }
        // song is considered HD when there is a download_url available
        public bool IsHD { get; set; }
        public int playback_count { get; set; }
        public int download_count { get; set; }
        public int favoritings_count { get; set; }
        public int comment_count { get; set; }
        public bool IsDownloaded { get; set; } = false;
        public int IndexFromSoundcloud { get; set; }

        public override string ToString()
        {
            using (var sw = new StringWriter())
            {
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;
                    var serializer = new JsonSerializer();
                    serializer.Serialize(jw, this);
                }
                return sw.ToString();
            }
        }
    }
}
