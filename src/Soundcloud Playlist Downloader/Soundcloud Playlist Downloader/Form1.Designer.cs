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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.syncButton = new System.Windows.Forms.Button();
            this.browseButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.chk_deleteRemovedOrAlteredSongs = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.favoritesRadio = new System.Windows.Forms.RadioButton();
            this.playlistRadio = new System.Windows.Forms.RadioButton();
            this.artistRadio = new System.Windows.Forms.RadioButton();
            this.chk_folderByArtist = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.directoryPath = new System.Windows.Forms.TextBox();
            this.url = new System.Windows.Forms.TextBox();
            this.chk_highquality = new System.Windows.Forms.CheckBox();
            this.tt_qualityExplanation = new System.Windows.Forms.ToolTip(this.components);
            this.chk_convertToMp3 = new System.Windows.Forms.CheckBox();
            this.chk_replaceIllegalCharacters = new System.Windows.Forms.CheckBox();
            this.chk_includeArtistinFilename = new System.Windows.Forms.CheckBox();
            this.chk_redownloadLocallyRemoved = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL";
            // 
            // syncButton
            // 
            this.syncButton.Location = new System.Drawing.Point(15, 298);
            this.syncButton.Name = "syncButton";
            this.syncButton.Size = new System.Drawing.Size(397, 23);
            this.syncButton.TabIndex = 4;
            this.syncButton.Text = "Synchronize";
            this.syncButton.UseVisualStyleBackColor = true;
            this.syncButton.Click += new System.EventHandler(this.syncButton_Click);
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(350, 272);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(62, 20);
            this.browseButton.TabIndex = 6;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 279);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Local dir.";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // chk_deleteRemovedOrAlteredSongs
            // 
            this.chk_deleteRemovedOrAlteredSongs.AutoSize = true;
            this.chk_deleteRemovedOrAlteredSongs.Location = new System.Drawing.Point(70, 238);
            this.chk_deleteRemovedOrAlteredSongs.Name = "chk_deleteRemovedOrAlteredSongs";
            this.chk_deleteRemovedOrAlteredSongs.Size = new System.Drawing.Size(293, 30);
            this.chk_deleteRemovedOrAlteredSongs.TabIndex = 8;
            this.chk_deleteRemovedOrAlteredSongs.Text = "Locally delete songs removed from SC and delete songs \r\npreviously downloaded wit" +
    "h different settings.";
            this.chk_deleteRemovedOrAlteredSongs.UseVisualStyleBackColor = true;
            this.chk_deleteRemovedOrAlteredSongs.CheckedChanged += new System.EventHandler(this.chk_deleteRemovedOrAlteredSongs_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 350);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(424, 22);
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
            this.progressBar.Location = new System.Drawing.Point(15, 327);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(396, 13);
            this.progressBar.TabIndex = 10;
            // 
            // favoritesRadio
            // 
            this.favoritesRadio.AutoSize = true;
            this.favoritesRadio.Location = new System.Drawing.Point(70, 76);
            this.favoritesRadio.Name = "favoritesRadio";
            this.favoritesRadio.Size = new System.Drawing.Size(303, 17);
            this.favoritesRadio.TabIndex = 12;
            this.favoritesRadio.Text = "Download all songs favorited by the user at this profile URL";
            this.favoritesRadio.UseVisualStyleBackColor = true;
            // 
            // playlistRadio
            // 
            this.playlistRadio.AutoSize = true;
            this.playlistRadio.Checked = true;
            this.playlistRadio.Location = new System.Drawing.Point(70, 53);
            this.playlistRadio.Name = "playlistRadio";
            this.playlistRadio.Size = new System.Drawing.Size(218, 17);
            this.playlistRadio.TabIndex = 11;
            this.playlistRadio.TabStop = true;
            this.playlistRadio.Text = "Download all songs from this playlist URL";
            this.playlistRadio.UseVisualStyleBackColor = true;
            // 
            // artistRadio
            // 
            this.artistRadio.AutoSize = true;
            this.artistRadio.Location = new System.Drawing.Point(70, 99);
            this.artistRadio.Name = "artistRadio";
            this.artistRadio.Size = new System.Drawing.Size(205, 17);
            this.artistRadio.TabIndex = 13;
            this.artistRadio.Text = "Download all songs by this artists URL";
            this.artistRadio.UseVisualStyleBackColor = true;
            // 
            // chk_folderByArtist
            // 
            this.chk_folderByArtist.AutoSize = true;
            this.chk_folderByArtist.Checked = true;
            this.chk_folderByArtist.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_folderByArtist.Location = new System.Drawing.Point(70, 169);
            this.chk_folderByArtist.Name = "chk_folderByArtist";
            this.chk_folderByArtist.Size = new System.Drawing.Size(172, 17);
            this.chk_folderByArtist.TabIndex = 15;
            this.chk_folderByArtist.Text = "Sort songs into folders by artist ";
            this.chk_folderByArtist.UseVisualStyleBackColor = true;
            this.chk_folderByArtist.CheckedChanged += new System.EventHandler(this.chk_folderByArtist_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(424, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click_1);
            // 
            // directoryPath
            // 
            this.directoryPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Soundcloud_Playlist_Downloader.Properties.Settings.Default, "LocalPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.directoryPath.Location = new System.Drawing.Point(71, 274);
            this.directoryPath.Name = "directoryPath";
            this.directoryPath.Size = new System.Drawing.Size(273, 20);
            this.directoryPath.TabIndex = 5;
            this.directoryPath.Text = global::Soundcloud_Playlist_Downloader.Properties.Settings.Default.LocalPath;
            // 
            // url
            // 
            this.url.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Soundcloud_Playlist_Downloader.Properties.Settings.Default, "PlaylistUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.url.Location = new System.Drawing.Point(70, 27);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(273, 20);
            this.url.TabIndex = 1;
            this.url.Text = global::Soundcloud_Playlist_Downloader.Properties.Settings.Default.PlaylistUrl;
            // 
            // chk_highquality
            // 
            this.chk_highquality.AutoSize = true;
            this.chk_highquality.Checked = true;
            this.chk_highquality.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_highquality.Location = new System.Drawing.Point(70, 123);
            this.chk_highquality.Name = "chk_highquality";
            this.chk_highquality.Size = new System.Drawing.Size(216, 17);
            this.chk_highquality.TabIndex = 17;
            this.chk_highquality.Text = "Choose high quality versions, if available";
            this.tt_qualityExplanation.SetToolTip(this.chk_highquality, "Some songs (not all) can be downloaded in high quality. These files are usually m" +
        "uch larger than the low quality MP3, thus taking more time to download. ");
            this.chk_highquality.UseVisualStyleBackColor = true;
            this.chk_highquality.CheckedChanged += new System.EventHandler(this.chk_highquality_CheckedChanged);
            // 
            // chk_convertToMp3
            // 
            this.chk_convertToMp3.AutoSize = true;
            this.chk_convertToMp3.Checked = true;
            this.chk_convertToMp3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_convertToMp3.Location = new System.Drawing.Point(282, 123);
            this.chk_convertToMp3.Name = "chk_convertToMp3";
            this.chk_convertToMp3.Size = new System.Drawing.Size(129, 17);
            this.chk_convertToMp3.TabIndex = 18;
            this.chk_convertToMp3.Text = "Convert these to MP3";
            this.tt_qualityExplanation.SetToolTip(this.chk_convertToMp3, "Writing metadata to high quality files in a lossless format is problematic for so" +
        "me fields. There isn\'t a broadly used standard like ID3 for MP3.");
            this.chk_convertToMp3.UseVisualStyleBackColor = true;
            this.chk_convertToMp3.CheckedChanged += new System.EventHandler(this.chk_convertToMp3_CheckedChanged);
            // 
            // chk_replaceIllegalCharacters
            // 
            this.chk_replaceIllegalCharacters.AutoSize = true;
            this.chk_replaceIllegalCharacters.Checked = true;
            this.chk_replaceIllegalCharacters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_replaceIllegalCharacters.Location = new System.Drawing.Point(70, 215);
            this.chk_replaceIllegalCharacters.Name = "chk_replaceIllegalCharacters";
            this.chk_replaceIllegalCharacters.Size = new System.Drawing.Size(333, 17);
            this.chk_replaceIllegalCharacters.TabIndex = 22;
            this.chk_replaceIllegalCharacters.Text = "Replace illegal characters in filename with equivalent instead of _";
            this.tt_qualityExplanation.SetToolTip(this.chk_replaceIllegalCharacters, "Characters to be replaced: / ? < > \\ : * | \"\r\nWill be replaced with Halfwidth and" +
        " Fullwidth Forms\r\n");
            this.chk_replaceIllegalCharacters.UseVisualStyleBackColor = true;
            // 
            // chk_includeArtistinFilename
            // 
            this.chk_includeArtistinFilename.AutoSize = true;
            this.chk_includeArtistinFilename.Checked = true;
            this.chk_includeArtistinFilename.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_includeArtistinFilename.Location = new System.Drawing.Point(70, 192);
            this.chk_includeArtistinFilename.Name = "chk_includeArtistinFilename";
            this.chk_includeArtistinFilename.Size = new System.Drawing.Size(168, 17);
            this.chk_includeArtistinFilename.TabIndex = 20;
            this.chk_includeArtistinFilename.Text = "Include artist name in filename";
            this.chk_includeArtistinFilename.UseVisualStyleBackColor = true;
            // 
            // chk_redownloadLocallyRemoved
            // 
            this.chk_redownloadLocallyRemoved.AutoSize = true;
            this.chk_redownloadLocallyRemoved.Checked = true;
            this.chk_redownloadLocallyRemoved.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_redownloadLocallyRemoved.Location = new System.Drawing.Point(70, 146);
            this.chk_redownloadLocallyRemoved.Name = "chk_redownloadLocallyRemoved";
            this.chk_redownloadLocallyRemoved.Size = new System.Drawing.Size(196, 17);
            this.chk_redownloadLocallyRemoved.TabIndex = 21;
            this.chk_redownloadLocallyRemoved.Text = "Re-download locally removed songs";
            this.chk_redownloadLocallyRemoved.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 372);
            this.Controls.Add(this.chk_replaceIllegalCharacters);
            this.Controls.Add(this.chk_redownloadLocallyRemoved);
            this.Controls.Add(this.chk_includeArtistinFilename);
            this.Controls.Add(this.chk_convertToMp3);
            this.Controls.Add(this.chk_highquality);
            this.Controls.Add(this.chk_folderByArtist);
            this.Controls.Add(this.artistRadio);
            this.Controls.Add(this.favoritesRadio);
            this.Controls.Add(this.playlistRadio);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.chk_deleteRemovedOrAlteredSongs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.directoryPath);
            this.Controls.Add(this.syncButton);
            this.Controls.Add(this.url);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "SoundCloud Playlist Sync r1.0.0.52";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.CheckBox chk_deleteRemovedOrAlteredSongs;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.RadioButton playlistRadio;
        private System.Windows.Forms.RadioButton favoritesRadio;
        private System.Windows.Forms.RadioButton artistRadio;
        private System.Windows.Forms.CheckBox chk_folderByArtist;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.CheckBox chk_highquality;
        private System.Windows.Forms.ToolTip tt_qualityExplanation;
        private System.Windows.Forms.CheckBox chk_convertToMp3;
        private System.Windows.Forms.CheckBox chk_includeArtistinFilename;
        private System.Windows.Forms.CheckBox chk_redownloadLocallyRemoved;
        private System.Windows.Forms.CheckBox chk_replaceIllegalCharacters;
    }
}

