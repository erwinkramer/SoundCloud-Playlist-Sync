namespace Soundcloud_Playlist_Downloader.Views
{
    partial class API_Config
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(API_Config));
            this.bttn_save = new System.Windows.Forms.Button();
            this.txt_alteredClientID = new System.Windows.Forms.TextBox();
            this.lbl_ClientID = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_stockClientID = new System.Windows.Forms.TextBox();
            this.txt_CustomClientID = new System.Windows.Forms.TextBox();
            this.txt_stockClientID2 = new System.Windows.Forms.TextBox();
            this.rbutton_clientid1 = new System.Windows.Forms.RadioButton();
            this.rbutton_clientid2 = new System.Windows.Forms.RadioButton();
            this.rbutton_clientidcustom = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bttn_save
            // 
            this.bttn_save.Location = new System.Drawing.Point(12, 226);
            this.bttn_save.Name = "bttn_save";
            this.bttn_save.Size = new System.Drawing.Size(271, 23);
            this.bttn_save.TabIndex = 0;
            this.bttn_save.Text = "Save settings";
            this.bttn_save.UseVisualStyleBackColor = true;
            this.bttn_save.Click += new System.EventHandler(this.bttn_save_Click);
            // 
            // txt_alteredClientID
            // 
            this.txt_alteredClientID.Location = new System.Drawing.Point(100, 185);
            this.txt_alteredClientID.Name = "txt_alteredClientID";
            this.txt_alteredClientID.Size = new System.Drawing.Size(183, 20);
            this.txt_alteredClientID.TabIndex = 1;
            // 
            // lbl_ClientID
            // 
            this.lbl_ClientID.AutoSize = true;
            this.lbl_ClientID.Location = new System.Drawing.Point(15, 188);
            this.lbl_ClientID.Name = "lbl_ClientID";
            this.lbl_ClientID.Size = new System.Drawing.Size(79, 13);
            this.lbl_ClientID.TabIndex = 2;
            this.lbl_ClientID.Text = "Edit custom ID:";
            this.lbl_ClientID.Click += new System.EventHandler(this.lbl_ClientID_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(274, 81);
            this.label1.TabIndex = 3;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // txt_stockClientID
            // 
            this.txt_stockClientID.Location = new System.Drawing.Point(100, 107);
            this.txt_stockClientID.Name = "txt_stockClientID";
            this.txt_stockClientID.ReadOnly = true;
            this.txt_stockClientID.Size = new System.Drawing.Size(183, 20);
            this.txt_stockClientID.TabIndex = 7;
            this.txt_stockClientID.TextChanged += new System.EventHandler(this.txt_stockClientID_TextChanged);
            // 
            // txt_CustomClientID
            // 
            this.txt_CustomClientID.Location = new System.Drawing.Point(100, 159);
            this.txt_CustomClientID.Name = "txt_CustomClientID";
            this.txt_CustomClientID.ReadOnly = true;
            this.txt_CustomClientID.Size = new System.Drawing.Size(183, 20);
            this.txt_CustomClientID.TabIndex = 8;
            this.txt_CustomClientID.TextChanged += new System.EventHandler(this.txt_CustomClientID_TextChanged);
            // 
            // txt_stockClientID2
            // 
            this.txt_stockClientID2.Location = new System.Drawing.Point(100, 132);
            this.txt_stockClientID2.Name = "txt_stockClientID2";
            this.txt_stockClientID2.ReadOnly = true;
            this.txt_stockClientID2.Size = new System.Drawing.Size(183, 20);
            this.txt_stockClientID2.TabIndex = 10;
            // 
            // rbutton_clientid1
            // 
            this.rbutton_clientid1.AutoSize = true;
            this.rbutton_clientid1.Location = new System.Drawing.Point(18, 108);
            this.rbutton_clientid1.Name = "rbutton_clientid1";
            this.rbutton_clientid1.Size = new System.Drawing.Size(77, 17);
            this.rbutton_clientid1.TabIndex = 11;
            this.rbutton_clientid1.TabStop = true;
            this.rbutton_clientid1.Text = "Client ID 1:";
            this.rbutton_clientid1.UseVisualStyleBackColor = true;
            this.rbutton_clientid1.CheckedChanged += new System.EventHandler(this.rbutton_clientid1_CheckedChanged);
            // 
            // rbutton_clientid2
            // 
            this.rbutton_clientid2.AutoSize = true;
            this.rbutton_clientid2.Location = new System.Drawing.Point(18, 133);
            this.rbutton_clientid2.Name = "rbutton_clientid2";
            this.rbutton_clientid2.Size = new System.Drawing.Size(77, 17);
            this.rbutton_clientid2.TabIndex = 12;
            this.rbutton_clientid2.TabStop = true;
            this.rbutton_clientid2.Text = "Client ID 2:";
            this.rbutton_clientid2.UseVisualStyleBackColor = true;
            this.rbutton_clientid2.CheckedChanged += new System.EventHandler(this.rbutton_clientid2_CheckedChanged);
            // 
            // rbutton_clientidcustom
            // 
            this.rbutton_clientidcustom.AutoSize = true;
            this.rbutton_clientidcustom.Location = new System.Drawing.Point(18, 160);
            this.rbutton_clientidcustom.Name = "rbutton_clientidcustom";
            this.rbutton_clientidcustom.Size = new System.Drawing.Size(77, 17);
            this.rbutton_clientidcustom.TabIndex = 13;
            this.rbutton_clientidcustom.TabStop = true;
            this.rbutton_clientidcustom.Text = "Custom ID:";
            this.rbutton_clientidcustom.UseVisualStyleBackColor = true;
            this.rbutton_clientidcustom.CheckedChanged += new System.EventHandler(this.rbutton_clientidcustom_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Active:";
            // 
            // API_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 258);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rbutton_clientidcustom);
            this.Controls.Add(this.rbutton_clientid2);
            this.Controls.Add(this.rbutton_clientid1);
            this.Controls.Add(this.txt_stockClientID2);
            this.Controls.Add(this.txt_CustomClientID);
            this.Controls.Add(this.txt_stockClientID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_ClientID);
            this.Controls.Add(this.txt_alteredClientID);
            this.Controls.Add(this.bttn_save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "API_Config";
            this.Text = "API Config";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.API_Config_FormClosing);
            this.Load += new System.EventHandler(this.API_Config_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bttn_save;
        private System.Windows.Forms.TextBox txt_alteredClientID;
        private System.Windows.Forms.Label lbl_ClientID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_stockClientID;
        private System.Windows.Forms.TextBox txt_CustomClientID;
        private System.Windows.Forms.TextBox txt_stockClientID2;
        private System.Windows.Forms.RadioButton rbutton_clientid1;
        private System.Windows.Forms.RadioButton rbutton_clientid2;
        private System.Windows.Forms.RadioButton rbutton_clientidcustom;
        private System.Windows.Forms.Label label5;
    }
}