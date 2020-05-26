﻿using Config.Net;
using System;
using System.IO;
using System.Reflection;

namespace SC_SYNC_Base.JsonObjects
{
    public static class SyncSetting
    {
        public static ISyncSetting settings = InitializeSettingsFile();

        public static ISyncSetting InitializeSettingsFile()
        {
            var settingsfolderInDocumentStore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SoundCloud Playlist Sync");
            var settingsfileInDocumentStore = Path.Combine(settingsfolderInDocumentStore, "appsettings.json");
            
            //create directory or else it won't create the file
            Directory.CreateDirectory(settingsfolderInDocumentStore);

            var settingsfileInSource = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "appsettings.json");
            if (!File.Exists(settingsfileInDocumentStore))
                File.Copy(settingsfileInSource, settingsfileInDocumentStore);
            return new ConfigurationBuilder<ISyncSetting>().UseJsonFile(settingsfileInDocumentStore).Build();
        }

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
