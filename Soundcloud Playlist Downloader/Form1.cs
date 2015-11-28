using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Soundcloud_Playlist_Downloader.Properties;
using System.IO;
using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Soundcloud_Playlist_Downloader
{
    public partial class Form1 : Form
    {

        //private string CLIENT_ID = "93a4fae1bd98b84c9b4f6bf1cc838b4f";
        //new key should fix same reason as stated here: 
        //https://stackoverflow.com/questions/29914622/get-http-mp3-stream-from-every-song/30018216#30018216
        private string CLIENT_ID = "376f225bf427445fc4bfb6b99b72e0bf";
        private box_about aboutWindow = new box_about();

        private PlaylistSync sync = null;
        private delegate void ProgressBarUpdate();
        private delegate void PerformSyncComplete();
        private delegate void PerformStatusUpdate();

        private bool completed = false;
        public static bool Highqualitysong = false;
        public static bool ConvertToMp3 = false;
        public static bool IncludeArtistInFilename = false;
        public static int SyncMethod = 1;

        public static bool FoldersPerArtist = false;
        public static bool ReplaceIllegalCharacters = false;
        public static bool excludeM4A = false;
        public static bool excludeAAC = false;
        public static string ManifestName = "";

        private PerformSyncComplete PerformSyncCompleteImplementation = null;
        private ProgressBarUpdate ProgressBarUpdateImplementation = null;
        private PerformStatusUpdate PerformStatusUpdateImplementation = null;

        private string DefaultActionText = "Synchronize";
        private string AbortActionText = "Abort";

        private bool exiting = false;

        public Form1()
        {
            InitializeComponent();
            sync = new PlaylistSync();
            PerformSyncCompleteImplementation = SyncCompleteButton;
            ProgressBarUpdateImplementation = UpdateProgressBar;
            PerformStatusUpdateImplementation = UpdateStatus;
            status.Text = "Ready";
            MinimumSize = new Size(Width, Height);
            MaximumSize = new Size(Width, Height);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                directoryPath.Text = dialog.SelectedPath;
            }
        }

        [SilentFailure]
        private void UpdateStatus()
        {
            if (!exiting)
            {
                if (sync.IsActive && progressBar.Value == progressBar.Maximum && progressBar.Value != progressBar.Minimum)
                {
                    status.Text = "Completed";
                }
                else if (sync.IsActive && progressBar.Value >= progressBar.Minimum && progressBar.Maximum > 0)
                {
                    status.Text = "Synchronizing... " + progressBar.Value + " of " + progressBar.Maximum + " songs downloaded.";
                }
                else if (sync.IsActive && completed && !sync.IsError)
                {
                    status.Text = "Tracks are already synchronized";
                }
                else if (sync.IsActive && completed && sync.IsError)
                {
                    status.Text = "An error prevented synchronization from starting";
                }
                else if (!sync.IsActive && syncButton.Text == AbortActionText)
                {
                    status.Text = "Aborting downloads... Please Wait.";
                }
                else if (sync.IsActive)
                {
                    status.Text = "Enumerating tracks to download...";
                }
                else if (!sync.IsActive)
                {
                    status.Text = "Aborted";
                }
            }
            else if (completed)
            {
                // the form has indicated it is being closed and the sync utility has finished aborting
                Close();
                Dispose();
            }
            
        }

        [SilentFailure]
        private void InvokeUpdateStatus()
        {
            statusStrip1.Invoke(PerformStatusUpdateImplementation);
        }

        [SilentFailure]
        private void UpdateProgressBar()
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = sync.SongsToDownload;
            progressBar.Value = sync.SongsDownloaded;

            TaskbarManager.Instance.SetProgressValue(sync.SongsDownloaded, sync.SongsToDownload);
            if(progressBar.Minimum != 0 && progressBar.Maximum == progressBar.Value)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            }
        }

        private void InvokeUpdateProgressBar()
        {
            progressBar.Invoke(ProgressBarUpdateImplementation);
        }

        private void InvokeSyncComplete()
        {
            syncButton.Invoke(PerformSyncCompleteImplementation);
        }

        [SilentFailure]
        private void SyncCompleteButton()
        {
            syncButton.Text = DefaultActionText;
            syncButton.Enabled = true;
            if (exiting)
            {
                Dispose();
            }
        }

        private void syncButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(url.Text) &&
                !string.IsNullOrWhiteSpace(directoryPath.Text) &&
                syncButton.Text == DefaultActionText)
            {
                syncButton.Text = AbortActionText;
                status.Text = "Checking for track changes...";
                completed = false;

                progressBar.Value = 0;
                progressBar.Maximum = 0;
                progressBar.Minimum = 0;
               
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);

                Form1.Highqualitysong = chk_highquality.Checked;
                Form1.ConvertToMp3 = chk_convertToMp3.Checked;
                Form1.IncludeArtistInFilename = chk_includeArtistinFilename.Checked;
                Form1.SyncMethod = rbttn_oneWay.Checked ? 1 : 2;
                Form1.FoldersPerArtist = chk_folderByArtist.Checked;
                Form1.ReplaceIllegalCharacters = chk_replaceIllegalCharacters.Checked;
                Form1.excludeAAC = chk_excl_m4a.Checked;
                Form1.excludeM4A = chk_excl_m4a.Checked;
            
                PlaylistSync.DownloadMode dlMode = playlistRadio.Checked ? PlaylistSync.DownloadMode.Playlist : favoritesRadio.Checked ? PlaylistSync.DownloadMode.Favorites : PlaylistSync.DownloadMode.Artist;

                System.Uri uri = new Uri(url.Text);
                string uriWithoutScheme = uri.Host + uri.PathAndQuery;
                string validManifestFilename = JsonPoco.Track.staticCoerceValidFileName(uriWithoutScheme, false);
                Form1.ManifestName = ".MNFST=" + validManifestFilename + ",FPA=" + FoldersPerArtist + ",IAIF=" + IncludeArtistInFilename + ",DM=" + dlMode + ",SM=" + SyncMethod +".csv";

                if (Directory.Exists(directoryPath.Text))
                {
                    string[] files = System.IO.Directory.GetFiles(directoryPath.Text, ".MNFST=*", System.IO.SearchOption.TopDirectoryOnly);
                    if ((files.Length > 0) || (System.IO.File.Exists(Path.Combine(directoryPath.Text, "manifest"))))
                    {
                        if (!System.IO.File.Exists(Path.Combine(directoryPath.Text, Form1.ManifestName)) ||
                            (System.IO.File.Exists(Path.Combine(directoryPath.Text, "manifest")))) //old manifest format
                        {
                            //different or old manifest found, quitting
                            status.Text = "Old or different manifest found, please change settings or local directoy.";

                            completed = true;
                            InvokeSyncComplete();
                            return;
                        }
                        else if (System.IO.File.Exists(Path.Combine(directoryPath.Text, Form1.ManifestName)))
                        {
                            //copy to backup location
                            string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                "SoundCloud Playlist Sync",
                                DateTime.Today.ToString("dd/MM/yyyy") + " Manifest Backups");

                            string destinationPathWithFile = Path.Combine(destinationPath, Form1.ManifestName);
                            Directory.CreateDirectory(destinationPath);

                            File.Copy((Path.Combine(directoryPath.Text, Form1.ManifestName)), destinationPathWithFile, true);
                        }
                    }
                }

                new Thread(() =>
                {
                    try
                    {
                        sync.Synchronize(
                            url: url.Text,
                            mode: dlMode,
                            directory: directoryPath.Text, 
                            clientId: CLIENT_ID
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                    finally
                    {
                        completed = true;
                        InvokeSyncComplete();
                    }
                }).Start();

                new Thread(() =>
                {
                    // perform progress updates
                    while (!completed && !exiting)
                    {
                        Thread.Sleep(500);
                        InvokeUpdateStatus();
                        InvokeUpdateProgressBar();
                    }
                    if (!exiting)
                    {
                        InvokeUpdateStatus();
                    }

                }).Start();

            }
            else if (sync.IsActive && syncButton.Text == AbortActionText)
            {
                sync.IsActive = false;
                syncButton.Enabled = false;
            }
            else if (syncButton.Text == DefaultActionText && 
                string.IsNullOrWhiteSpace(url.Text))
            {
                status.Text = "Enter the download url";
            }
            else if (syncButton.Text == DefaultActionText &&
                string.IsNullOrWhiteSpace(directoryPath.Text))
            {
                status.Text = "Enter local directory path";
            }
        }

        [SilentFailure]
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Soundcloud_Playlist_Downloader.Properties.Settings.Default.PlaylistUrl = this.url.Text;
            Soundcloud_Playlist_Downloader.Properties.Settings.Default.LocalPath = this.directoryPath.Text;
            Soundcloud_Playlist_Downloader.Properties.Settings.Default.Save();
            exiting = true;
            sync.IsActive = false;
            status.Text = "Preparing for exit... Please Wait.";
            syncButton.Enabled = false;

            if (syncButton.Text != DefaultActionText)
            {
                e.Cancel = true;
            }
            else
            {
                syncButton.Text = AbortActionText;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            url.Text = Soundcloud_Playlist_Downloader.Properties.Settings.Default.PlaylistUrl;
            directoryPath.Text = Soundcloud_Playlist_Downloader.Properties.Settings.Default.LocalPath;
        }

        private void chk_folderByArtist_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
       
        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (aboutWindow.Visible)
            {
                aboutWindow.Focus();
            }
            else
            {
                aboutWindow.Show();
            }
        }

        private void chk_convertToMp3_CheckedChanged(object sender, EventArgs e)
        {
            if(chk_convertToMp3.Checked)
            {
                chk_excl_m4a.Visible = true;
                chk_exl_aac.Visible = true;
                lbl_exclude.Visible = true;
            }
            else
            {
                chk_excl_m4a.Visible = false;
                chk_excl_m4a.Checked = false;
                chk_exl_aac.Visible = false;
                chk_exl_aac.Checked = false;
                lbl_exclude.Visible = false;
            }
        }

        private void chk_highquality_CheckedChanged(object sender, EventArgs e)
        {
            if(chk_highquality.Checked)
            {
                chk_convertToMp3.Enabled = true;
                chk_convertToMp3.Checked = true;
                pnl_convert.Visible = true;
            }
            else
            {
                chk_convertToMp3.Enabled = false;
                chk_convertToMp3.Checked = false;
                pnl_convert.Visible = false;
            }
        }

        private void chk_deleteRemovedOrAlteredSongs_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbox_syncMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }

        private void url_TextChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void progressBar_Click(object sender, EventArgs e)
        {

        }
    }
}
