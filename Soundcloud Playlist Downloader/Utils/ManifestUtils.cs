using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Views;
public delegate void ProcessUpdateManifestDelegate(Track trackDownloaded, string directoryPath);

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class ManifestUtils
    {
        static ReaderWriterLock ReadWriteManifestLock = new ReaderWriterLock();
        const int ReadLockTimeoutMs = 500;
        const int WriteLockTimeoutMs = 1000;
        public static string ManifestName = "";

        public static string DetermineManifestPath(string directoryPath)
        {
            return Path.Combine(directoryPath, ManifestName);
        }
        public static void UpdateManifest(Track trackDownloaded, string directoryPath)
        {
            var updateSuccesful = false;
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {                  
                    List<Track> manifest = LoadManifestFromFile(directoryPath);                       
                    AppendToJsonManifestObject(ref manifest, trackDownloaded, directoryPath);
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
            List<Track> manifest = new List<Track>();
            var manifestPath = DetermineManifestPath(directoryPath);
            if (!File.Exists(manifestPath)) return manifest;
            ReadWriteManifestLock.AcquireReaderLock(ReadLockTimeoutMs);
            try
            {
                using (var sr = new StreamReader(manifestPath))
                {                
                    var jsonManifestText = sr.ReadToEnd();
                    manifest = JsonConvert.DeserializeObject<List<Track>>(jsonManifestText);
                }
            }
            finally
            {
                ReadWriteManifestLock.ReleaseReaderLock();
            }
            return manifest;
        }

        public static void WriteManifestToFile(List<Track> manifest, string directoryPath)
        {
            if (manifest.Count < 1) return;
            var manifestPath = DetermineManifestPath(directoryPath);
            ReadWriteManifestLock.AcquireWriterLock(WriteLockTimeoutMs);
            try
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
            finally
            {
                ReadWriteManifestLock.ReleaseWriterLock();
            }
        }
        internal static void AppendToJsonManifestObject(ref List<Track> manifests, Track trackDownloaded, string directoryPath)
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
            var destinationPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SoundCloud Playlist Sync",
                    DateTime.Today.ToString("dd/MM/yyyy") + " Manifest Backups"); 

            var destinationPathWithFile = Path.Combine(destinationPath, manifestName);
            Directory.CreateDirectory(destinationPath);
            ReadWriteManifestLock.AcquireReaderLock(WriteLockTimeoutMs);
            try
            {
                File.Copy(Path.Combine(directoryPath, manifestName), destinationPathWithFile, true);
            }
            finally
            {
                ReadWriteManifestLock.ReleaseReaderLock();
            }
        }

        public static bool FindManifestAndBackup(string directoryPath, string manifestName, out bool differentmanifest)
        {
            if (manifestName == "")
            {
                differentmanifest = true;
                return false;
            }
            differentmanifest = false;
            if (!Directory.Exists(directoryPath)) return false;        
            var files = Directory.GetFiles(directoryPath, ".MNFST=*", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                if (File.Exists(Path.Combine(directoryPath, manifestName)))
                {
                    BackupManifest(directoryPath, manifestName);
                    return true;
                }
                differentmanifest = true;
            }
            return false;
        }
    }
}