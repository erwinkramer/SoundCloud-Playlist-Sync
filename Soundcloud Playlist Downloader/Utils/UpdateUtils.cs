using Newtonsoft.Json.Linq;
using SC_SYNC_Base.JsonObjects;
using Soundcloud_Playlist_Downloader.Language;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class UpdateUtils
    {
        public enum UpdateCheckStatus {
            NoUpdateAvailable,  OptionalUpdateAvailable, MandatoryUpdateAvailable, IsNotNetworkDeployed, InError };

        public Exception InErrorException;
        public UpdateCheckStatus CurrentStatus;
        public string rootReleaseUrl = "https://raw.githubusercontent.com/erwinkramer/SoundCloud-Playlist-Sync/fix/Soundcloud%20Playlist%20Downloader/Releases/";
        public string ReleaseUrlBlob = "https://github.com/erwinkramer/SoundCloud-Playlist-Sync/blob/fix/Soundcloud%20Playlist%20Downloader/Releases/Soundcloud%20Playlist%20Downloader.exe?raw=true";
        public UpdateUtils()
        {
            CheckForUpdates();
        }
        public void CheckForUpdates()
        {       
            try
            {
                if (GetOnlineVersion() > GetCurrentVersion())
                {
                    CurrentStatus = UpdateCheckStatus.MandatoryUpdateAvailable;
                }
                else
                {
                    CurrentStatus = UpdateCheckStatus.NoUpdateAvailable;
                }
            }
            catch
            {
                CurrentStatus = UpdateCheckStatus.InError;
            }
        }

        public int GetOnlineVersion()
        {
            string json = string.Empty;
            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                json = client.DownloadString($"{rootReleaseUrl}ReleaseInfo.json");
            }
            return JObject.Parse(json)["AssemblyMajorVersion"].Value<int>();
        }

        public static int GetCurrentVersion()
        {
            return typeof(UpdateUtils).Assembly.GetName().Version.Major;
        }

        public string LabelTextForCurrentStatus()
        {
            switch (CurrentStatus)
            {
                case UpdateCheckStatus.OptionalUpdateAvailable:
                case UpdateCheckStatus.MandatoryUpdateAvailable:
                    return " [!]";
                case UpdateCheckStatus.NoUpdateAvailable:
                    return " [✓]";
                case UpdateCheckStatus.IsNotNetworkDeployed:
                    return " [~]";
                case UpdateCheckStatus.InError:
                    return " [X]";
                default:
                    return "";
            }
        }

        internal void Update()
        {
            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                client.DownloadFile(ReleaseUrlBlob, "Soundcloud Playlist Downloader.exe.new");
            }
            CompleteUpdate_part1();
        }

        public void CompleteUpdate_part1()
        {
            SyncSetting.settings.Set("Updating", "True");
            System.IO.File.Copy("Soundcloud Playlist Downloader.exe", "Soundcloud Playlist Downloader.exe.old", true);
            Process.Start("Soundcloud Playlist Downloader.exe.new");
            Application.Exit();
        }

        public static void CompleteUpdate_part2()
        {
            if (SyncSetting.settings.Get("Updating") ==  "False")
                return;
            System.IO.File.Copy("Soundcloud Playlist Downloader.exe.new", "Soundcloud Playlist Downloader.exe", true);
            SyncSetting.settings.Set("Updating", "False");
            Process.Start("Soundcloud Playlist Downloader.exe");
            Application.Exit();
        }

        public void InstallUpdateSyncWithInfo()
        {
            CheckForUpdates();
            switch (CurrentStatus)
            {
                case UpdateCheckStatus.OptionalUpdateAvailable:
                case UpdateCheckStatus.MandatoryUpdateAvailable:
                    {
                        DialogResult dr = MessageBox.Show(LanguageManager.Language["STR_UPDATE_AVAILABLE_TEXT"], LanguageManager.Language["STR_UPDATE_AVAILABLE_TITLE"], MessageBoxButtons.OKCancel);
                        if ((DialogResult.OK == dr))
                        {
                            try
                            {
                                Update();
                            }
                            catch (Exception dde)
                            {
                                MessageBox.Show(LanguageManager.Language["STR_UPDATE_ERROR_TEXT"].Replace("\\n", "\n") + ": " + dde, LanguageManager.Language["STR_UPDATE_ERROR_TITLE"]);
                                return;
                            }
                        }
                        break;
                    }
                case UpdateCheckStatus.NoUpdateAvailable:
                case UpdateCheckStatus.IsNotNetworkDeployed:
                    {
                        MessageBox.Show(LanguageManager.Language["STR_UPDATE_NO_TEXT"], LanguageManager.Language["STR_UPDATE_NO_TITLE"]);
                        break;
                    }
                case UpdateCheckStatus.InError:
                    {
                        MessageBox.Show(LanguageManager.Language["STR_UPDATE_ERROR1_TEXT"] + ":" + InErrorException.Message, LanguageManager.Language["STR_UPDATE_ERROR1_TITLE"]);
                        break;
                    }
                default:
                    break;
            }        
        }
    }
}
