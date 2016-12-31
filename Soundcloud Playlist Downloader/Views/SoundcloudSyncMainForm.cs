using System;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using Soundcloud_Playlist_Downloader.Properties;
using Soundcloud_Playlist_Downloader.Utils;
using System.Collections;
using System.Collections.Generic;

namespace Soundcloud_Playlist_Downloader.Views
{
    public partial class SoundcloudSyncMainForm : Form
    {
        public static bool Highqualitysong;
        public static bool ConvertToMp3;
        public static bool IncludeArtistInFilename;
        public static int SyncMethod = 1;
        private static EnumUtil.DownloadMode _dlMode;
        public static bool FoldersPerArtist;
        public static bool ReplaceIllegalCharacters;
        public static bool ExcludeM4A;
        public static bool ExcludeAac;
        public static ICollection<string> trackProgress;

        private readonly string AbortActionText = "Abort";
        private readonly BoxAbout _aboutWindow = new BoxAbout();
        private bool _completed;

        private readonly string DefaultActionText = "Synchronize";
        private bool _exiting;
        private readonly PerformStatusUpdate _performStatusUpdateImplementation;

        private readonly PerformSyncComplete _performSyncCompleteImplementation;
        private readonly ProgressBarUpdate _progressBarUpdateImplementation;
        private readonly SoundcloudSync _sync;

        public SoundcloudSyncMainForm()
        {
            InitializeComponent();         
            Text = $"SoundCloud Playlist Sync {Version()} Stable";
            _sync = new SoundcloudSync();
            _performSyncCompleteImplementation = SyncCompleteButton;
            _progressBarUpdateImplementation = UpdateProgressBar;
            _performStatusUpdateImplementation = UpdateStatus;
            trackProgress = new System.Collections.Generic.List<string>();
            status.Text = @"Ready";
            MinimumSize = new Size(Width, Height);
            MaximumSize = new Size(Width, Height);
        }     

