using System;
using Soundcloud_Playlist_Downloader.Utils;

namespace Soundcloud_Playlist_Downloader.JsonObjects
{
    public class Track : SoundcloudBaseTrack
    {
        public string EffectiveDownloadUrl => DownloadUtils.GetEffectiveDownloadUrl(stream_url, download_url, id, downloadable);
        public string LocalPath { get; set; }
        public string LocalPathRelative { get; set; }
        public DateTime DownloadDateTimeUtc { get; set; }
        public DateTime ModifiedDateTimeUtc { get; set; }
        // song is considered HD when there is a download_url available
        public bool IsHD => download_url == EffectiveDownloadUrl;
        public int playback_count { get; set; }
        public int download_count { get; set; }
        public int favoritings_count { get; set; }
        public int comment_count { get; set; }
        public bool IsDownloaded { get; set; } = false;
        public int IndexFromSoundcloud { get; set; }
    }
}
