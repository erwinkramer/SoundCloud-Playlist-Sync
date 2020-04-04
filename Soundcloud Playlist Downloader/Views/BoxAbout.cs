using Soundcloud_Playlist_Downloader.Language;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader.Views
{
    internal partial class BoxAbout : Form
    {
        public BoxAbout()
        {
            InitializeComponent();
            Text = $"About {AssemblyTitle}";

            var link = new LinkLabel.Link();
            var link2 = new LinkLabel.Link();
            var link3 = new LinkLabel.Link();
            var link4 = new LinkLabel.Link();
            var link6 = new LinkLabel.Link();
            var link7 = new LinkLabel.Link();
            var link8 = new LinkLabel.Link();
            var link9 = new LinkLabel.Link();
            var link10 = new LinkLabel.Link();

            link.LinkData = "https://github.com/StephenCasella/SoundCloud-Playlist-Sync/releases";
            link_github2.Links.Add(link);
            link2.LinkData = "https://github.com/erwinkramer/SoundCloud-Playlist-Sync/releases";
            link_github.Links.Add(link2);
            link9.LinkData = "https://github.com/valentingiraud";
            link_github_contributor_1.Links.Add(link9);
            link3.LinkData = "https://github.com/mono/taglib-sharp/releases";
            link_taglib.Links.Add(link3);
            link4.LinkData = "https://github.com/JamesNK/Newtonsoft.Json/releases";
            link_JSON.Links.Add(link4);
            link6.LinkData = "https://github.com/naudio/NAudio";
            link_nAudio.Links.Add(link6);
            link7.LinkData = "https://github.com/Corey-M/NAudio.Lame";
            link_naudioLame.Links.Add(link7);
            link8.LinkData = "https://htmlagilitypack.codeplex.com";
            link_HtmlAgilityPack.Links.Add(link8);
            link10.LinkData = "https://github.com/HongSic";
            linkLabel1.Links.Add(link10); linkLabel2.Links.Add(link10); linkLabel3.Links.Add(link10);
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
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
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void link_github_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void link_taglib_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void link_JSON_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void link_postsharp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void link_HtmlAgilityPack_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void link_github_contributor_1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void linkLabel1_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = e.Link.LinkData as string,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        internal void LoadLanguage()
        {
            Text = LanguageManager.Language["STR_ABOUT_TITLE"];
            lbl_info1.Text = LanguageManager.Language["STR_ABOUT_INFO"];
            lbl_copy.Text = LanguageManager.Language["STR_ABOUT_COPYRIGHT"];
            richTextBox1.Text = LanguageManager.Language["STR_ABOUT_LICENSE"].Replace("\\n", "\r\n");
            okButton.Text = LanguageManager.Language["STR_ABOUT_OK"];
            License.Text = LanguageManager.Language["STR_ABOUT_TAB1"];
            Projectwebsites.Text = LanguageManager.Language["STR_ABOUT_TAB2"];
            Translators.Text = LanguageManager.Language["STR_ABOUT_TAB3"];
            Libraries.Text = LanguageManager.Language["STR_ABOUT_TAB4"];
            label4.Text = LanguageManager.Language["STR_ABOUT_PW_CM"] + ":";
            label3.Text = LanguageManager.Language["STR_ABOUT_PW_OD"] + ":";
            label6.Text = LanguageManager.Language["STR_ABOUT_PW_CON"] + ":";
            label7.Text = LanguageManager.Language["STR_ABOUT_TRANSIMPL"] + ":";
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}