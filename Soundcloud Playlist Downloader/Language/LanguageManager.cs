//Language Manager class ⓒ Author by HongSic
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundcloud_Playlist_Downloader.Language
{
    public class LanguageManager
    {
        public static LanguageManager Language = GetDefault();

        Dictionary<string, string> lng = new Dictionary<string, string>();
        public LanguageManager() {}
        public LanguageManager(string[] Language)
        {
            for (int i = 0; i < Language.Length; i++)
            {
                if (!string.IsNullOrEmpty(Language[i]) &&
                    (Language[i][0] >= 'A' && Language[i][0] <= 'Z'))
                {
                    int index = Language[i].IndexOf('=');
                    if (index > 0)
                    {
                        string key = Language[i].Remove(index);
                        if (!Exist(key)) lng.Add(key, Language[i].Substring(index + 1).Replace("\\n", "\n"));
                    }
                }
            }
        }

        public string this[string Key] { get { return lng.ContainsKey(Key) ? lng[Key] : Key; }}
        public bool Exist(string Key) { return lng.ContainsKey(Key); }
        public int Count { get { return lng.Count; } }

        //Language: English (Defalut)
        //Author: HongSic(HSKernel)
        public static LanguageManager GetDefault()
        {
            LanguageManager m = new LanguageManager();
            //Main Area
            m.lng.Add("STR_MAIN_TITLE_STABLE", "SoundCloud Playlist Sync {0} Stable");
            m.lng.Add("STR_MAIN_MENU_CONFIGS", "Configurations");
            m.lng.Add("STR_MAIN_MENU_CONFIG", "Config");
            m.lng.Add("STR_MAIN_MENU_CLIENT", "API Config");
            m.lng.Add("STR_MAIN_MENU_UPDATE", "Update");
            m.lng.Add("STR_MAIN_MENU_ABOUT", "About");
            m.lng.Add("STR_MAIN_MENU_LANG", "Language");
            m.lng.Add("STR_MAIN_STATUS_READY", "Ready");
            m.lng.Add("STR_MAIN_STATUS_COMPLETE", "Completed");
            m.lng.Add("STR_MAIN_STATUS_DOWNLOAD", "Synchronizing... {0} of {1} songs downloaded.");


            m.lng.Add("STR_MAIN_STATUS_CHECK", "Ready");

            //API Area
            m.lng.Add("STR_APICONFIG_TITLE", "API Config");
            m.lng.Add("STR_APICONFIG_DESC", "This setting changes your client ID.\nIf the API returns unauthorized it will be likely that the client ID is not working anymore.\nCreate a new ID by signing up for an application under the button.");
            m.lng.Add("STR_APICONFIG_ACTIVE", "Active");
            m.lng.Add("STR_APICONFIG_CLIENTID", "Client ID");
            m.lng.Add("STR_APICONFIG_CUSTOMID", "Custom ID");
            m.lng.Add("STR_APICONFIG_SAVE", "Save settings");
            m.lng.Add("STR_APICONFIG_LINK", "Click here to create new ID");
            return m;
        }
    }
}
