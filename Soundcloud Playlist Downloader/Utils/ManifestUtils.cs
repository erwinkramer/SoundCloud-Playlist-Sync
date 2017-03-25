using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Soundcloud_Playlist_Downloader.JsonObjects;

public delegate void ProcessUpdateManifestDelegate(Track trackDownloaded);

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class ManifestUtils
    {
        public FilesystemUtils FileSystemUtil;
        public string ManifestName = "";
        public Uri SoundCloudUri;
        public int SyncMethod;
        public EnumUtil.DownloadMode DownloadMode;
        public ProgressUtils ProgressUtil;
        public ManifestUtils(ProgressUtils progressUtil, FilesystemUtils fileSystemUtil, Uri soundCloudUri, EnumUtil.DownloadMode downloadMode, int syncMethod)
        {
            ProgressUtil = progressUtil;
            SyncMethod = syncMethod;
            DownloadMode = downloadMode;
            SoundCloudUri = soundCloudUri;
            ManifestName = MakeManifestString(
                    fileSystemUtil.CoerceValidFileName(soundCloudUri.Host + soundCloudUri.PathAndQuery, false), fileSystemUtil.FoldersPerArtist,
                    fileSystemUtil.IncludeArtistInFilename, downloadMode, syncMethod); ;
            FileSystemUtil = fileSystemUtil;
        }
        static readonly ReaderWriterLock ReadWriteManifestLock = new ReaderWriterLock();
        const int ReadLockTimeoutMs = 500;
        const int WriteLockTimeoutMs = 1000;

        public string DetermineManifestPath()
        {
            return Path.Combine(FileSystemUtil.Directory.FullName, ManifestName);
        }
        public void UpdateManifest(Track trackDownloaded)
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
            ProgressUtil.IsError = true;
            throw new Exception("Unable to update manifest");
        }

        public List<Track> LoadManifestFromFile()
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

        public void WriteManifestToFile(List<Track> manifest)
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
        internal void AppendToJsonManifestObject(ref List<Track> manifests, Track trackDownloaded)
        {
            trackDownloaded.DownloadDateTimeUtc = DateTime.UtcNow;
            trackDownloaded.ModifiedDateTimeUtc = DateTime.UtcNow;
            trackDownloaded.LocalPathRelative = FileSystemUtil.MakeRelativePath(trackDownloaded.LocalPath);
            manifests.Add(trackDownloaded);
        }

        internal void ReplaceJsonManifestObject(ref List<Track> manifests, Track trackChanged, Track oldTrack, int index)
        {
            trackChanged.ModifiedDateTimeUtc = DateTime.UtcNow;
            trackChanged.DownloadDateTimeUtc = oldTrack.DownloadDateTimeUtc;
            trackChanged.LocalPath += Path.GetExtension(oldTrack.LocalPath);
            trackChanged.LocalPathRelative = FileSystemUtil.MakeRelativePath(trackChanged.LocalPath);
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

        public bool FindManifestAndBackup(out bool differentmanifest)
        {
            if (ManifestName == "")
            {
                differentmanifest = true;
                return false;
            }
            differentmanifest = false;
            if (!Directory.Exists(FileSystemUtil.Directory.FullName)) return false;        
            var files = Directory.GetFiles(FileSystemUtil.Directory.FullName, ".MNFST=*", SearchOption.TopDirectoryOnly);

            string[] manifestProperties = ManifestName.Split(',');

            foreach (string ManifestFileName in files)
            {
                if (ManifestFileName.StartsWith(Path.Combine(FileSystemUtil.Directory.FullName, manifestProperties[0] + ",")))
                {
                    if(File.Exists(Path.Combine(FileSystemUtil.Directory.FullName, ManifestName)))
                    {
                        BackupManifest(FileSystemUtil.Directory.FullName, ManifestName);
                        return true;
                    }
                    else
                    {
                        differentmanifest = true;
                    }
                }
            }       
            return false;
        }
    }
}