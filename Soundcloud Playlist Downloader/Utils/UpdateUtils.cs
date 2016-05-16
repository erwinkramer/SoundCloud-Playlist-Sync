using System;
using System.Deployment.Application;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class UpdateUtils
    {
        public static void InstallUpdateSyncWithInfo()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                MessageBox.Show("IsNetworkDeployed false");
                return;
            }
            UpdateCheckInfo info = null;
            ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
            try
            {
                info = ad.CheckForDetailedUpdate();

            }
            catch (DeploymentDownloadException dde)
            {
                MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                return;
            }
            catch (InvalidDeploymentException ide)
            {
                MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                return;
            }
            catch (InvalidOperationException ioe)
            {
                MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                return;
            }

            if (info.UpdateAvailable)
            {
                Boolean doUpdate = true;

                if (!info.IsUpdateRequired)
                {
                    DialogResult dr = MessageBox.Show("An update is available. Would you like to update the application now?", "Update Available", MessageBoxButtons.OKCancel);
                    if (!(DialogResult.OK == dr))
                    {
                        doUpdate = false;
                    }
                }
                else
                {
                    // Display a message that the app MUST reboot. Display the minimum required version.
                    MessageBox.Show("This application has detected a mandatory update from your current " +
                                    "version to version " + info.MinimumRequiredVersion.ToString() +
                                    ". The application will now install the update and restart.",
                        "Update Available", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                if (doUpdate)
                {
                    try
                    {
                        ad.Update();
                        MessageBox.Show("The application has been upgraded, and will now restart.");
                        Application.Restart();
                    }
                    catch (DeploymentDownloadException dde)
                    {
                        MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("No update available");
            }
        }
    }
}
