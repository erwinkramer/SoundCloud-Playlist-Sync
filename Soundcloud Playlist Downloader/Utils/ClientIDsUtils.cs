using SC_SYNC_Base.JsonObjects;
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
                return SyncSetting.settings.Get("clientID1");
            }
            set
            {
                SyncSetting.settings.Set("clientID1", value);
            }
        }
        public string ClientId2
        {
            get
            {
                return SyncSetting.settings.Get("clientID2");
            }
            set
            {
                SyncSetting.settings.Set("clientID2", value);
            }
        }
        public string ClientIdCustom
        {
            get
            {
                return SyncSetting.settings.Get("clientIDcustom");
            }
            set
            {
                SyncSetting.settings.Set("clientIDcustom", value);
            }
        }

        public string ClientIdCurrentName
        {
            get
            {
                return SyncSetting.settings.Get("clientIDcurrentSelected");
            }
            set
            {
                SyncSetting.settings.Set("clientIDcurrentSelected", value);
            }
        }

        public string ClientIdCurrentValue
        {
            get
            {
                return SyncSetting.settings.Get(SyncSetting.settings.Get("clientIDcurrentSelected"));
            }
        }
    }
}
