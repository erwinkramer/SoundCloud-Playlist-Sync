namespace Soundcloud_Playlist_Downloader
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.syncButton = new System.Windows.Forms.Button();
            this.browseButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.deleteRemovedSongs = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.favoritesRadio = new System.Windows.Forms.RadioButton();
            this.playlistRadio = new System.Windows.Forms.RadioButton();
            this.artistRadio = new System.Windows.Forms.RadioButton();
            this.directoryPath = new System.Windows.Forms.TextBox();
            this.url = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL";
            // 
            // syncButton
            // 
            this.syncButton.Location = new System.Drawing.Point(15, 169);
            this.syncButton.Name = "syncButton";
            this.syncButton.Size = new System.Drawing.Size(390, 23);
            this.syncButton.TabIndex = 4;
            this.syncButton.Text = "Synchronize";
            this.syncButton.UseVisualStyleBackColor = true;
            this.syncButton.Click += new System.EventHandler(this.syncButton_Click);
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(353, 118);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(52, 23);
            this.browseButton.TabIndex = 6;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Local Directory";
            // 
            // deleteRemovedSongs
            // 
            this.deleteRemovedSongs.AutoSize = true;
            this.deleteRemovedSongs.Location = new System.Drawing.Point(96, 146);
            this.deleteRemovedSongs.Name = "deleteRemovedSongs";
            this.deleteRemovedSongs.Size = new System.Drawing.Size(239, 17);
            this.deleteRemovedSongs.TabIndex = 8;
            this.deleteRemovedSongs.Text = "Delete Removed Songs From Local Directory";
            this.deleteRemovedSongs.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 222);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(421, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status
            // 
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(0, 17);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 198);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(390, 13);
            this.progressBar.TabIndex = 10;
            // 
            // favoritesRadio
            // 
            this.favoritesRadio.AutoSize = true;
            this.favoritesRadio.Location = new System.Drawing.Point(96, 64);
            this.favoritesRadio.Name = "favoritesRadio";
            this.favoritesRadio.Size = new System.Drawing.Size(292, 17);
            this.favoritesRadio.TabIndex = 12;
            this.favoritesRadio.Text = "Download all songs favorited by the user at this profile url";
            this.favoritesRadio.UseVisualStyleBackColor = true;
            // 
            // playlistRadio
            // 
            this.playlistRadio.AutoSize = true;
            this.playlistRadio.Checked = true;
            this.playlistRadio.Location = new System.Drawing.Point(96, 40);
            this.playlistRadio.Name = "playlistRadio";
            this.playlistRadio.Size = new System.Drawing.Size(207, 17);
            this.playlistRadio.TabIndex = 11;
            this.playlistRadio.TabStop = true;
            this.playlistRadio.Text = "Download all songs from this playlist url";
            this.playlistRadio.UseVisualStyleBackColor = true;
            // 
            // artistRadio
            // 
            this.artistRadio.AutoSize = true;
            this.artistRadio.Location = new System.Drawing.Point(96, 87);
            this.artistRadio.Name = "artistRadio";
            this.artistRadio.Size = new System.Drawing.Size(194, 17);
            this.artistRadio.TabIndex = 13;
            this.artistRadio.Text = "Download all songs by this artists url";
            this.artistRadio.UseVisualStyleBackColor = true;
            // 
            // directoryPath
            // 
            this.directoryPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Soundcloud_Playlist_Downloader.Properties.Settings.Default, "LocalPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.directoryPath.Location = new System.Drawing.Point(96, 120);
            this.directoryPath.Name = "directoryPath";
            this.directoryPath.Size = new System.Drawing.Size(251, 20);
            this.directoryPath.TabIndex = 5;
            this.directoryPath.Text = global::Soundcloud_Playlist_Downloader.Properties.Settings.Default.LocalPath;
            // 
            // url
            // 
            this.url.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Soundcloud_Playlist_Downloader.Properties.Settings.Default, "PlaylistUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.url.Location = new System.Drawing.Point(96, 13);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(309, 20);
            this.url.TabIndex = 1;
            this.url.Text = global::Soundcloud_Playlist_Downloader.Properties.Settings.Default.PlaylistUrl;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 244);
            this.Controls.Add(this.artistRadio);
            this.Controls.Add(this.favoritesRadio);
            this.Controls.Add(this.playlistRadio);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.deleteRemovedSongs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.directoryPath);
            this.Controls.Add(this.syncButton);
            this.Controls.Add(this.url);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "SoundCloud Playlist Sync";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox url;
        private System.Windows.Forms.Button syncButton;
        private System.Windows.Forms.TextBox directoryPath;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox deleteRemovedSongs;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.RadioButton playlistRadio;
        private System.Windows.Forms.RadioButton favoritesRadio;
        private System.Windows.Forms.RadioButton artistRadio;
    }
}

