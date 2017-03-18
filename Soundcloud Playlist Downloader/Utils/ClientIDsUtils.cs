using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class ClientIDsUtils
    {
        public ClientIDsUtils()
        {

        }
        //OLD CLIENT_ID = "376f225bf427445fc4bfb6b99b72e0bf";
        //OLD key (not working anymore) should fix same reason as stated here: 
        //https://stackoverflow.com/questions/29914622/get-http-mp3-stream-from-every-song/30018216#30018216
        public string ClientId1
        {
            get
            {
                return ReadSetting("clientID1");
            }
            set
            {
                AddUpdateAppSettings("clientID1", value);
            }
        }
        public string ClientId2
        {
            get
            {
                return ReadSetting("clientID2");
            }
            set
            {
                AddUpdateAppSettings("clientID2", value);
            }
        }
        public string ClientIdCustom
        {
            get
            {
                return ReadSetting("clientIDcustom");
            }
            set
            {
                AddUpdateAppSettings("clientIDcustom", value);
            }
        }

        public string ClientIdCurrentName
        {
            get
            {
                return ReadSetting("clientIDcurrentSelected");
            }
            set
            {
                AddUpdateAppSettings("clientIDcurrentSelected", value);
            }
        }

        public string ClientIdCurrentValue
        {
            get
            {
                return ReadSetting(ReadSetting("clientIDcurrentSelected"));
            }
        }

        static string ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings[key] ?? "Not Found";
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
