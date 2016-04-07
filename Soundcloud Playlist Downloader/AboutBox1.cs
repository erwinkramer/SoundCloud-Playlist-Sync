using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader
{
    internal partial class BoxAbout : Form
    {
        public BoxAbout()
        {
            InitializeComponent();
            Text = string.Format("About {0}", AssemblyTitle);

            var link = new LinkLabel.Link();
            var link2 = new LinkLabel.Link();
            var link3 = new LinkLabel.Link();
            var link4 = new LinkLabel.Link();
            var link5 = new LinkLabel.Link();
            var link6 = new LinkLabel.Link();
            var link7 = new LinkLabel.Link();

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
            link7.LinkData = "https://github.com/Corey-M/NAudio.Lame";
            link_naudioLame.Links.Add(link7);
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof (AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute) attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void AboutBox1_Load(object sender, EventArgs e)
        {
        }

        private void AboutBox1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
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

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }
    }
}