        private static string Version()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4);
            }
            return "_";
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
            if (!_exiting)
            {
                if (DownloadUtils.IsActive && progressBar.Value == progressBar.Maximum &&
                    progressBar.Value != progressBar.Minimum)
                {
                    status.Text = @"Completed";
                }
                else if (DownloadUtils.IsActive && progressBar.Value >= progressBar.Minimum && progressBar.Maximum > 0)
                {
                    status.Text = $"Synchronizing... {progressBar.Value} of {progressBar.Maximum} songs downloaded.";
                }
                else if (DownloadUtils.IsActive && _completed && !SoundcloudSync.IsError)
                {
                    status.Text = @"Tracks are already synchronized";
                }
                else if (DownloadUtils.IsActive && _completed && SoundcloudSync.IsError)
                {
                    status.Text = @"An error prevented synchronization from starting";
                }
                else if (!DownloadUtils.IsActive && syncButton.Text == AbortActionText)
                {
                    status.Text = @"Aborting downloads... Please Wait.";
                }
                else if (DownloadUtils.IsActive)
                {
                    var plural = "";
                    if (_dlMode != EnumUtil.DownloadMode.Track)
                        plural = "s";              
                    status.Text = $"Fetching track{plural} to download...";
                }
                else if (!DownloadUtils.IsActive)
                {
                    status.Text = @"Aborted";
                }
            }
            else if (_completed)
            {
                // the form has indicated it is being closed and the sync utility has finished aborting
                Close();
                Dispose();
            }
        }

        [SilentFailure]
        private void InvokeUpdateStatus()
        {
            statusStrip1.Invoke(_performStatusUpdateImplementation);
        }

        [SilentFailure]
        private void UpdateProgressBar()
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = DownloadUtils.SongsToDownload;

            progressBar.Value = DownloadUtils.SongsDownloaded;

            TaskbarManager.Instance.SetProgressValue(DownloadUtils.SongsDownloaded, DownloadUtils.SongsToDownload);
            if (progressBar.Minimum != 0 && progressBar.Maximum == progressBar.Value)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            }
        }

        private void InvokeUpdateProgressBar()
        {
            progressBar.Invoke(_progressBarUpdateImplementation);
        }

        private void InvokeSyncComplete()
        {
            syncButton.Invoke(_performSyncCompleteImplementation);
        }

        [SilentFailure]
        private void SyncCompleteButton()
        {
            syncButton.Text = DefaultActionText;
            syncButton.Enabled = true;
            if (_exiting)
            {
                Dispose();
            }
        }

        private void syncButton_Click(object sender, EventArgs e)
        {
            Settings.Default.LocalPath = directoryPath.Text;
            Settings.Default.PlaylistUrl = url.Text;
            Settings.Default.ConcurrentDownloads = (int) nudConcurrency.Value;
            Settings.Default.favoritesRadio = favoritesRadio.Checked;
            Settings.Default.PlaylistRadio = playlistRadio.Checked;
            Settings.Default.artistRadio = artistRadio.Checked;
            Settings.Default.trackRadio = trackRadio.Checked;
            Settings.Default.chk_convertToMp3 = chk_convertToMp3.Checked;
            Settings.Default.chk_excl_m4a = chk_excl_m4a.Checked;
            Settings.Default.chk_exl_aac = chk_exl_aac.Checked;
            Settings.Default.chk_folderByArtist = chk_folderByArtist.Checked;
            Settings.Default.chk_highquality = chk_highquality.Checked;
            Settings.Default.chk_includeArtistinFilename = chk_includeArtistinFilename.Checked;
            Settings.Default.chk_replaceIllegalCharacters = chk_replaceIllegalCharacters.Checked;
            Settings.Default.rbttn_oneWay = rbttn_oneWay.Checked;
            Settings.Default.rbttn_twoWay = rbttn_twoWay.Checked;
            Settings.Default.Save();

            _dlMode = playlistRadio.Checked
                ? EnumUtil.DownloadMode.Playlist
                : favoritesRadio.Checked ? EnumUtil.DownloadMode.Favorites
                    : artistRadio.Checked ? EnumUtil.DownloadMode.Artist : EnumUtil.DownloadMode.Track;
            if (!string.IsNullOrWhiteSpace(url.Text) &&
                !string.IsNullOrWhiteSpace(directoryPath.Text) &&
                syncButton.Text == DefaultActionText)
            {
                syncButton.Text = AbortActionText;
                status.Text = @"Checking for track changes...";
                _completed = false;

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
                ExcludeAac = chk_excl_m4a.Checked;
                ExcludeM4A = chk_excl_m4a.Checked;
                FilesystemUtils.Directory = new DirectoryInfo(directoryPath.Text);
                Uri uri;
                try
                {
                    uri = new Uri(url.Text);
                }
                catch (Exception)
                {
                    status.Text = @"Invalid URL";
                    _completed = true;
                    InvokeSyncComplete();
                    return;
                }
                ManifestUtils.ManifestName = ManifestUtils.MakeManifestString(
                    FilesystemUtils.CoerceValidFileName(uri.Host + uri.PathAndQuery, false), FoldersPerArtist,
                    IncludeArtistInFilename, _dlMode, SyncMethod);

                if (_dlMode != EnumUtil.DownloadMode.Track)
                {
                    bool differentmanifest;
                    if (!ManifestUtils.FindManifestAndBackup(ManifestUtils.ManifestName, out differentmanifest))
                    {
                        if (differentmanifest)
                        {
                            status.Text = @"Change settings or directoy.";
                            _completed = true;
                            InvokeSyncComplete();
                            return;
                        }
                    }
                }
                new Thread(() =>
                {
                    // perform progress updates
                    while (!_completed && !_exiting)
                    {
                        Thread.Sleep(500);
                        InvokeUpdateStatus();

                        this.Invoke((MethodInvoker)(() => lb_progressOfTracks.DataSource = trackProgress ));
                        this.Invoke((MethodInvoker)(() => lb_progressOfTracks.Refresh() ) );
                   
                        InvokeUpdateProgressBar();
                    }
                    if (!_exiting)
                    {
                        InvokeUpdateStatus();
                    }
                }).Start();

                new Thread(() =>
                {
                    try
                    {
                        _sync.Synchronize(url.Text, _dlMode);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message} \r\nInnerException: { ex.InnerException?.Message}", @"Error");
                    }
                    finally
                    {
                        _completed = true;
                        InvokeSyncComplete();
                    }
                }).Start();             
            }
            else if (DownloadUtils.IsActive && syncButton.Text == AbortActionText)
            {
                DownloadUtils.IsActive = false;
                syncButton.Enabled = false;
            }
            else if (syncButton.Text == DefaultActionText &&
                     string.IsNullOrWhiteSpace(url.Text))
            {
                status.Text = @"Enter the download url";
            }
            else if (syncButton.Text == DefaultActionText &&
                     string.IsNullOrWhiteSpace(directoryPath.Text))
            {
                status.Text = @"Enter local directory path";
            }
        }

        [SilentFailure]
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {          
            _exiting = true;
            DownloadUtils.IsActive = false;
            status.Text = @"Preparing for exit... Please Wait.";
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
            nudConcurrency.Value = Settings.Default.ConcurrentDownloads;
            favoritesRadio.Checked = Settings.Default.favoritesRadio;
            playlistRadio.Checked = Settings.Default.PlaylistRadio;
            artistRadio.Checked = Settings.Default.artistRadio;
            trackRadio.Checked = Settings.Default.trackRadio;
            chk_convertToMp3.Checked = Settings.Default.chk_convertToMp3;
            chk_excl_m4a.Checked = Settings.Default.chk_excl_m4a;
            chk_exl_aac.Checked = Settings.Default.chk_exl_aac;
            chk_folderByArtist.Checked = Settings.Default.chk_folderByArtist;
            chk_highquality.Checked = Settings.Default.chk_highquality;
            chk_includeArtistinFilename.Checked = Settings.Default.chk_includeArtistinFilename;
            chk_replaceIllegalCharacters.Checked = Settings.Default.chk_replaceIllegalCharacters;
            rbttn_oneWay.Checked = Settings.Default.rbttn_oneWay;
            rbttn_twoWay.Checked = Settings.Default.rbttn_twoWay;
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (_aboutWindow.Visible)
            {
                _aboutWindow.Focus();
            }
            else
            {
                _aboutWindow.Show();
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

        private delegate void ProgressBarUpdate();

        private delegate void PerformSyncComplete();

        private delegate void PerformStatusUpdate();

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateUtils.InstallUpdateSyncWithInfo();
        }

        private void rbttn_twoWay_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gbox_downMethod_Enter(object sender, EventArgs e)
        {

        }
    }
}