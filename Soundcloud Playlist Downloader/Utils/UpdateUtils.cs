using System;
using System.Deployment.Application;
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
            InErrorException = null;
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                CurrentStatus = UpdateCheckStatus.IsNotNetworkDeployed;
                return;
            }
            try
            {
                UpdateCheckInfo info = ApplicationDeployment.CurrentDeployment.CheckForDetailedUpdate();
                if (!info.UpdateAvailable)
                {
                    CurrentStatus = UpdateCheckStatus.NoUpdateAvailable;
                    return;
                }

                if (info.IsUpdateRequired)
                {
                    CurrentStatus = UpdateCheckStatus.MandatoryUpdateAvailable;
                    return;
                }

                CurrentStatus = UpdateCheckStatus.OptionalUpdateAvailable;
            }
            catch(Exception e)
            {
                CurrentStatus = UpdateCheckStatus.InError;
                InErrorException = e;
            }
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
            ApplicationDeployment.CurrentDeployment.Update();
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
                        DialogResult dr = MessageBox.Show("An update is available. Would you like to update the application now?", "Update Available", MessageBoxButtons.OKCancel);
                        if ((DialogResult.OK == dr))
                        {
                            try
                            {
                                Update();
                            }
                            catch (Exception dde)
                            {
                                MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                                return;
                            }
                        }
                        break;
                    }
                case UpdateCheckStatus.NoUpdateAvailable:
                case UpdateCheckStatus.IsNotNetworkDeployed:
                    {
                        MessageBox.Show("No update available");
                        break;
                    }
                case UpdateCheckStatus.InError:
                    {
                        MessageBox.Show("Exception while checking for updates available. Exception thrown:" + InErrorException.Message);
                        break;
                    }
                default:
                    break;
            }        
        }
    }
}
