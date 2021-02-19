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

        public string DefaultOAuthToken = "2-291834-4570680-lpD214XFqfKQZS6d";

        public string ClientId1
        {
            get
            {
                return "a3dd183a357fcff9a6943c0d65664087";
            }
        }
        public string ClientId2
        {
            get
            {
                return "dfd268143d94dad03a4242c54646e4a4";
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

        public string OAuthToken
        {
            get
            {
                return SyncSetting.settings.Get("oAuthToken");
            }
            set
            {
                SyncSetting.settings.Set("oAuthToken", value);
            }
        }

        public string ClientIdCurrentValue
        {
            get
            {
                var currentSelected = SyncSetting.settings.Get("clientIDcurrentSelected");
                if (string.Equals(currentSelected, "clientIDcustom", StringComparison.InvariantCultureIgnoreCase))
                    return ClientIdCustom;
                else if (string.Equals(currentSelected, "clientID1", StringComparison.InvariantCultureIgnoreCase))
                    return ClientId1;
                else if (string.Equals(currentSelected, "ClientId2", StringComparison.InvariantCultureIgnoreCase))
                    return ClientId2;
                else
                    throw new Exception("Current selected client id does not match any of the existing cliend id values.");
            }
        }
    }
}
