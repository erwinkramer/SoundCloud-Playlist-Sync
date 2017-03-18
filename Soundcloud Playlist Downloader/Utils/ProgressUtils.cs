using System.Collections.Generic;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class ProgressUtils
    {
        public bool IsActive { get; set; }
        public int SongsToDownload { get; set; }
        public int SongsDownloaded;
        public bool Completed { get; set; }
        public bool Exiting { get; set; }
        public bool IsError { get; set; }
        public ICollection<string> TrackProgress;

        public ProgressUtils()
        {
            Completed = false;
            Exiting = false;
            IsError = false;
            IsActive = false;
            SongsDownloaded = 0;
            SongsToDownload = 0;
            TrackProgress = new System.Collections.Generic.List<string>();
        }

        public void ResetProgress()
        {
            SongsDownloaded = 0;
            SongsToDownload = 0;
            IsActive = true;
            IsError = false;
        }
    }

   
}
