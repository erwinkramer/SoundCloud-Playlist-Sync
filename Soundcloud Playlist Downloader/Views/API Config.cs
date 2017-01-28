using Soundcloud_Playlist_Downloader.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            DownloadUtils.ClientId = txt_alteredClientID.Text.Trim();
            txt_CurrentClientID.Text = DownloadUtils.ClientId;
            Hide();
        }

        private void API_Config_Load(object sender, EventArgs e)
        {
            txt_CurrentClientID.Text = DownloadUtils.ClientId;
        }

        private void API_Config_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
