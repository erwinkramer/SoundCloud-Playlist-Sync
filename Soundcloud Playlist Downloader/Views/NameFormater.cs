using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader.Views
{
    public partial class NameFormater : Form
    {
        Track track_sample;
        public NameFormater()
        {
            InitializeComponent();
            FormatBefore = Format = textBox1.Text = "%title%.%ext%";
            Init();
        }
        [Obsolete]
        public NameFormater(params int[] DisableTagIndex)
        {
            InitializeComponent();
            FormatBefore = Format = textBox1.Text = "%title%.%ext%";
            Init();

            for (int i = 0; DisableTagIndex != null && i < DisableTagIndex.Length; i++)
            {
                if (DisableTagIndex[i] > -1 && DisableTagIndex[i] < contextMenuStrip1.Items.Count)
                    contextMenuStrip1.Items[i].Enabled = false;
            }
        }
        public NameFormater(string FormatBefore)
        {
            InitializeComponent();
            Init();
            this.FormatBefore = Format = textBox1.Text = FormatBefore;
        }
        [Obsolete]
        public NameFormater(string FormatBefore, params int[] DisableTagIndex)
        {
            InitializeComponent();
            Init();
            this.FormatBefore = Format = textBox1.Text = FormatBefore;

            for (int i = 0; DisableTagIndex != null && i < DisableTagIndex.Length; i++)
            {
                if (DisableTagIndex[i] > -1 && DisableTagIndex[i] < contextMenuStrip1.Items.Count)
                    contextMenuStrip1.Items[i].Enabled = false;
            }
        }

        void Init()
        {
            track_sample = JsonConvert.DeserializeObject<Track>(Properties.Resources.SampleTrack1);
            RefreshTaglist();
        }
        void RefreshTaglist()
        {
            contextMenuStrip1.Items.Clear();
            Description[] desc = Description.GetDescriptions(track_sample);
            for (int i = 0; i < desc.Length; i++)
                contextMenuStrip1.Items.Add(new DropDownItem(desc[i], toolStripMenuItem_Click));
        }

        public string FormatBefore { get; set; }
        public string Format { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            //contextMenuStrip1.Show(button1, new Point(0, button1.Height));
            Point screenPoint = button1.PointToScreen(new Point(button1.Left, button1.Bottom));
            if (screenPoint.Y + contextMenuStrip1.Size.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                contextMenuStrip1.Show(button1, new Point(0, -contextMenuStrip1.Size.Height));
            }
            else
            {
                contextMenuStrip1.Show(button1, new Point(0, button1.Height));
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Format = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(sender is DropDownItem t && t.Item != null)
            {
                string NewText = textBox1.Text.Remove(textBox1.SelectionStart, textBox1.SelectionLength);
                textBox1.Text = NewText.Insert(textBox1.SelectionStart, t.Item.Tag);
            }
        }


        
        private void NameFormater_Load(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = FilesystemUtils.BuildName(textBox1.Text, track_sample, checkBox1.Checked);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Point screenPoint = button3.PointToScreen(new Point(button3.Left, button3.Bottom));
            if (screenPoint.Y + contextMenuStrip2.Size.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                contextMenuStrip2.Show(button3, new Point(0, -contextMenuStrip2.Size.Height));
            }
            else
            {
                contextMenuStrip2.Show(button3, new Point(0, button3.Height));
            }
        }

        private void contextMenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Name == "toolStripMenuItem1") track_sample = JsonConvert.DeserializeObject<Track>(Properties.Resources.SampleTrack1);
            else if (e.ClickedItem.Name == "toolStripMenuItem2") track_sample = JsonConvert.DeserializeObject<Track>(Properties.Resources.SampleTrack2);
            textBox1_TextChanged(sender, e);
            RefreshTaglist();
        }
    }

    class Description
    {
        public static Description[] GetDescriptions()
        {
            Description[] desc = new Description[]
            {
                    new Description("%title%", "Song Title"),
                    new Description("%genre%", "Song Genre"),
                    new Description("%index%", "Song Index"),
                    new Description("%user%", "User Name"),
                    new Description("%date%", "Creation date (YY-MM-DD)"),
                    new Description("%time%", "Creation time (HH.mm.ss)"),
                    new Description("%ext%", "Song Extension (Format)"),
                    new Description("%quality%", "Song is HD quality"),
                    new Description("%label_name%", "Title (Name)"),
                    new Description("%desc%", "Song Description"),
            };
            return desc;
        }
        public static Description[] GetDescriptions(Track track)
        {
            string date = null, time = null;
            try { date = DateTime.Parse(track.created_at).ToString("yyyy-MM-dd"); } catch { }
            try { time = DateTime.Parse(track.created_at).ToString("HH.mm.ss"); } catch { }
            Description[] desc = new Description[]
            {
                    new Description("%title%", "Song Title", track.Title),
                    new Description("%user%", "User Name", track.Artist),
                    new Description("%index%", "Song Index", (track.IndexFromSoundcloud + 1).ToString()),
                    new Description("%genre%", "Song Genre", track.genre),
                    new Description("%date%", "Creation date (YY-MM-DD)", date),
                    new Description("%time%", "Creation time (HH.mm.ss)", time),
                    new Description("%ext%", "Song Extension (Format)", track.original_format),
                    new Description("%quality%", "Song is HD quality", track.IsHD ? "(HQ)" : null),
                    new Description("%label_name%", "Title (Name)", track.label_name),
                    new Description("%desc%", "Song Description", track.description),
            };
            return desc;
        }

        public Description(string Tag, string Desc, string Sample = null) { this.Tag = Tag; this.Desc = Desc; this.Sample = Sample; }
        public string Tag;
        public string Desc;
        private string Sample;

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Sample) ?
                string.Format("[{0}] {1}", Tag, Desc) :
                string.Format("[{0}] {1} {2}", Tag, Desc, "{"+ CutString(Sample, 30) + "}");
        }
        public static string CutString(string Text, int MaxLength)
        {
            string tmp = Text.Replace("\n", "").Replace("\r", "");
            return tmp.Length > MaxLength ? tmp.Remove(MaxLength) + "..." : tmp;
        }
        public override bool Equals(object obj)
        {
            if (obj is Description d) return d.Tag == Tag && d.Desc == Desc;
            return false;
        }
    }

    class DropDownItem : ToolStripMenuItem
    {
        private Description _Item;
        public DropDownItem(Description Item, EventHandler Handler) : base(Item.ToString(), null, Handler) { this.Item = Item; }

        public Description Item
        {
            get { return _Item; }
            set
            {
                _Item = value;
                Invalidate();
            }
        }

        /*
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString(Item.ToString(), Font, new SolidBrush(ForeColor), new Point(0, 0));
            //g.DrawString("김세훈", Font, new SolidBrush(ForeColor), new Point(0, 0));
        }
        */

        public override bool Equals(object obj)
        {
            if (obj is DropDownItem d) return d.Item.Equals(Item);
            return false;
        }
    }
}
