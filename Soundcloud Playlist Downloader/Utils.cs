namespace Soundcloud_Playlist_Downloader
{
    public class Utils
    {
        public static string ParseTrackPath(string csv, int position)
        {
            if (csv != null && csv.IndexOf(',') >= 0)
            {
                //only make 1 split, as a comma (,) can be found in a song name!
                return csv.Split(new[] {','}, 2)[position]; //position 0 is streampath, position 1 is local path
            }
            return csv;
        }
    }
}