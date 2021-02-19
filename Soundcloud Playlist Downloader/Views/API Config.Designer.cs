using Soundcloud_Playlist_Downloader.Language;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(API_Config));
            this.bttn_save = new System.Windows.Forms.Button();
            this.txt_CustomClientID = new System.Windows.Forms.TextBox();
            this.txt_stockClientID = new System.Windows.Forms.TextBox();
            this.txt_stockClientID2 = new System.Windows.Forms.TextBox();
            this.rbutton_clientid1 = new System.Windows.Forms.RadioButton();
            this.rbutton_clientid2 = new System.Windows.Forms.RadioButton();
            this.rbutton_clientidcustom = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txt_OAuthToken = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_info = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bttn_save
            // 
            this.bttn_save.Location = new System.Drawing.Point(6, 258);
            this.bttn_save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bttn_save.Name = "bttn_save";
            this.bttn_save.Size = new System.Drawing.Size(337, 26);
            this.bttn_save.TabIndex = 0;
            this.bttn_save.Text = "Save settings";
            this.bttn_save.UseVisualStyleBackColor = true;
            this.bttn_save.Click += new System.EventHandler(this.bttn_save_Click);
            // 
            // txt_CustomClientID
            // 
            this.txt_CustomClientID.Location = new System.Drawing.Point(128, 184);
            this.txt_CustomClientID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_CustomClientID.Name = "txt_CustomClientID";
            this.txt_CustomClientID.Size = new System.Drawing.Size(217, 23);
            this.txt_CustomClientID.TabIndex = 1;
            // 
            // txt_stockClientID
            // 
            this.txt_stockClientID.Location = new System.Drawing.Point(128, 126);
            this.txt_stockClientID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_stockClientID.Name = "txt_stockClientID";
            this.txt_stockClientID.ReadOnly = true;
            this.txt_stockClientID.Size = new System.Drawing.Size(217, 23);
            this.txt_stockClientID.TabIndex = 7;
            this.txt_stockClientID.TextChanged += new System.EventHandler(this.txt_stockClientID_TextChanged);
            // 
            // txt_stockClientID2
            // 
            this.txt_stockClientID2.Location = new System.Drawing.Point(128, 155);
            this.txt_stockClientID2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_stockClientID2.Name = "txt_stockClientID2";
            this.txt_stockClientID2.ReadOnly = true;
            this.txt_stockClientID2.Size = new System.Drawing.Size(217, 23);
            this.txt_stockClientID2.TabIndex = 10;
            // 
            // rbutton_clientid1
            // 
            this.rbutton_clientid1.AutoSize = true;
            this.rbutton_clientid1.Location = new System.Drawing.Point(10, 128);
            this.rbutton_clientid1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbutton_clientid1.Name = "rbutton_clientid1";
            this.rbutton_clientid1.Size = new System.Drawing.Size(82, 19);
            this.rbutton_clientid1.TabIndex = 11;
            this.rbutton_clientid1.TabStop = true;
            this.rbutton_clientid1.Text = "Client ID 1:";
            this.rbutton_clientid1.UseVisualStyleBackColor = true;
            this.rbutton_clientid1.CheckedChanged += new System.EventHandler(this.rbutton_clientid1_CheckedChanged);
            // 
            // rbutton_clientid2
            // 
            this.rbutton_clientid2.AutoSize = true;
            this.rbutton_clientid2.Location = new System.Drawing.Point(10, 158);
            this.rbutton_clientid2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbutton_clientid2.Name = "rbutton_clientid2";
            this.rbutton_clientid2.Size = new System.Drawing.Size(82, 19);
            this.rbutton_clientid2.TabIndex = 12;
            this.rbutton_clientid2.TabStop = true;
            this.rbutton_clientid2.Text = "Client ID 2:";
            this.rbutton_clientid2.UseVisualStyleBackColor = true;
            this.rbutton_clientid2.CheckedChanged += new System.EventHandler(this.rbutton_clientid2_CheckedChanged);
            // 
            // rbutton_clientidcustom
            // 
            this.rbutton_clientidcustom.AutoSize = true;
            this.rbutton_clientidcustom.Location = new System.Drawing.Point(10, 186);
            this.rbutton_clientidcustom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbutton_clientidcustom.Name = "rbutton_clientidcustom";
            this.rbutton_clientidcustom.Size = new System.Drawing.Size(84, 19);
            this.rbutton_clientidcustom.TabIndex = 13;
            this.rbutton_clientidcustom.TabStop = true;
            this.rbutton_clientidcustom.Text = "Custom ID:";
            this.rbutton_clientidcustom.UseVisualStyleBackColor = true;
            this.rbutton_clientidcustom.CheckedChanged += new System.EventHandler(this.rbutton_clientidcustom_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 106);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Active:";
            // 
            // txt_OAuthToken
            // 
            this.txt_OAuthToken.Location = new System.Drawing.Point(128, 228);
            this.txt_OAuthToken.Name = "txt_OAuthToken";
            this.txt_OAuthToken.Size = new System.Drawing.Size(215, 23);
            this.txt_OAuthToken.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 231);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 16;
            this.label2.Text = "OAuth token";
            // 
            // lbl_info
            // 
            this.lbl_info.Location = new System.Drawing.Point(6, 9);
            this.lbl_info.Name = "lbl_info";
            this.lbl_info.Size = new System.Drawing.Size(330, 97);
            this.lbl_info.TabIndex = 17;
            this.lbl_info.Text = resources.GetString("lbl_info.Text");
            // 
            // API_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 297);
            this.Controls.Add(this.lbl_info);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_OAuthToken);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rbutton_clientidcustom);
            this.Controls.Add(this.rbutton_clientid2);
            this.Controls.Add(this.rbutton_clientid1);
            this.Controls.Add(this.txt_stockClientID2);
            this.Controls.Add(this.txt_stockClientID);
            this.Controls.Add(this.txt_CustomClientID);
            this.Controls.Add(this.bttn_save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "API_Config";
            this.Text = "API Config";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.API_Config_FormClosing);
            this.Load += new System.EventHandler(this.API_Config_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bttn_save;
        private System.Windows.Forms.TextBox txt_CustomClientID;
        private System.Windows.Forms.TextBox txt_stockClientID;
        private System.Windows.Forms.TextBox txt_stockClientID2;
        private System.Windows.Forms.RadioButton rbutton_clientid1;
        private System.Windows.Forms.RadioButton rbutton_clientid2;
        private System.Windows.Forms.RadioButton rbutton_clientidcustom;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txt_OAuthToken;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_info;
    }
}