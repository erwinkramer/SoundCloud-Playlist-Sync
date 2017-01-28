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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_stockClientID = new System.Windows.Forms.TextBox();
            this.txt_CurrentClientID = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // bttn_save
            // 
            this.bttn_save.Location = new System.Drawing.Point(12, 226);
            this.bttn_save.Name = "bttn_save";
            this.bttn_save.Size = new System.Drawing.Size(260, 23);
            this.bttn_save.TabIndex = 0;
            this.bttn_save.Text = "Save settings";
            this.bttn_save.UseVisualStyleBackColor = true;
            this.bttn_save.Click += new System.EventHandler(this.bttn_save_Click);
            // 
            // txt_alteredClientID
            // 
            this.txt_alteredClientID.Location = new System.Drawing.Point(89, 148);
            this.txt_alteredClientID.Name = "txt_alteredClientID";
            this.txt_alteredClientID.Size = new System.Drawing.Size(183, 20);
            this.txt_alteredClientID.TabIndex = 1;
            // 
            // lbl_ClientID
            // 
            this.lbl_ClientID.AutoSize = true;
            this.lbl_ClientID.Location = new System.Drawing.Point(16, 151);
            this.lbl_ClientID.Name = "lbl_ClientID";
            this.lbl_ClientID.Size = new System.Drawing.Size(73, 13);
            this.lbl_ClientID.TabIndex = 2;
            this.lbl_ClientID.Text = "Alter client ID:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(263, 81);
            this.label1.TabIndex = 3;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.label2.Location = new System.Drawing.Point(9, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Stock client ID:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Current client ID:";
            // 
            // txt_stockClientID
            // 
            this.txt_stockClientID.Location = new System.Drawing.Point(89, 87);
            this.txt_stockClientID.Name = "txt_stockClientID";
            this.txt_stockClientID.ReadOnly = true;
            this.txt_stockClientID.Size = new System.Drawing.Size(183, 20);
            this.txt_stockClientID.TabIndex = 7;
            this.txt_stockClientID.Text = "93a4fae1bd98b84c9b4f6bf1cc838b4f";
            // 
            // txt_CurrentClientID
            // 
            this.txt_CurrentClientID.Location = new System.Drawing.Point(89, 113);
            this.txt_CurrentClientID.Name = "txt_CurrentClientID";
            this.txt_CurrentClientID.ReadOnly = true;
            this.txt_CurrentClientID.Size = new System.Drawing.Size(183, 20);
            this.txt_CurrentClientID.TabIndex = 8;
            // 
            // API_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.txt_CurrentClientID);
            this.Controls.Add(this.txt_stockClientID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_stockClientID;
        private System.Windows.Forms.TextBox txt_CurrentClientID;
    }
}