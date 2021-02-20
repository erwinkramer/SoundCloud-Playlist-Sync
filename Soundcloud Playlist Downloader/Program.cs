using System;
using System.Windows.Forms;
using SC_SYNC_Base.JsonObjects;
using Soundcloud_Playlist_Downloader.Utils;
using Soundcloud_Playlist_Downloader.Views;

namespace Soundcloud_Playlist_Downloader
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if(SyncSetting.settings.Get("Updating") == "True")
                UpdateUtils.CompleteUpdate_part2();
            else
            {
                //Application.EnableVisualStyles(); //issue with loading: https://github.com/dotnet/winforms/issues/4145#issuecomment-714028387
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SoundcloudSyncMainForm());
            }
        }
    }
}