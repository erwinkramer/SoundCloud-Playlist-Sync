using Soundcloud_Playlist_Downloader.Language;
using Soundcloud_Playlist_Downloader.Utils;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader.Views
{
    public partial class API_Config : Form
    {
        public ClientIDsUtils ClientIDsUtil;
        public API_Config(ClientIDsUtils clientIDsUtil)
        {
            ClientIDsUtil = clientIDsUtil;
            InitializeComponent();
        }

        private void bttn_save_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(txt_CustomClientID.Text))
            {
                ClientIDsUtil.ClientIdCustom = txt_CustomClientID.Text.Trim();
                txt_CustomClientID.Text = ClientIDsUtil.ClientIdCustom;
            }
            ClientIDsUtil.OAuthToken = txt_OAuthToken.Text.Trim();

            Hide();
        }

        private void API_Config_Load(object sender, EventArgs e)
        {
            txt_stockClientID.Text = ClientIDsUtil.ClientId1;
            txt_stockClientID2.Text = ClientIDsUtil.ClientId2;         
            txt_CustomClientID.Text = ClientIDsUtil.ClientIdCustom;
            switch (ClientIDsUtil.ClientIdCurrentName)
            {
                case "clientID1":
                    rbutton_clientid1.Checked = true;
                    break;
                case "clientID2":
                    rbutton_clientid2.Checked = true;
                    break;
                case "clientIDcustom":
                    rbutton_clientidcustom.Checked = true;
                    break;
                default:
                    break;
            }

            if (ClientIDsUtil.OAuthToken == null)
                ClientIDsUtil.OAuthToken = ClientIDsUtil.DefaultOAuthToken;
            txt_OAuthToken.Text = ClientIDsUtil.OAuthToken;
        }

        private void API_Config_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void txt_stockClientID_TextChanged(object sender, EventArgs e)
        {

        }

        private void rbutton_clientid2_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
                return;
            ClientIDsUtil.ClientIdCurrentName = "clientID2";
        }

        private void rbutton_clientidcustom_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
                return;
            ClientIDsUtil.ClientIdCurrentName = "clientIDcustom";
        }

        private void rbutton_clientid1_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
                return;
            ClientIDsUtil.ClientIdCurrentName = "clientID1";
        }

        internal void LoadLanguage()
        {
            this.Text = LanguageManager.Language["STR_APICONFIG_TITLE"];
            this.lbl_info.Text = LanguageManager.Language["STR_APICONFIG_DESC"];
            this.label5.Text = LanguageManager.Language["STR_APICONFIG_ACTIVE"];
            this.rbutton_clientid1.Text = LanguageManager.Language["STR_APICONFIG_CLIENTID"] + " 1:";
            this.rbutton_clientid2.Text = LanguageManager.Language["STR_APICONFIG_CLIENTID"] + " 2:";
            this.rbutton_clientidcustom.Text = LanguageManager.Language["STR_APICONFIG_CUSTOMID"];
            this.bttn_save.Text = LanguageManager.Language["STR_APICONFIG_SAVE"];
        }
    }
}
