
namespace Soundcloud_Playlist_Downloader.Views
{
    partial class SoundcloudSyncMainForm
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
            this.syncButton = new System.Windows.Forms.Button();
            this.browseButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.favoritesRadio = new System.Windows.Forms.RadioButton();
            this.userPlaylists = new System.Windows.Forms.RadioButton();
            this.playlistRadio = new System.Windows.Forms.RadioButton();
            this.artistRadio = new System.Windows.Forms.RadioButton();
            this.chk_folderByArtist = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.configurationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.config1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.config2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.config3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.config4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.config5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clientIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.chk_highquality = new System.Windows.Forms.CheckBox();
            this.tt_qualityExplanation = new System.Windows.Forms.ToolTip(this.components);
            this.chk_convertToMp3 = new System.Windows.Forms.CheckBox();
            this.chk_replaceIllegalCharacters = new System.Windows.Forms.CheckBox();
            this.lbl_exclude = new System.Windows.Forms.Label();
            this.chk_excl_m4a = new System.Windows.Forms.CheckBox();
            this.chk_exl_aac = new System.Windows.Forms.CheckBox();
            this.pnl_convert = new System.Windows.Forms.Panel();
            this.rbttn_twoWay = new System.Windows.Forms.RadioButton();
            this.rbttn_oneWay = new System.Windows.Forms.RadioButton();
            this.gbox_syncMethod = new System.Windows.Forms.GroupBox();
            this.gbox_downMethod = new System.Windows.Forms.GroupBox();
            this.trackRadio = new System.Windows.Forms.RadioButton();
            this.gbox_url = new System.Windows.Forms.GroupBox();
            this.url = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbl_configurationPrefix = new System.Windows.Forms.Label();
            this.lbl_currentConfig = new System.Windows.Forms.Label();
            this.chk_configActive = new System.Windows.Forms.CheckBox();
            this.gbox_advanced_conversion = new System.Windows.Forms.GroupBox();
            this.chk_CreatePlaylists = new System.Windows.Forms.CheckBox();
            this.nudConcurrency = new System.Windows.Forms.NumericUpDown();
            this.concurrency = new System.Windows.Forms.Label();
            this.gbox_localdir = new System.Windows.Forms.GroupBox();
            this.directoryPath = new System.Windows.Forms.TextBox();
            this.lb_progressOfTracks = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl_general = new System.Windows.Forms.TabControl();
            this.tabPage_BasicOptions = new System.Windows.Forms.TabPage();
            this.tabPage_AdvancedOptions = new System.Windows.Forms.TabPage();
            this.gbox_advanced_other = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btn_FormatForTag = new System.Windows.Forms.Button();
            this.btn_FormatForName = new System.Windows.Forms.Button();
            this.chk_MergePlaylists = new System.Windows.Forms.CheckBox();
            this.gbox_advanced_enginebehaviour = new System.Windows.Forms.GroupBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.pnl_convert.SuspendLayout();
            this.gbox_syncMethod.SuspendLayout();
            this.gbox_downMethod.SuspendLayout();
            this.gbox_url.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbox_advanced_conversion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConcurrency)).BeginInit();
            this.gbox_localdir.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl_general.SuspendLayout();
            this.tabPage_BasicOptions.SuspendLayout();
            this.tabPage_AdvancedOptions.SuspendLayout();
            this.gbox_advanced_other.SuspendLayout();
            this.gbox_advanced_enginebehaviour.SuspendLayout();
            this.SuspendLayout();
            // 
            // syncButton
            // 
            this.syncButton.Location = new System.Drawing.Point(7, 234);
            this.syncButton.Name = "syncButton";
            this.syncButton.Size = new System.Drawing.Size(234, 21);
            this.syncButton.TabIndex = 4;
            this.syncButton.Tag = "STR_SYNCHRONIZE";
            this.syncButton.Text = "Synchronize";
            this.syncButton.UseVisualStyleBackColor = true;
            this.syncButton.Click += new System.EventHandler(this.syncButton_Click);
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(381, 18);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(72, 21);
            this.browseButton.TabIndex = 6;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 373);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(779, 22);
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
            this.progressBar.Location = new System.Drawing.Point(7, 207);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(234, 21);
            this.progressBar.TabIndex = 10;
            // 
            // favoritesRadio
            // 
            this.favoritesRadio.AutoSize = true;
            this.favoritesRadio.Location = new System.Drawing.Point(7, 57);
            this.favoritesRadio.Name = "favoritesRadio";
            this.favoritesRadio.Size = new System.Drawing.Size(298, 16);
            this.favoritesRadio.TabIndex = 12;
            this.favoritesRadio.Text = "All songs favorited by the user at this profile URL";
            this.favoritesRadio.UseVisualStyleBackColor = true;
            // 
            // userPlaylists
            // 
            this.userPlaylists.AutoSize = true;
            this.userPlaylists.Checked = true;
            this.userPlaylists.ForeColor = System.Drawing.Color.Black;
            this.userPlaylists.Location = new System.Drawing.Point(7, 15);
            this.userPlaylists.Name = "userPlaylists";
            this.userPlaylists.Size = new System.Drawing.Size(197, 16);
            this.userPlaylists.TabIndex = 15;
            this.userPlaylists.TabStop = true;
            this.userPlaylists.Text = "All playlists from this user URL";
            this.userPlaylists.UseVisualStyleBackColor = true;
            // 
            // playlistRadio
            // 
            this.playlistRadio.AutoSize = true;
            this.playlistRadio.Location = new System.Drawing.Point(7, 36);
            this.playlistRadio.Name = "playlistRadio";
            this.playlistRadio.Size = new System.Drawing.Size(200, 16);
            this.playlistRadio.TabIndex = 11;
            this.playlistRadio.Text = "All songs from this playlist URL";
            this.playlistRadio.UseVisualStyleBackColor = true;
            // 
            // artistRadio
            // 
            this.artistRadio.AutoSize = true;
            this.artistRadio.Location = new System.Drawing.Point(7, 78);
            this.artistRadio.Name = "artistRadio";
            this.artistRadio.Size = new System.Drawing.Size(183, 16);
            this.artistRadio.TabIndex = 13;
            this.artistRadio.Text = "All songs by this artists URL";
            this.artistRadio.UseVisualStyleBackColor = true;
            // 
            // chk_folderByArtist
            // 
            this.chk_folderByArtist.AutoSize = true;
            this.chk_folderByArtist.Checked = true;
            this.chk_folderByArtist.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_folderByArtist.Location = new System.Drawing.Point(212, 9);
            this.chk_folderByArtist.Name = "chk_folderByArtist";
            this.chk_folderByArtist.Size = new System.Drawing.Size(204, 16);
            this.chk_folderByArtist.TabIndex = 15;
            this.chk_folderByArtist.Text = "Sort songs into folders by artist ";
            this.chk_folderByArtist.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationsToolStripMenuItem,
            this.clientIDToolStripMenuItem,
            this.updateToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.languageToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(779, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // configurationsToolStripMenuItem
            // 
            this.configurationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.config1ToolStripMenuItem,
            this.config2ToolStripMenuItem,
            this.config3ToolStripMenuItem,
            this.config4ToolStripMenuItem,
            this.config5ToolStripMenuItem});
            this.configurationsToolStripMenuItem.Name = "configurationsToolStripMenuItem";
            this.configurationsToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.configurationsToolStripMenuItem.Text = "Configurations";
            // 
            // config1ToolStripMenuItem
            // 
            this.config1ToolStripMenuItem.Name = "config1ToolStripMenuItem";
            this.config1ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.config1ToolStripMenuItem.Text = "Config 1";
            this.config1ToolStripMenuItem.Click += new System.EventHandler(this.config1ToolStripMenuItem_Click);
            // 
            // config2ToolStripMenuItem
            // 
            this.config2ToolStripMenuItem.Name = "config2ToolStripMenuItem";
            this.config2ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.config2ToolStripMenuItem.Text = "Config 2";
            this.config2ToolStripMenuItem.Click += new System.EventHandler(this.config2ToolStripMenuItem_Click);
            // 
            // config3ToolStripMenuItem
            // 
            this.config3ToolStripMenuItem.Name = "config3ToolStripMenuItem";
            this.config3ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.config3ToolStripMenuItem.Text = "Config 3";
            this.config3ToolStripMenuItem.Click += new System.EventHandler(this.config3ToolStripMenuItem_Click);
            // 
            // config4ToolStripMenuItem
            // 
            this.config4ToolStripMenuItem.Name = "config4ToolStripMenuItem";
            this.config4ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.config4ToolStripMenuItem.Text = "Config 4";
            this.config4ToolStripMenuItem.Click += new System.EventHandler(this.config4ToolStripMenuItem_Click);
            // 
            // config5ToolStripMenuItem
            // 
            this.config5ToolStripMenuItem.Name = "config5ToolStripMenuItem";
            this.config5ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.config5ToolStripMenuItem.Text = "Config 5";
            this.config5ToolStripMenuItem.Click += new System.EventHandler(this.config5ToolStripMenuItem_Click);
            // 
            // clientIDToolStripMenuItem
            // 
            this.clientIDToolStripMenuItem.Name = "clientIDToolStripMenuItem";
            this.clientIDToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.clientIDToolStripMenuItem.Text = "API Config";
            this.clientIDToolStripMenuItem.Click += new System.EventHandler(this.clientIDToolStripMenuItem_Click);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.updateToolStripMenuItem.Text = "Update";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click_1);

            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "English (Default)",
            "한국어 (Korean)"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripComboBox1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Left;
            this.toolStripComboBox1.AutoSize = true;
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);

            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(300, 50);
            this.languageToolStripMenuItem.Text = "Language";
            this.languageToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.languageToolStripMenuItem.DropDownOpened += new System.EventHandler(this.languageToolStripMenuItem_DropDownOpened);

            // 
            // chk_highquality
            // 
            this.chk_highquality.AutoSize = true;
            this.chk_highquality.Checked = true;
            this.chk_highquality.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_highquality.Location = new System.Drawing.Point(7, 18);
            this.chk_highquality.Name = "chk_highquality";
            this.chk_highquality.Size = new System.Drawing.Size(253, 16);
            this.chk_highquality.TabIndex = 17;
            this.chk_highquality.Text = "Choose high quality versions if available";
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
            this.chk_convertToMp3.Location = new System.Drawing.Point(3, 3);
            this.chk_convertToMp3.Name = "chk_convertToMp3";
            this.chk_convertToMp3.Size = new System.Drawing.Size(179, 16);
            this.chk_convertToMp3.TabIndex = 18;
            this.chk_convertToMp3.Text = "Convert high quality to MP3";
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
            this.chk_replaceIllegalCharacters.Location = new System.Drawing.Point(7, 18);
            this.chk_replaceIllegalCharacters.Name = "chk_replaceIllegalCharacters";
            this.chk_replaceIllegalCharacters.Size = new System.Drawing.Size(394, 16);
            this.chk_replaceIllegalCharacters.TabIndex = 22;
            this.chk_replaceIllegalCharacters.Text = "Replace illegal characters in filename with equivalent instead of _";
            this.tt_qualityExplanation.SetToolTip(this.chk_replaceIllegalCharacters, "Characters to be replaced: / ? < > \\ : * | \"\r\nWill be replaced with Halfwidth and" +
        " Fullwidth Forms\r\n");
            this.chk_replaceIllegalCharacters.UseVisualStyleBackColor = true;
            this.chk_replaceIllegalCharacters.CheckedChanged += new System.EventHandler(this.chk_replaceIllegalCharacters_CheckedChanged);
            // 
            // lbl_exclude
            // 
            this.lbl_exclude.AutoSize = true;
            this.lbl_exclude.Location = new System.Drawing.Point(46, 23);
            this.lbl_exclude.Name = "lbl_exclude";
            this.lbl_exclude.Size = new System.Drawing.Size(55, 12);
            this.lbl_exclude.TabIndex = 23;
            this.lbl_exclude.Text = "Exclude:";
            // 
            // chk_excl_m4a
            // 
            this.chk_excl_m4a.AutoSize = true;
            this.chk_excl_m4a.Location = new System.Drawing.Point(122, 23);
            this.chk_excl_m4a.Name = "chk_excl_m4a";
            this.chk_excl_m4a.Size = new System.Drawing.Size(52, 16);
            this.chk_excl_m4a.TabIndex = 24;
            this.chk_excl_m4a.Text = ".m4a";
            this.chk_excl_m4a.UseVisualStyleBackColor = true;
            // 
            // chk_exl_aac
            // 
            this.chk_exl_aac.AutoSize = true;
            this.chk_exl_aac.Location = new System.Drawing.Point(122, 41);
            this.chk_exl_aac.Name = "chk_exl_aac";
            this.chk_exl_aac.Size = new System.Drawing.Size(49, 16);
            this.chk_exl_aac.TabIndex = 25;
            this.chk_exl_aac.Text = ".aac";
            this.chk_exl_aac.UseVisualStyleBackColor = true;
            // 
            // pnl_convert
            // 
            this.pnl_convert.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnl_convert.Controls.Add(this.chk_convertToMp3);
            this.pnl_convert.Controls.Add(this.chk_exl_aac);
            this.pnl_convert.Controls.Add(this.lbl_exclude);
            this.pnl_convert.Controls.Add(this.chk_excl_m4a);
            this.pnl_convert.Location = new System.Drawing.Point(23, 39);
            this.pnl_convert.Name = "pnl_convert";
            this.pnl_convert.Size = new System.Drawing.Size(231, 63);
            this.pnl_convert.TabIndex = 26;
            // 
            // rbttn_twoWay
            // 
            this.rbttn_twoWay.AutoSize = true;
            this.rbttn_twoWay.Location = new System.Drawing.Point(7, 36);
            this.rbttn_twoWay.Name = "rbttn_twoWay";
            this.rbttn_twoWay.Size = new System.Drawing.Size(339, 16);
            this.rbttn_twoWay.TabIndex = 28;
            this.rbttn_twoWay.Text = "Two-way sync: Locally delete songs removed from SC";
            this.rbttn_twoWay.UseVisualStyleBackColor = true;
            this.rbttn_twoWay.CheckedChanged += new System.EventHandler(this.rbttn_twoWay_CheckedChanged);
            // 
            // rbttn_oneWay
            // 
            this.rbttn_oneWay.AutoSize = true;
            this.rbttn_oneWay.Checked = true;
            this.rbttn_oneWay.Location = new System.Drawing.Point(7, 16);
            this.rbttn_oneWay.Name = "rbttn_oneWay";
            this.rbttn_oneWay.Size = new System.Drawing.Size(325, 16);
            this.rbttn_oneWay.TabIndex = 29;
            this.rbttn_oneWay.TabStop = true;
            this.rbttn_oneWay.Text = "One-way sync: Re-download locally removed songs";
            this.rbttn_oneWay.UseVisualStyleBackColor = true;
            // 
            // gbox_syncMethod
            // 
            this.gbox_syncMethod.Controls.Add(this.rbttn_oneWay);
            this.gbox_syncMethod.Controls.Add(this.rbttn_twoWay);
            this.gbox_syncMethod.Location = new System.Drawing.Point(23, 244);
            this.gbox_syncMethod.Name = "gbox_syncMethod";
            this.gbox_syncMethod.Size = new System.Drawing.Size(458, 57);
            this.gbox_syncMethod.TabIndex = 30;
            this.gbox_syncMethod.TabStop = false;
            this.gbox_syncMethod.Text = "Sync Method";
            // 
            // gbox_downMethod
            // 
            this.gbox_downMethod.Controls.Add(this.userPlaylists);
            this.gbox_downMethod.Controls.Add(this.trackRadio);
            this.gbox_downMethod.Controls.Add(this.playlistRadio);
            this.gbox_downMethod.Controls.Add(this.artistRadio);
            this.gbox_downMethod.Controls.Add(this.favoritesRadio);
            this.gbox_downMethod.Location = new System.Drawing.Point(23, 116);
            this.gbox_downMethod.Name = "gbox_downMethod";
            this.gbox_downMethod.Size = new System.Drawing.Size(458, 122);
            this.gbox_downMethod.TabIndex = 31;
            this.gbox_downMethod.TabStop = false;
            this.gbox_downMethod.Text = "Download Method";
            this.gbox_downMethod.Enter += new System.EventHandler(this.gbox_downMethod_Enter);
            // 
            // trackRadio
            // 
            this.trackRadio.AutoSize = true;
            this.trackRadio.Location = new System.Drawing.Point(7, 99);
            this.trackRadio.Name = "trackRadio";
            this.trackRadio.Size = new System.Drawing.Size(250, 16);
            this.trackRadio.TabIndex = 14;
            this.trackRadio.Text = "Single track URL (ignores sync method)";
            this.trackRadio.UseVisualStyleBackColor = true;
            // 
            // gbox_url
            // 
            this.gbox_url.Controls.Add(this.url);
            this.gbox_url.Location = new System.Drawing.Point(23, 12);
            this.gbox_url.Name = "gbox_url";
            this.gbox_url.Size = new System.Drawing.Size(458, 44);
            this.gbox_url.TabIndex = 32;
            this.gbox_url.TabStop = false;
            this.gbox_url.Text = "SoundCloud URL";
            // 
            // url
            // 
            this.url.Location = new System.Drawing.Point(7, 18);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(445, 21);
            this.url.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbl_configurationPrefix);
            this.groupBox2.Controls.Add(this.lbl_currentConfig);
            this.groupBox2.Controls.Add(this.chk_configActive);
            this.groupBox2.Location = new System.Drawing.Point(519, 48);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(248, 43);
            this.groupBox2.TabIndex = 39;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Config State";
            // 
            // lbl_configurationPrefix
            // 
            this.lbl_configurationPrefix.AutoSize = true;
            this.lbl_configurationPrefix.Location = new System.Drawing.Point(7, 21);
            this.lbl_configurationPrefix.Name = "lbl_configurationPrefix";
            this.lbl_configurationPrefix.Size = new System.Drawing.Size(79, 12);
            this.lbl_configurationPrefix.TabIndex = 40;
            this.lbl_configurationPrefix.Text = "Configuration";
            // 
            // lbl_currentConfig
            // 
            this.lbl_currentConfig.AutoSize = true;
            this.lbl_currentConfig.Location = new System.Drawing.Point(85, 21);
            this.lbl_currentConfig.Name = "lbl_currentConfig";
            this.lbl_currentConfig.Size = new System.Drawing.Size(11, 12);
            this.lbl_currentConfig.TabIndex = 39;
            this.lbl_currentConfig.Text = "1";
            // 
            // chk_configActive
            // 
            this.chk_configActive.AutoSize = true;
            this.chk_configActive.Checked = true;
            this.chk_configActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_configActive.Location = new System.Drawing.Point(107, 20);
            this.chk_configActive.Name = "chk_configActive";
            this.chk_configActive.Size = new System.Drawing.Size(58, 16);
            this.chk_configActive.TabIndex = 38;
            this.chk_configActive.Text = "Active";
            this.chk_configActive.UseVisualStyleBackColor = true;
            this.chk_configActive.CheckedChanged += new System.EventHandler(this.chk_configActive_CheckedChanged);
            // 
            // gbox_advanced_conversion
            // 
            this.gbox_advanced_conversion.Controls.Add(this.chk_highquality);
            this.gbox_advanced_conversion.Controls.Add(this.pnl_convert);
            this.gbox_advanced_conversion.Location = new System.Drawing.Point(24, 14);
            this.gbox_advanced_conversion.Name = "gbox_advanced_conversion";
            this.gbox_advanced_conversion.Size = new System.Drawing.Size(456, 109);
            this.gbox_advanced_conversion.TabIndex = 33;
            this.gbox_advanced_conversion.TabStop = false;
            this.gbox_advanced_conversion.Text = "Conversion";
            // 
            // chk_CreatePlaylists
            // 
            this.chk_CreatePlaylists.AutoSize = true;
            this.chk_CreatePlaylists.Checked = true;
            this.chk_CreatePlaylists.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_CreatePlaylists.Location = new System.Drawing.Point(212, 52);
            this.chk_CreatePlaylists.Name = "chk_CreatePlaylists";
            this.chk_CreatePlaylists.Size = new System.Drawing.Size(180, 16);
            this.chk_CreatePlaylists.TabIndex = 31;
            this.chk_CreatePlaylists.Text = "Generate m3u8 playlist files";
            this.chk_CreatePlaylists.UseVisualStyleBackColor = true;
            // 
            // nudConcurrency
            // 
            this.nudConcurrency.Location = new System.Drawing.Point(147, 42);
            this.nudConcurrency.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudConcurrency.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudConcurrency.Name = "nudConcurrency";
            this.nudConcurrency.ReadOnly = true;
            this.nudConcurrency.Size = new System.Drawing.Size(52, 21);
            this.nudConcurrency.TabIndex = 30;
            this.nudConcurrency.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // concurrency
            // 
            this.concurrency.AutoSize = true;
            this.concurrency.Location = new System.Drawing.Point(3, 48);
            this.concurrency.Name = "concurrency";
            this.concurrency.Size = new System.Drawing.Size(141, 12);
            this.concurrency.TabIndex = 29;
            this.concurrency.Text = "Amount of concurrency:";
            // 
            // gbox_localdir
            // 
            this.gbox_localdir.Controls.Add(this.directoryPath);
            this.gbox_localdir.Controls.Add(this.browseButton);
            this.gbox_localdir.Location = new System.Drawing.Point(23, 62);
            this.gbox_localdir.Name = "gbox_localdir";
            this.gbox_localdir.Size = new System.Drawing.Size(458, 47);
            this.gbox_localdir.TabIndex = 34;
            this.gbox_localdir.TabStop = false;
            this.gbox_localdir.Text = "Local Directory";
            // 
            // directoryPath
            // 
            this.directoryPath.Location = new System.Drawing.Point(7, 18);
            this.directoryPath.Name = "directoryPath";
            this.directoryPath.Size = new System.Drawing.Size(367, 21);
            this.directoryPath.TabIndex = 5;
            // 
            // lb_progressOfTracks
            // 
            this.lb_progressOfTracks.BackColor = System.Drawing.SystemColors.Menu;
            this.lb_progressOfTracks.FormattingEnabled = true;
            this.lb_progressOfTracks.HorizontalScrollbar = true;
            this.lb_progressOfTracks.ItemHeight = 12;
            this.lb_progressOfTracks.Location = new System.Drawing.Point(7, 18);
            this.lb_progressOfTracks.Name = "lb_progressOfTracks";
            this.lb_progressOfTracks.Size = new System.Drawing.Size(231, 184);
            this.lb_progressOfTracks.TabIndex = 36;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lb_progressOfTracks);
            this.groupBox1.Controls.Add(this.syncButton);
            this.groupBox1.Controls.Add(this.progressBar);
            this.groupBox1.Location = new System.Drawing.Point(519, 102);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(248, 263);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download Progress";
            // 
            // tabControl_general
            // 
            this.tabControl_general.Controls.Add(this.tabPage_BasicOptions);
            this.tabControl_general.Controls.Add(this.tabPage_AdvancedOptions);
            this.tabControl_general.Location = new System.Drawing.Point(14, 35);
            this.tabControl_general.Name = "tabControl_general";
            this.tabControl_general.SelectedIndex = 0;
            this.tabControl_general.Size = new System.Drawing.Size(498, 330);
            this.tabControl_general.TabIndex = 40;
            // 
            // tabPage_BasicOptions
            // 
            this.tabPage_BasicOptions.Controls.Add(this.gbox_url);
            this.tabPage_BasicOptions.Controls.Add(this.gbox_localdir);
            this.tabPage_BasicOptions.Controls.Add(this.gbox_syncMethod);
            this.tabPage_BasicOptions.Controls.Add(this.gbox_downMethod);
            this.tabPage_BasicOptions.Location = new System.Drawing.Point(4, 22);
            this.tabPage_BasicOptions.Name = "tabPage_BasicOptions";
            this.tabPage_BasicOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_BasicOptions.Size = new System.Drawing.Size(490, 304);
            this.tabPage_BasicOptions.TabIndex = 0;
            this.tabPage_BasicOptions.Text = "Basic Options";
            this.tabPage_BasicOptions.UseVisualStyleBackColor = true;
            // 
            // tabPage_AdvancedOptions
            // 
            this.tabPage_AdvancedOptions.Controls.Add(this.gbox_advanced_other);
            this.tabPage_AdvancedOptions.Controls.Add(this.gbox_advanced_enginebehaviour);
            this.tabPage_AdvancedOptions.Controls.Add(this.gbox_advanced_conversion);
            this.tabPage_AdvancedOptions.Location = new System.Drawing.Point(4, 22);
            this.tabPage_AdvancedOptions.Name = "tabPage_AdvancedOptions";
            this.tabPage_AdvancedOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_AdvancedOptions.Size = new System.Drawing.Size(490, 304);
            this.tabPage_AdvancedOptions.TabIndex = 1;
            this.tabPage_AdvancedOptions.Text = "Advanced Options";
            this.tabPage_AdvancedOptions.UseVisualStyleBackColor = true;
            // 
            // gbox_advanced_other
            // 
            this.gbox_advanced_other.Controls.Add(this.checkBox1);
            this.gbox_advanced_other.Controls.Add(this.btn_FormatForTag);
            this.gbox_advanced_other.Controls.Add(this.btn_FormatForName);
            this.gbox_advanced_other.Controls.Add(this.chk_folderByArtist);
            this.gbox_advanced_other.Controls.Add(this.chk_MergePlaylists);
            this.gbox_advanced_other.Controls.Add(this.chk_CreatePlaylists);
            this.gbox_advanced_other.Location = new System.Drawing.Point(24, 208);
            this.gbox_advanced_other.Name = "gbox_advanced_other";
            this.gbox_advanced_other.Size = new System.Drawing.Size(456, 93);
            this.gbox_advanced_other.TabIndex = 35;
            this.gbox_advanced_other.TabStop = false;
            this.gbox_advanced_other.Text = "Other";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(212, 72);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(222, 16);
            this.checkBox1.TabIndex = 36;
            this.checkBox1.Text = "Manifest save to Tag (ID3 [JSON])";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // btn_FormatForTag
            // 
            this.btn_FormatForTag.Enabled = false;
            this.btn_FormatForTag.Location = new System.Drawing.Point(7, 52);
            this.btn_FormatForTag.Name = "btn_FormatForTag";
            this.btn_FormatForTag.Size = new System.Drawing.Size(163, 20);
            this.btn_FormatForTag.TabIndex = 35;
            this.btn_FormatForTag.Text = "Metadata Formatter (ID3)";
            this.btn_FormatForTag.UseVisualStyleBackColor = true;
            this.btn_FormatForTag.Visible = false;
            this.btn_FormatForTag.Click += new System.EventHandler(this.btn_FormatForTag_Click);
            // 
            // btn_FormatForName
            // 
            this.btn_FormatForName.Location = new System.Drawing.Point(7, 27);
            this.btn_FormatForName.Name = "btn_FormatForName";
            this.btn_FormatForName.Size = new System.Drawing.Size(163, 20);
            this.btn_FormatForName.TabIndex = 34;
            this.btn_FormatForName.Text = "Filename Formatter";
            this.btn_FormatForName.UseVisualStyleBackColor = true;
            this.btn_FormatForName.Click += new System.EventHandler(this.btn_FormatForName_Click);
            // 
            // chk_MergePlaylists
            // 
            this.chk_MergePlaylists.AutoSize = true;
            this.chk_MergePlaylists.Checked = true;
            this.chk_MergePlaylists.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_MergePlaylists.Location = new System.Drawing.Point(212, 30);
            this.chk_MergePlaylists.Name = "chk_MergePlaylists";
            this.chk_MergePlaylists.Size = new System.Drawing.Size(184, 16);
            this.chk_MergePlaylists.TabIndex = 33;
            this.chk_MergePlaylists.Text = "Merge SoundCloud playlists";
            this.chk_MergePlaylists.UseVisualStyleBackColor = true;
            // 
            // gbox_advanced_enginebehaviour
            // 
            this.gbox_advanced_enginebehaviour.Controls.Add(this.chk_replaceIllegalCharacters);
            this.gbox_advanced_enginebehaviour.Controls.Add(this.nudConcurrency);
            this.gbox_advanced_enginebehaviour.Controls.Add(this.concurrency);
            this.gbox_advanced_enginebehaviour.Location = new System.Drawing.Point(24, 128);
            this.gbox_advanced_enginebehaviour.Name = "gbox_advanced_enginebehaviour";
            this.gbox_advanced_enginebehaviour.Size = new System.Drawing.Size(456, 74);
            this.gbox_advanced_enginebehaviour.TabIndex = 34;
            this.gbox_advanced_enginebehaviour.TabStop = false;
            this.gbox_advanced_enginebehaviour.Text = "Download Behaviour";
            // 
            // SoundcloudSyncMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 395);
            this.Controls.Add(this.tabControl_general);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SoundcloudSyncMainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnl_convert.ResumeLayout(false);
            this.pnl_convert.PerformLayout();
            this.gbox_syncMethod.ResumeLayout(false);
            this.gbox_syncMethod.PerformLayout();
            this.gbox_downMethod.ResumeLayout(false);
            this.gbox_downMethod.PerformLayout();
            this.gbox_url.ResumeLayout(false);
            this.gbox_url.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbox_advanced_conversion.ResumeLayout(false);
            this.gbox_advanced_conversion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConcurrency)).EndInit();
            this.gbox_localdir.ResumeLayout(false);
            this.gbox_localdir.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabControl_general.ResumeLayout(false);
            this.tabPage_BasicOptions.ResumeLayout(false);
            this.tabPage_AdvancedOptions.ResumeLayout(false);
            this.gbox_advanced_other.ResumeLayout(false);
            this.gbox_advanced_other.PerformLayout();
            this.gbox_advanced_enginebehaviour.ResumeLayout(false);
            this.gbox_advanced_enginebehaviour.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox url;
        private System.Windows.Forms.Button syncButton;
        private System.Windows.Forms.TextBox directoryPath;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.RadioButton userPlaylists;
        private System.Windows.Forms.RadioButton playlistRadio;
        private System.Windows.Forms.RadioButton favoritesRadio;
        private System.Windows.Forms.RadioButton artistRadio;
        private System.Windows.Forms.CheckBox chk_folderByArtist;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.CheckBox chk_highquality;
        private System.Windows.Forms.ToolTip tt_qualityExplanation;
        private System.Windows.Forms.CheckBox chk_convertToMp3;
        private System.Windows.Forms.CheckBox chk_replaceIllegalCharacters;
        private System.Windows.Forms.Label lbl_exclude;
        private System.Windows.Forms.CheckBox chk_excl_m4a;
        private System.Windows.Forms.CheckBox chk_exl_aac;
        private System.Windows.Forms.Panel pnl_convert;
        private System.Windows.Forms.RadioButton rbttn_twoWay;
        private System.Windows.Forms.RadioButton rbttn_oneWay;
        private System.Windows.Forms.GroupBox gbox_syncMethod;
        private System.Windows.Forms.GroupBox gbox_downMethod;
        private System.Windows.Forms.GroupBox gbox_url;
        private System.Windows.Forms.GroupBox gbox_advanced_conversion;
        private System.Windows.Forms.GroupBox gbox_localdir;
        private System.Windows.Forms.RadioButton trackRadio;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.Label concurrency;
        private System.Windows.Forms.NumericUpDown nudConcurrency;
        private System.Windows.Forms.ListBox lb_progressOfTracks;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripMenuItem clientIDToolStripMenuItem;
        private System.Windows.Forms.CheckBox chk_CreatePlaylists;
        private System.Windows.Forms.ToolStripMenuItem configurationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem config1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem config2ToolStripMenuItem;
        private System.Windows.Forms.CheckBox chk_configActive;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbl_currentConfig;
        private System.Windows.Forms.ToolStripMenuItem config3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem config4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem config5ToolStripMenuItem;
        private System.Windows.Forms.Label lbl_configurationPrefix;
        private System.Windows.Forms.TabControl tabControl_general;
        private System.Windows.Forms.TabPage tabPage_BasicOptions;
        private System.Windows.Forms.TabPage tabPage_AdvancedOptions;
        private System.Windows.Forms.CheckBox chk_MergePlaylists;
        private System.Windows.Forms.GroupBox gbox_advanced_other;
        private System.Windows.Forms.GroupBox gbox_advanced_enginebehaviour;
        private System.Windows.Forms.Button btn_FormatForTag;
        private System.Windows.Forms.Button btn_FormatForName;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
    }
}