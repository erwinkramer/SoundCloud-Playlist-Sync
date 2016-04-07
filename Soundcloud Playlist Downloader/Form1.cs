using System;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using Soundcloud_Playlist_Downloader.Properties;

namespace Soundcloud_Playlist_Downloader
{
    public partial class Form1 : Form
    {
        public static bool Highqualitysong;
        public static bool ConvertToMp3;
        public static bool IncludeArtistInFilename;
        public static int SyncMethod = 1;

        public static bool FoldersPerArtist;
        public static bool ReplaceIllegalCharacters;
        public static bool excludeM4A;
        public static bool excludeAAC;
        public static string ManifestName = "";
        private readonly string AbortActionText = "Abort";
        private readonly BoxAbout aboutWindow = new BoxAbout();

        //private string CLIENT_ID = "93a4fae1bd98b84c9b4f6bf1cc838b4f";
        //new key should fix same reason as stated here: 
        //https://stackoverflow.com/questions/29914622/get-http-mp3-stream-from-every-song/30018216#30018216
        private readonly string CLIENT_ID = "376f225bf427445fc4bfb6b99b72e0bf";

        private bool completed;

        private readonly string DefaultActionText = "Synchronize";

        private bool exiting;
        private readonly PerformStatusUpdate PerformStatusUpdateImplementation;

        private readonly PerformSyncComplete PerformSyncCompleteImplementation;
        private readonly ProgressBarUpdate ProgressBarUpdateImplementation;

        private readonly PlaylistSync _sync;

        public Form1()
        {
            InitializeComponent();         
            Text = $"SoundCloud Playlist Sync {Version()} Stable";
            _sync = new PlaylistSync();
            PerformSyncCompleteImplementation = SyncCompleteButton;
            ProgressBarUpdateImplementation = UpdateProgressBar;
            PerformStatusUpdateImplementation = UpdateStatus;
            status.Text = "Ready";
            MinimumSize = new Size(Width, Height);
            MaximumSize = new Size(Width, Height);
        }

        private static string Version()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return $"Your application name - v{ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4)}";
            }
            else
            {
                return "_";
            }
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                directoryPath.Text = dialog.SelectedPath;
            }
        }

        [SilentFailure]
        private void UpdateStatus()
        {
            if (!exiting)
            {
                if (_sync.IsActive && progressBar.Value == progressBar.Maximum &&
                    progressBar.Value != progressBar.Minimum)
                {
                    status.Text = "Completed";
                }
                else if (_sync.IsActive && progressBar.Value >= progressBar.Minimum && progressBar.Maximum > 0)
                {
                    status.Text = "Synchronizing... " + progressBar.Value + " of " + progressBar.Maximum +
                                  " songs downloaded.";
                }
                else if (_sync.IsActive && completed && !_sync.IsError)
                {
                    status.Text = "Tracks are already synchronized";
                }
                else if (_sync.IsActive && completed && _sync.IsError)
                {
                    status.Text = "An error prevented synchronization from starting";
                }
                else if (!_sync.IsActive && syncButton.Text == AbortActionText)
                {
                    status.Text = "Aborting downloads... Please Wait.";
                }
                else if (_sync.IsActive)
                {
                    status.Text = "Enumerating tracks to download...";
                }
                else if (!_sync.IsActive)
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
            progressBar.Maximum = _sync.SongsToDownload;
            progressBar.Value = _sync.SongsDownloaded;

            TaskbarManager.Instance.SetProgressValue(_sync.SongsDownloaded, _sync.SongsToDownload);
            if (progressBar.Minimum != 0 && progressBar.Maximum == progressBar.Value)
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

                Highqualitysong = chk_highquality.Checked;
                ConvertToMp3 = chk_convertToMp3.Checked;
                IncludeArtistInFilename = chk_includeArtistinFilename.Checked;
                SyncMethod = rbttn_oneWay.Checked ? 1 : 2;
                FoldersPerArtist = chk_folderByArtist.Checked;
                ReplaceIllegalCharacters = chk_replaceIllegalCharacters.Checked;
                excludeAAC = chk_excl_m4a.Checked;
                excludeM4A = chk_excl_m4a.Checked;

                var dlMode = playlistRadio.Checked
                    ? PlaylistSync.DownloadMode.Playlist
                    : favoritesRadio.Checked ? PlaylistSync.DownloadMode.Favorites : PlaylistSync.DownloadMode.Artist;

                var uri = new Uri(url.Text);
                var uriWithoutScheme = uri.Host + uri.PathAndQuery;
                var validManifestFilename = Track.StaticCoerceValidFileName(uriWithoutScheme, false);
                ManifestName = ".MNFST=" + validManifestFilename + ",FPA=" + FoldersPerArtist + ",IAIF=" +
                               IncludeArtistInFilename + ",DM=" + dlMode + ",SM=" + SyncMethod + ".csv";

                if (Directory.Exists(directoryPath.Text))
                {
                    var files = Directory.GetFiles(directoryPath.Text, ".MNFST=*", SearchOption.TopDirectoryOnly);
                    if ((files.Length > 0) || File.Exists(Path.Combine(directoryPath.Text, "manifest")))
                    {
                        if (!File.Exists(Path.Combine(directoryPath.Text, ManifestName)) ||
                            File.Exists(Path.Combine(directoryPath.Text, "manifest"))) //old manifest format
                        {
                            //different or old manifest found, quitting
                            status.Text = "Old or different manifest found, please change settings or local directoy.";

                            completed = true;
                            InvokeSyncComplete();
                            return;
                        }
                        if (File.Exists(Path.Combine(directoryPath.Text, ManifestName)))
                        {
                            //copy to backup location
                            var destinationPath =
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    "SoundCloud Playlist Sync",
                                    DateTime.Today.ToString("dd/MM/yyyy") + " Manifest Backups");

                            var destinationPathWithFile = Path.Combine(destinationPath, ManifestName);
                            Directory.CreateDirectory(destinationPath);

                            File.Copy(Path.Combine(directoryPath.Text, ManifestName), destinationPathWithFile, true);
                        }
                    }
                }

                new Thread(() =>
                {
                    try
                    {
                        _sync.Synchronize(url.Text, dlMode, directoryPath.Text, CLIENT_ID
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
            else if (_sync.IsActive && syncButton.Text == AbortActionText)
            {
                _sync.IsActive = false;
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
            Settings.Default.PlaylistUrl = url.Text;
            Settings.Default.LocalPath = directoryPath.Text;
            Settings.Default.Save();
            exiting = true;
            _sync.IsActive = false;
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
            url.Text = Settings.Default.PlaylistUrl;
            directoryPath.Text = Settings.Default.LocalPath;
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
            if (chk_convertToMp3.Checked)
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
            if (chk_highquality.Checked)
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

        private delegate void ProgressBarUpdate();

        private delegate void PerformSyncComplete();

        private delegate void PerformStatusUpdate();
    }
}