using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SC_SYNC_Base.JsonObjects;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Language;
using Soundcloud_Playlist_Downloader.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Soundcloud_Playlist_Downloader.Views
{
    public partial class NameFormater : Form
    {
        Track track_sample;

        public NameFormater(FilesystemUtils filesystemUtils)
        {
            InitializeComponent();
            FormatBefore = Format = textBox1.Text = "%user% - %title% %quality%";
            Init();
        }
        [Obsolete]
        public NameFormater(params int[] DisableTagIndex)
        {
            InitializeComponent();
            FormatBefore = Format = textBox1.Text = "%user% - %title% %quality%";
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
            base.Text = LanguageManager.Language["STR_FORMAT_FILE_TITLE"];
            button3.Text = LanguageManager.Language["STR_FORMAT_FILE_SAMTR"] + " ▼";
            button1.Text = LanguageManager.Language["STR_FORMAT_FILE_FOROP"] + " ▼";
            checkBox1.Text = LanguageManager.Language["STR_FORMAT_FILE_FROMID3"];
            groupBox1.Text = LanguageManager.Language["STR_FORMAT_FILE_PREV"];
            button2.Text = LanguageManager.Language["STR_FORMAT_FILE_SAVE"];

            track_sample = JsonConvert.DeserializeObject<Track>(SyncSetting.LoadSettingFromConfig("SampleTrack1"));
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

        /// <summary>
        /// Assume Replace illegal characters to make it a nice preview. No need to make it based on the current option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = FilesystemUtils.BuildName(textBox1.Text, track_sample, true);
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
            if(e.ClickedItem.Name == "toolStripMenuItem1") track_sample = JsonConvert.DeserializeObject<Track>(SyncSetting.settings.Get("SampleTrack1"));
            else if (e.ClickedItem.Name == "toolStripMenuItem2") track_sample = JsonConvert.DeserializeObject<Track>(SyncSetting.settings.Get("SampleTrack2"));
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
                    new Description("%title%", LanguageManager.Language["STR_FORMAT_FILE_FTITLE"]),
                    new Description("%genre%", LanguageManager.Language["STR_FORMAT_FILE_FGENRE"]),
                    new Description("%index%", LanguageManager.Language["STR_FORMAT_FILE_FINDEX"]),
                    new Description("%user%", LanguageManager.Language["STR_FORMAT_FILE_FUNAME"]),
                    new Description("%date%", LanguageManager.Language["STR_FORMAT_FILE_FCDATE"]+" (YY-MM-DD)"),
                    new Description("%time%", LanguageManager.Language["STR_FORMAT_FILE_FCTIME"]+" (HH.mm.ss)"),
                    new Description("%ext%", LanguageManager.Language["STR_FORMAT_FILE_FEXT"]),
                    new Description("%quality%", LanguageManager.Language["STR_FORMAT_FILE_FHD"]),
                    new Description("%label_name%", LanguageManager.Language["STR_FORMAT_FILE_FNAME"]),
                    new Description("%desc%",LanguageManager.Language["STR_FORMAT_FILE_FDESC"]),
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
                    new Description("%title%", LanguageManager.Language["STR_FORMAT_FILE_FTITLE"], track.Title),
                    new Description("%genre%", LanguageManager.Language["STR_FORMAT_FILE_FGENRE"], track.Artist),
                    new Description("%index%", LanguageManager.Language["STR_FORMAT_FILE_FINDEX"], (track.IndexFromSoundcloud + 1).ToString()),
                    new Description("%user%", LanguageManager.Language["STR_FORMAT_FILE_FUNAME"], track.genre),
                    new Description("%date%", LanguageManager.Language["STR_FORMAT_FILE_FCDATE"]+" (YY-MM-DD)", date),
                    new Description("%time%", LanguageManager.Language["STR_FORMAT_FILE_FCTIME"]+" (HH.mm.ss)", time),
                    new Description("%ext%", LanguageManager.Language["STR_FORMAT_FILE_FEXT"], track.original_format),
                    new Description("%quality%", LanguageManager.Language["STR_FORMAT_FILE_FHD"], track.IsHD ? "(HQ)" : null),
                    new Description("%label_name%", LanguageManager.Language["STR_FORMAT_FILE_FNAME"], track.label_name),
                    new Description("%desc%", LanguageManager.Language["STR_FORMAT_FILE_FDESC"], track.description),
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

        public override bool Equals(object obj)
        {
            if (obj is DropDownItem d) return d.Item.Equals(Item);
            return false;
        }
    }
}
