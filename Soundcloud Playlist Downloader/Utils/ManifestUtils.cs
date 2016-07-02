using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Soundcloud_Playlist_Downloader.JsonObjects;

public delegate void ProcessUpdateManifestDelegate(Track trackDownloaded);

namespace Soundcloud_Playlist_Downloader.Utils
{
    public static class ManifestUtils
    {
        static readonly ReaderWriterLock ReadWriteManifestLock = new ReaderWriterLock();
        const int ReadLockTimeoutMs = 500;
        const int WriteLockTimeoutMs = 1000;
        public static string ManifestName = "";

        public static string DetermineManifestPath()
        {
            return Path.Combine(FilesystemUtils.Directory.FullName, ManifestName);
        }
        public static void UpdateManifest(Track trackDownloaded)
        {
            var updateSuccesful = false;
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {                  
                    List<Track> manifest = LoadManifestFromFile();                       
                    AppendToJsonManifestObject(ref manifest, trackDownloaded);
                    WriteManifestToFile(manifest);                       
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

        public static List<Track> LoadManifestFromFile()
        {
            var manifest = new List<Track>();
            var manifestPath = DetermineManifestPath();
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

        public static void WriteManifestToFile(List<Track> manifest)
        {
            if (manifest.Count < 1) return;
            var manifestPath = DetermineManifestPath();
            ReadWriteManifestLock.AcquireWriterLock(WriteLockTimeoutMs);
            try
            { 
                using (var sw = new StreamWriter(manifestPath))
                {
                    using (JsonWriter jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;
                        var serializer = new JsonSerializer();
                        serializer.Serialize(jw, manifest);
                    }
                }
            }
            finally
            {
                ReadWriteManifestLock.ReleaseWriterLock();
            }
        }
        internal static void AppendToJsonManifestObject(ref List<Track> manifests, Track trackDownloaded)
        {
            trackDownloaded.DownloadDateTimeUtc = DateTime.UtcNow;
            trackDownloaded.ModifiedDateTimeUtc = DateTime.UtcNow;
            trackDownloaded.LocalPathRelative = FilesystemUtils.MakeRelativePath(trackDownloaded.LocalPath);
            manifests.Add(trackDownloaded);
        }

        internal static void ReplaceJsonManifestObject(ref List<Track> manifests, Track trackChanged, Track oldTrack, int index)
        {
            trackChanged.ModifiedDateTimeUtc = DateTime.UtcNow;
            trackChanged.DownloadDateTimeUtc = oldTrack.DownloadDateTimeUtc;
            trackChanged.LocalPath += Path.GetExtension(oldTrack.LocalPath);
            trackChanged.LocalPathRelative = FilesystemUtils.MakeRelativePath(trackChanged.LocalPath);
            manifests[index] = trackChanged;
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

        public static bool FindManifestAndBackup(string manifestName, out bool differentmanifest)
        {
            if (manifestName == "")
            {
                differentmanifest = true;
                return false;
            }
            differentmanifest = false;
            if (!Directory.Exists(FilesystemUtils.Directory.FullName)) return false;        
            var files = Directory.GetFiles(FilesystemUtils.Directory.FullName, ".MNFST=*", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                if (File.Exists(Path.Combine(FilesystemUtils.Directory.FullName, manifestName)))
                {
                    BackupManifest(FilesystemUtils.Directory.FullName, manifestName);
                    return true;
                }
                differentmanifest = true;
            }
            return false;
        }
    }
}