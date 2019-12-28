using Config.Net;

namespace SC_SYNC_Base.JsonObjects
{
    public static class SyncSetting
    {
        public static ISyncSetting settings = new ConfigurationBuilder<ISyncSetting>().UseJsonFile("appsettings.json").Build();

        public static string LoadSettingFromConfig(string propertyName, string accessString = "")
        {
            try
            {
                var value = settings.Get(accessString + propertyName);
                if(value != null)
                    return value;
            }
            catch
            {
            }
            return settings.Get(propertyName);
        }

        public static string GetAccessString(string currentIndexInSettingsFile, string currentIndexOnForm)
        {
            string accessString = "";
            if (currentIndexInSettingsFile != "1")
                accessString = currentIndexOnForm;
            return accessString;
        }


        public static void SaveSettingToConfig(string propertyName, string propertyValue, string currentIndexOnForm)
        {
            string accessString = GetAccessString(settings.Get("ConfigStateCurrentIndex"), currentIndexOnForm);
            settings.Set(accessString + propertyName, propertyValue);
        }
    }

    public interface ISyncSetting
    {
        string Get(string keyName);
        void Set(string keyName, string value);
    }
}
