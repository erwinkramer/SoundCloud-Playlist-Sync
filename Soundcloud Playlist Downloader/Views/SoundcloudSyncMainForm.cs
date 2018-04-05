using System;
using System.Configuration;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using Soundcloud_Playlist_Downloader.Properties;
using Soundcloud_Playlist_Downloader.Utils;

namespace Soundcloud_Playlist_Downloader.Views
{
    public partial class SoundcloudSyncMainForm : Form
    {
        private static bool Highqualitysong;
        private static bool ConvertToMp3;
        private static bool IncludeArtistInFilename;
        private static int SyncMethod = 1;
        private static EnumUtil.DownloadMode _dlMode;
        private static bool FoldersPerArtist;
        private static bool ReplaceIllegalCharacters;
        private static bool IncludeDateInFilename;
        private static bool ExcludeM4A;
        private static bool ExcludeAac;
        private static bool CreatePlaylists;
        private static bool MergePlaylists;
        private static int ConcurrentDownloads;
        private static bool ConfigStateActive;
        private static int ConfigStateCurrentIndex = 1;

        private readonly string AbortActionText = "Abort";
        private readonly BoxAbout _aboutWindow = new BoxAbout();
        private readonly API_Config _apiConfigSettings;

        private readonly string DefaultActionText = "Synchronize";

        private readonly PerformStatusUpdate _performStatusUpdateImplementation;

        private readonly PerformSyncComplete _performSyncCompleteImplementation;
        private readonly ProgressBarUpdate _progressBarUpdateImplementation;
        private ProgressUtils progressUtil;
        private ClientIDsUtils clientIdUtil;
        private UpdateUtils updateUtil;

        public SoundcloudSyncMainForm()
        {
            InitializeComponent();

            updateUtil = new UpdateUtils();
            updateToolStripMenuItem.Text = updateUtil.LabelTextForCurrentStatus();

            clientIdUtil = new ClientIDsUtils();
            _apiConfigSettings = new API_Config(clientIdUtil);
            progressUtil = new ProgressUtils();

            Text = $"SoundCloud Playlist Sync {Version()} Stable";
            _performSyncCompleteImplementation = SyncCompleteButton;
            _progressBarUpdateImplementation = UpdateProgressBar;
            _performStatusUpdateImplementation = UpdateStatus;
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
            return "-";
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
                directoryPath.Text = dialog.SelectedPath?.ToLower();
            }
        }

        [SilentFailure]
        private void UpdateStatus()
        {
            if (!progressUtil.Exiting)
            {
                if (progressUtil.IsActive && progressBar.Value == progressBar.Maximum &&
                progressBar.Value != progressBar.Minimum)
                {
                    status.Text = @"Completed";
                }
                else if (progressUtil.IsActive && progressBar.Value >= progressBar.Minimum && progressBar.Maximum > 0)
                {
                    status.Text = $"Synchronizing... {progressBar.Value} of {progressBar.Maximum} songs downloaded.";
                }
                else if (progressUtil.IsActive && progressUtil.Completed && !progressUtil.IsError)
                {
                    status.Text = @"Tracks are already synchronized";
                }
                else if (progressUtil.IsActive && progressUtil.Completed && progressUtil.IsError)
                {
                    status.Text = @"An error prevented synchronization from starting";
                }
                else if (!progressUtil.IsActive && syncButton.Text == AbortActionText)
                {
                    status.Text = @"Aborting downloads... Please Wait.";
                }
                else if (progressUtil.IsActive)
                {
                    var plural = "";
                    if (_dlMode != EnumUtil.DownloadMode.Track)
                        plural = "s";
                    status.Text = $"Fetching track{plural} to download...";
                }
                else if (!progressUtil.IsActive)
                {
                    status.Text = @"Aborted";
                }
            }
            else if (progressUtil.Completed)
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
            progressBar.Maximum = progressUtil.SongsToDownload;

            progressBar.Value = progressUtil.SongsDownloaded;

            TaskbarManager.Instance.SetProgressValue(progressUtil.SongsDownloaded, progressUtil.SongsToDownload);
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
            if (progressUtil.Exiting)
            {
                Dispose();
            }
        }

