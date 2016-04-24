using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Views;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class ManifestUtils
    {
        private static readonly object ReadWriteManifestLock = new object();
        public static string DetermineManifestPath(string directoryPath)
        {
            return Path.Combine(directoryPath, SoundcloudSyncMainForm.ManifestName);
        }
        public static void UpdateManifest(Track trackDownloaded, string directoryPath)
        {            
            var updateSuccesful = false;
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {                  
                    List<Track> manifest = LoadManifestFromFile(directoryPath);                       
                    AppendToJsonManifestObject(manifest, trackDownloaded, directoryPath);
                    WriteManifestToFile(manifest, directoryPath);                       
                    updateSuccesful = true;
                    break;
                }
                catch (Exception)
                {
                    // ignored
                }
                Thread.Sleep(50); // Pause 50ms before new attempt
            }
            if (updateSuccesful) return;
            SoundcloudSync.IsError = true;
            throw new Exception("Unable to update manifest");
        }

        public static List<Track> LoadManifestFromFile(string directoryPath)
        {
            var manifestPath = DetermineManifestPath(directoryPath);
            List<Track> manifest = new List<Track>();
            try
            {
                lock (ReadWriteManifestLock)
                {
                    using (var sr = new StreamReader(manifestPath))
                    {
                        var jsonManifestText = sr.ReadToEnd();
                        manifest = JsonConvert.DeserializeObject<List<Track>>(jsonManifestText);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return manifest;
        }

        public static void WriteManifestToFile(List<Track> manifest, string directoryPath)
        {
            var manifestPath = DetermineManifestPath(directoryPath);
            try
            {
                lock (ReadWriteManifestLock)
                {
                    using (var sw = new StreamWriter(manifestPath))
                    {
                        using (JsonWriter jw = new JsonTextWriter(sw))
                        {
                            jw.Formatting = Formatting.Indented;
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize(jw, manifest);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        internal static void AppendToJsonManifestObject(List<Track> manifests, Track trackDownloaded, string directoryPath)
        {
            trackDownloaded.DownloadDateTimeUtc = DateTime.UtcNow;
            trackDownloaded.ModifiedDateTimeUtc = DateTime.UtcNow;
            trackDownloaded.LocalPathRelative = trackDownloaded.LocalPath.Replace(directoryPath, "");
            manifests.Add(trackDownloaded);
        }

        public static string MakeManifestString(string validManifestFilename, bool foldersPerArtist, bool includeArtistInFilename, EnumUtil.DownloadMode dlMode, int syncMethod)
        {
            return ".MNFST=" + validManifestFilename + ",FPA=" + foldersPerArtist + ",IAIF=" +
                   includeArtistInFilename + ",DM=" + dlMode + ",SM=" + syncMethod + ".json";
        }

        public static void BackupManifest(string directoryPath, string manifestName)
        {
            //copy to backup location
            var destinationPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SoundCloud Playlist Sync",
                    DateTime.Today.ToString("dd/MM/yyyy") + " Manifest Backups");

            var destinationPathWithFile = Path.Combine(destinationPath, manifestName);
            Directory.CreateDirectory(destinationPath);
            File.Copy(Path.Combine(directoryPath, manifestName), destinationPathWithFile, true);
        }
    }
}