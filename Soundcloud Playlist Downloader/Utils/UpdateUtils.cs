using Soundcloud_Playlist_Downloader.Language;
using System;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class UpdateUtils
    {
        public enum UpdateCheckStatus {
            NoUpdateAvailable,  OptionalUpdateAvailable, MandatoryUpdateAvailable, IsNotNetworkDeployed, InError };

        public Exception InErrorException;
        public UpdateCheckStatus CurrentStatus;

        public UpdateUtils()
        {
            CheckForUpdates();
        }
        public void CheckForUpdates()
        {        
            CurrentStatus = UpdateCheckStatus.IsNotNetworkDeployed;
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
            Application.Restart();       
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
