using Soundcloud_Playlist_Downloader.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader.Views
{
    public partial class API_Config : Form
    {
        public API_Config()
        {
            InitializeComponent();
        }

        private void bttn_save_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(txt_alteredClientID.Text))
            {
                DownloadUtils.ClientIdCustom = txt_alteredClientID.Text.Trim();
                txt_CustomClientID.Text = DownloadUtils.ClientIdCustom;
            }
            Hide();
        }

        private void API_Config_Load(object sender, EventArgs e)
        {
            txt_stockClientID.Text = DownloadUtils.ClientId1;
            txt_stockClientID2.Text = DownloadUtils.ClientId2;         
            txt_CustomClientID.Text = DownloadUtils.ClientIdCustom;
            txt_alteredClientID.Text = DownloadUtils.ClientIdCustom;
            switch (DownloadUtils.ClientIdCurrentName)
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
        }

        private void API_Config_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void lbl_ClientID_Click(object sender, EventArgs e)
        {

        }

      

        private void txt_stockClientID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_CustomClientID_TextChanged(object sender, EventArgs e)
        {

        }

        private void rbutton_clientid2_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
                return;
            DownloadUtils.ClientIdCurrentName = "clientID2";
        }

        private void rbutton_clientidcustom_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
                return;
            DownloadUtils.ClientIdCurrentName = "clientIDcustom";
        }

        private void rbutton_clientid1_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
                return;
            DownloadUtils.ClientIdCurrentName = "clientID1";
        }
    }
}
