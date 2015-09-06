using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader
{
    partial class box_about : Form
    {
        public box_about()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);

            LinkLabel.Link link = new LinkLabel.Link();
            LinkLabel.Link link2 = new LinkLabel.Link();
            LinkLabel.Link link3 = new LinkLabel.Link();
            LinkLabel.Link link4 = new LinkLabel.Link();
            LinkLabel.Link link5 = new LinkLabel.Link();
            LinkLabel.Link link6 = new LinkLabel.Link();

            link.LinkData = "https://github.com/StephenCasella/SoundCloud-Playlist-Sync/releases";
            link_github2.Links.Add(link);
            link2.LinkData = "https://github.com/erwinkramer/SoundCloud-Playlist-Sync/releases";
            link_github.Links.Add(link2);
            link3.LinkData = "https://github.com/mono/taglib-sharp/releases";
            link_taglib.Links.Add(link3);
            link4.LinkData = "https://github.com/JamesNK/Newtonsoft.Json/releases";
            link_JSON.Links.Add(link4);
            link5.LinkData = "https://www.postsharp.net/downloads";
            link_postsharp.Links.Add(link5);
            link6.LinkData = "https://github.com/naudio/NAudio";
            link_nAudio.Links.Add(link6);

        }

        #region Assembly Attribute Accessors
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
        #endregion

        private void labelProductName_Click(object sender, EventArgs e)
        {

        }

        private void labelVersion_Click(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void AboutBox1_Load(object sender, EventArgs e)
        {

        }

        private void AboutBox1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblGitHub_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void link_github2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void link_github_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void link_taglib_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void link_JSON_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void link_postsharp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }
    }
}
