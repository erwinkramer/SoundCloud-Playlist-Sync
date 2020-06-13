using Newtonsoft.Json.Linq;
using SC_SYNC_Base.JsonObjects;
using Soundcloud_Playlist_Downloader.Language;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class UpdateUtils
    {
        public enum UpdateCheckStatus {
            NoUpdateAvailable,  OptionalUpdateAvailable, MandatoryUpdateAvailable, IsNotNetworkDeployed, InError };

        public Exception InErrorException;
        public UpdateCheckStatus CurrentStatus;
        public string RootReleaseUrl = "https://raw.githubusercontent.com/erwinkramer/SoundCloud-Playlist-Sync/fix/Soundcloud%20Playlist%20Downloader/Releases/";
        public string ReleaseUrlBlob = "https://github.com/erwinkramer/SoundCloud-Playlist-Sync/blob/fix/Soundcloud%20Playlist%20Downloader/Releases/SoundcloudPlaylistDownloader.zip?raw=true";
        public static string ExecutableName = "SoundcloudPlaylistDownloader.exe";
        public static string ArchiveName = "SoundcloudPlaylistDownloader.zip";


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
            json = DownloadUtils.httpClient.GetStringAsync($"{RootReleaseUrl}ReleaseInfo.json").Result;
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
                    return " [x]";
                default:
                    return "";
            }
        }

        internal void DownloadUpdate()
        {
            using (var download = DownloadUtils.httpClient.GetAsync(ReleaseUrlBlob).Result) 
            using (var fs = new FileStream(ArchiveName, FileMode.Create))
            {
                download.Content.CopyToAsync(fs).GetAwaiter().GetResult();
            }

            var zipOutputFolder = Path.Combine(Directory.GetCurrentDirectory(), "new");
            ZipFile.ExtractToDirectory(ArchiveName, zipOutputFolder, true);
            File.Move(Path.Combine(zipOutputFolder, ExecutableName) , $"{ExecutableName}.new", true);
            File.Delete(ArchiveName);
            Directory.Delete(zipOutputFolder);
        }

        /// <summary>
        /// Restart as .old so we can rename .new as current version 
        /// </summary>
        public void CompleteUpdate_part1()
        {
            SyncSetting.settings.Set("Updating", "True");
            System.IO.File.Copy(ExecutableName, $"{ExecutableName}.old", true);
            Process.Start($"{ExecutableName}.old");
            Application.Exit();
        }

        public static void CompleteUpdate_part2()
        {
            SyncSetting.settings.Set("Updating", "False");
            System.IO.File.Copy($"{ExecutableName}.new", ExecutableName, true);
            Process.Start(ExecutableName);
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
                                DownloadUpdate();
                                CompleteUpdate_part1();
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