        private void syncButton_Click(object sender, EventArgs e)
        {
            SaveSettingsToConfig(ConfigStateCurrentIndex);

            _dlMode = playlistRadio.Checked
            ? EnumUtil.DownloadMode.Playlist
            : userPlaylists.Checked ? EnumUtil.DownloadMode.UserPlaylists
            : favoritesRadio.Checked ? EnumUtil.DownloadMode.Favorites
            : artistRadio.Checked ? EnumUtil.DownloadMode.Artist : EnumUtil.DownloadMode.Track;
            if (!string.IsNullOrWhiteSpace(url.Text?.ToLower()) &&
            !string.IsNullOrWhiteSpace(directoryPath.Text?.ToLower()) &&
            syncButton.Text == DefaultActionText)
            {
                syncButton.Text = AbortActionText;
                status.Text = @"Checking for track changes...";
                progressUtil.Completed = false;

                progressBar.Value = 0;
                progressBar.Maximum = 0;
                progressBar.Minimum = 0;
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);

                Highqualitysong = chk_highquality.Checked;
                ConvertToMp3 = chk_convertToMp3.Checked;
                IncludeArtistInFilename = chk_includeArtistinFilename.Checked;
                IncludeDateInFilename = chk_IncludeCreationDate.Checked;
                SyncMethod = rbttn_oneWay.Checked ? 1 : 2;
                FoldersPerArtist = chk_folderByArtist.Checked;
                ReplaceIllegalCharacters = chk_replaceIllegalCharacters.Checked;
                ExcludeAac = chk_excl_m4a.Checked;
                ExcludeM4A = chk_excl_m4a.Checked;
                CreatePlaylists = chk_CreatePlaylists.Checked;
                ConcurrentDownloads = (int)nudConcurrency.Value;
                MergePlaylists = chk_MergePlaylists.Checked;

                Uri soundCloudUri;
                try
                {
                    soundCloudUri = new Uri(url?.Text?.ToLower());
                }
                catch (Exception)
                {
                    status.Text = @"Invalid URL";
                    progressUtil.Completed = true;
                    InvokeSyncComplete();
                    return;
                }

                var filesystemUtil = new FilesystemUtils(new DirectoryInfo(directoryPath?.Text?.ToLower()), IncludeArtistInFilename, FoldersPerArtist, ReplaceIllegalCharacters, IncludeDateInFilename);
                var manifestUtil = new ManifestUtils(progressUtil, filesystemUtil, soundCloudUri, _dlMode, SyncMethod);
                var playlistUtil = new PlaylistUtils(manifestUtil);
                DownloadUtils downloadUtil = new DownloadUtils(clientIdUtil, ExcludeM4A, ExcludeAac, ConvertToMp3, manifestUtil, Highqualitysong, ConcurrentDownloads);
                var syncUtil = new SyncUtils(CreatePlaylists, manifestUtil, downloadUtil, playlistUtil);
                if (_dlMode != EnumUtil.DownloadMode.Track)
                {
                    bool differentmanifest;
                    if (!manifestUtil.FindManifestAndBackup(out differentmanifest))
                    {
                        if (differentmanifest)
                        {
                            status.Text = @"Change settings or directory.";
                            progressUtil.Completed = true;
                            InvokeSyncComplete();
                            return;
                        }
                    }
                }
                new Thread(() =>
                {
    // perform progress updates
    while (!progressUtil.Completed && !progressUtil.Exiting)
                    {
                        Thread.Sleep(500);
                        InvokeUpdateStatus();

                        this.Invoke((MethodInvoker)(() => lb_progressOfTracks.DataSource = progressUtil.GetTrackProgressValues()));
                        this.Invoke((MethodInvoker)(() => lb_progressOfTracks.Refresh()));

                        InvokeUpdateProgressBar();
                    }
                    if (!progressUtil.Exiting)
                    {
                        InvokeUpdateStatus();
                    }
                }).Start();

                new Thread(() =>
                {
                    try
                    {
                        var sync = new SoundcloudSync(syncUtil, MergePlaylists);
                        sync.Synchronize(url?.Text?.ToLower());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message} { ExceptionHandlerUtils.GetInnerExceptionMessages(ex)}", @"Error");
                    }
                    finally
                    {
                        progressUtil.Completed = true;
                        InvokeSyncComplete();
                    }
                }).Start();
            }
            else if (progressUtil.IsActive && syncButton.Text == AbortActionText)
            {
                progressUtil.IsActive = false;
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
            progressUtil.Exiting = true;
            progressUtil.IsActive = false;
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
            LoadSettingsFromCurrentConfig(Settings.Default.ConfigStateCurrentIndex);
        }

        private void SaveSettingsToConfig(int currentIndex)
        {
            Settings.Default.ConfigStateCurrentIndex = currentIndex;
            SaveSettingToConfig(chk_configActive.Name, chk_configActive.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig("LocalPath", directoryPath?.Text, directoryPath?.Text?.GetType());
            SaveSettingToConfig("PlaylistUrl", url?.Text, url?.Text?.GetType());
            SaveSettingToConfig(nameof(ConcurrentDownloads), nudConcurrency.Value.ToString(), nudConcurrency.Value.GetType());
            SaveSettingToConfig(favoritesRadio.Name, favoritesRadio.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig("playlistRadio", playlistRadio.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(userPlaylists.Name, userPlaylists.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(artistRadio.Name, artistRadio.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(trackRadio.Name, trackRadio.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_convertToMp3.Name, chk_convertToMp3.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_excl_m4a.Name, chk_excl_m4a.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_exl_aac.Name, chk_exl_aac.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_folderByArtist.Name, chk_folderByArtist.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_highquality.Name, chk_highquality.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_includeArtistinFilename.Name, chk_includeArtistinFilename.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_IncludeCreationDate.Name, chk_IncludeCreationDate.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_replaceIllegalCharacters.Name, chk_replaceIllegalCharacters.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(rbttn_oneWay.Name, rbttn_oneWay.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(rbttn_twoWay.Name, rbttn_twoWay.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_MergePlaylists.Name, chk_MergePlaylists.Checked.ToString(), typeof(Boolean));
            SaveSettingToConfig(chk_CreatePlaylists.Name, chk_CreatePlaylists.Checked.ToString(), typeof(Boolean));
            Settings.Default.Save();
        }

        public void SaveSettingToConfig(string propertyName, string propertyValue, Type propertyType)
        {
            string accessString = GetAccessString(Settings.Default.ConfigStateCurrentIndex);
            switch (Type.GetTypeCode(propertyType))
            {
                case TypeCode.Boolean:
                    Settings.Default[accessString + propertyName] = Boolean.Parse(propertyValue);
                    break;
                case TypeCode.Decimal:
                    Settings.Default[accessString + propertyName] = Int32.Parse(propertyValue);
                    break;
                case TypeCode.String:
                    Settings.Default[accessString + propertyName] = propertyValue.ToLower();
                    break;
                default:
                    break;
            }
        }

        private void LoadSettingsFromCurrentConfig(int currentIndex)
        {
            Settings.Default.ConfigStateCurrentIndex = currentIndex;
            string accessString = GetAccessString(currentIndex);
            lbl_currentConfig.Text = Settings.Default.ConfigStateCurrentIndex.ToString();

            chk_configActive.Checked = (bool) LoadSettingFromConfig(accessString, chk_configActive.Name, typeof(Boolean));
            url.Text = (string)LoadSettingFromConfig(accessString, "PlaylistUrl", typeof(String));
            directoryPath.Text = (string)LoadSettingFromConfig(accessString, "LocalPath", typeof(String));
            nudConcurrency.Value = (int)LoadSettingFromConfig(accessString, nameof(ConcurrentDownloads), typeof(Int32));
            favoritesRadio.Checked = (bool)LoadSettingFromConfig(accessString, favoritesRadio.Name, typeof(Boolean));
            userPlaylists.Checked = (bool)LoadSettingFromConfig(accessString, userPlaylists.Name, typeof(Boolean));
            playlistRadio.Checked = (bool)LoadSettingFromConfig(accessString, "playlistRadio", typeof(Boolean));
            artistRadio.Checked = (bool)LoadSettingFromConfig(accessString, artistRadio.Name, typeof(Boolean));
            trackRadio.Checked = (bool)LoadSettingFromConfig(accessString, trackRadio.Name, typeof(Boolean));
            chk_convertToMp3.Checked = (bool)LoadSettingFromConfig(accessString, chk_convertToMp3.Name, typeof(Boolean));
            chk_excl_m4a.Checked = (bool)LoadSettingFromConfig(accessString, chk_excl_m4a.Name, typeof(Boolean));
            chk_exl_aac.Checked = (bool)LoadSettingFromConfig(accessString, chk_exl_aac.Name, typeof(Boolean));
            chk_IncludeCreationDate.Checked = (bool)LoadSettingFromConfig(accessString, chk_IncludeCreationDate.Name, typeof(Boolean));
            chk_folderByArtist.Checked = (bool)LoadSettingFromConfig(accessString, chk_folderByArtist.Name, typeof(Boolean));
            chk_highquality.Checked = (bool)LoadSettingFromConfig(accessString, chk_highquality.Name, typeof(Boolean));
            chk_includeArtistinFilename.Checked = (bool)LoadSettingFromConfig(accessString, chk_includeArtistinFilename.Name, typeof(Boolean));
            chk_replaceIllegalCharacters.Checked = (bool)LoadSettingFromConfig(accessString, chk_replaceIllegalCharacters.Name, typeof(Boolean));
            chk_CreatePlaylists.Checked = (bool)LoadSettingFromConfig(accessString, chk_CreatePlaylists.Name, typeof(Boolean));
            chk_MergePlaylists.Checked = (bool)LoadSettingFromConfig(accessString, chk_MergePlaylists.Name, typeof(Boolean));
            rbttn_oneWay.Checked = (bool)LoadSettingFromConfig(accessString, rbttn_oneWay.Name, typeof(Boolean));
            rbttn_twoWay.Checked = (bool)LoadSettingFromConfig(accessString, rbttn_twoWay.Name, typeof(Boolean));
        }

        public object LoadSettingFromConfig(string accessString, string propertyName, Type propertyType)
        {
            try
            {
                return Settings.Default[accessString + propertyName];

            }
            catch (SettingsPropertyNotFoundException)
            {
                var property = new SettingsProperty(accessString + propertyName)
                {
                    DefaultValue = LoadSettingFromConfig("", propertyName, propertyType),
                    IsReadOnly = false,
                    PropertyType = propertyType,
                    Provider = Settings.Default.Providers["LocalFileSettingsProvider"],
                };
                property.Attributes.Add(typeof(System.Configuration.UserScopedSettingAttribute), new System.Configuration.UserScopedSettingAttribute());
                Settings.Default.Properties.Add(property);
                Settings.Default.Save();
                return Settings.Default[accessString + propertyName];
            }
        }

        private string GetAccessString(int currentIndex)
        {
            string accessString = "";
            if (currentIndex != 1)
                accessString = ConfigStateCurrentIndex.ToString();
            return accessString;
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
            updateUtil.InstallUpdateSyncWithInfo();
            updateToolStripMenuItem.Text = updateUtil.LabelTextForCurrentStatus();
        } 

        private void rbttn_twoWay_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gbox_downMethod_Enter(object sender, EventArgs e)
        {

        }

        private void clientIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_apiConfigSettings.Visible)
            {
                _apiConfigSettings.Focus();
            }
            else
            {
                _apiConfigSettings.Show();
            }
        }
      
        private void chk_configActive_CheckedChanged(object sender, EventArgs e)
        {
            ConfigStateActive = chk_configActive.Checked;
        }

        private void config1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettingsToConfig(ConfigStateCurrentIndex);
            lbl_currentConfig.Text = "1";
            ConfigStateCurrentIndex = 1;
            LoadSettingsFromCurrentConfig(ConfigStateCurrentIndex);
        }

        private void config2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettingsToConfig(ConfigStateCurrentIndex);
            lbl_currentConfig.Text = "2";
            ConfigStateCurrentIndex = 2;
            LoadSettingsFromCurrentConfig(ConfigStateCurrentIndex);
        }

        private void config3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettingsToConfig(ConfigStateCurrentIndex);
            lbl_currentConfig.Text = "3";
            ConfigStateCurrentIndex = 3;
            LoadSettingsFromCurrentConfig(ConfigStateCurrentIndex);
        }

        private void config4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettingsToConfig(ConfigStateCurrentIndex);
            lbl_currentConfig.Text = "4";
            ConfigStateCurrentIndex = 4;
            LoadSettingsFromCurrentConfig(ConfigStateCurrentIndex);
        }

        private void config5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettingsToConfig(ConfigStateCurrentIndex);
            lbl_currentConfig.Text = "5";
            ConfigStateCurrentIndex = 5;
            LoadSettingsFromCurrentConfig(ConfigStateCurrentIndex);
        }
    }
}