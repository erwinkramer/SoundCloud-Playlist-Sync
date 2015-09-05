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

namespace Soundcloud_Playlist_Downloader
{
    public partial class Form1 : Form
    {

        private string CLIENT_ID = "93a4fae1bd98b84c9b4f6bf1cc838b4f";
        private box_about aboutWindow = new box_about();

        private PlaylistSync sync = null;
        private delegate void ProgressBarUpdate();
        private delegate void PerformSyncComplete();
        private delegate void PerformStatusUpdate();

        private bool completed = false;
        public static bool Highqualitysong = false;

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
            progressBar.Maximum = sync.SongsToDownload.Count;
            progressBar.Value = sync.SongsDownloaded.Count;
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
                Form1.Highqualitysong = chk_highquality.Checked;
                new Thread(() =>
                {
                    try
                    {
                        sync.Synchronize(
                            url: url.Text,
                            mode: playlistRadio.Checked ? PlaylistSync.DownloadMode.Playlist : favoritesRadio.Checked ? PlaylistSync.DownloadMode.Favorites : PlaylistSync.DownloadMode.Artist,
                            directory: directoryPath.Text, 
                            deleteRemovedSongs: deleteRemovedSongs.Checked, 
                            clientId: CLIENT_ID,
                            foldersPerArtist: chk_folderByArtist.Checked
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
            Settings.Default.Save();
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

        }

        private void chk_folderByArtist_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
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
    }
}
