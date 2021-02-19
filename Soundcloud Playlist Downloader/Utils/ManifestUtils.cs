using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
                    FilesystemUtils.CoerceValidFileName(new StringBuilder(soundCloudUri.Host + soundCloudUri.PathAndQuery), false), fileSystemUtil.FoldersPerArtist, downloadMode, syncMethod);
            FileSystemUtil = fileSystemUtil;
        }
        static readonly ReaderWriterLock ReadWriteManifestLock = new ReaderWriterLock();
        static readonly ReaderWriterLock UpdateManifestLock = new ReaderWriterLock();

        const int ReadLockTimeoutMs = 500;
        const int WriteLockTimeoutMs = 10000;

        public string DetermineManifestPath()
        {
            return Path.Combine(FileSystemUtil.Directory.FullName, ManifestName);
        }
        public void UpdateManifest(Track trackDownloaded)
        {
            //use UpdateManifestLock because nested methods use the ReadWriteManifestLock for reading and writing
            UpdateManifestLock.AcquireWriterLock(WriteLockTimeoutMs); 
            try
            {    
                List<Track> manifest = LoadManifestFromFile();                       
                AppendToJsonManifestObject(ref manifest, trackDownloaded);
                WriteManifestToFile(manifest);
                return;
            }
            finally
            {
                UpdateManifestLock.ReleaseWriterLock();
            }
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

        public static string MakeManifestString(string validManifestFilename, bool foldersPerArtist, EnumUtil.DownloadMode dlMode, int syncMethod)
        {
            return ".MNFST=" + validManifestFilename + ",FPA=" + foldersPerArtist + ",DM=" + dlMode + ",SM=" + syncMethod + ".json";
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
                string updatedManifestFilename = UpdateToNewManifestStructure(ManifestFileName);
                if (updatedManifestFilename.StartsWith(Path.Combine(FileSystemUtil.Directory.FullName, manifestProperties[0] + ",")))
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

        /// <summary>
        /// Replace old structure of filename
        /// </summary>
        /// <param name="manifestfile"></param>
        private string UpdateToNewManifestStructure(string manifestfile)
        {
            string updatedManifestFileStructure = manifestfile.Replace("IAIF=True,", "").Replace("IAIF=False,", "");
            if(manifestfile != updatedManifestFileStructure)
                File.Move(manifestfile, updatedManifestFileStructure);
            return updatedManifestFileStructure;
        }
    }
}