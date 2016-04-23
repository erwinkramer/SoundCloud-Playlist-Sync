using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class ManifestUtils
    {
        private static readonly object WriteManifestLock = new object();

        public static string ParseTrackPath(string csv, int position)
        {
            if (csv != null && csv.IndexOf(',') >= 0)
            {
                //only make 1 split, as a comma (,) can be found in a song name!
                return csv.Split(new[] {','}, 2)[position]; //position 0 is streampath, position 1 is local path
            }
            return csv;
        }

        public static string DetermineManifestPath(string directoryPath)
        {
            return Path.Combine(directoryPath, Form1.ManifestName);
        }

        public static void UpdateSyncManifest(Track trackDownloaded, string directoryPath)
        {
            string track = null;
            track = trackDownloaded.EffectiveDownloadUrl + "," + trackDownloaded.LocalPath.Replace(directoryPath, "");
            IList<string> content = new List<string>();
            content.Add(track);

            var updateSuccesful = false;
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {
                    lock (WriteManifestLock)
                    {
                        var manifestPath = DetermineManifestPath(directoryPath);
                        File.AppendAllLines(manifestPath, content);
                        //if file does not exist, this function will create one
                        updateSuccesful = true;
                        break;
                    }
                }
                catch (Exception)
                {
                }
                Thread.Sleep(50); // Pause 50ms before new attempt
            }
            if (!updateSuccesful)
            {
                PlaylistSync.IsError = true;
                throw new Exception("Unable to update manifest");
            }
        }

        public static string MakeManifestString(string validManifestFilename, bool foldersPerArtist, bool includeArtistInFilename, EnumUtil.DownloadMode dlMode, int syncMethod)
        {
            return ".MNFST=" + validManifestFilename + ",FPA=" + foldersPerArtist + ",IAIF=" +
                   includeArtistInFilename + ",DM=" + dlMode + ",SM=" + syncMethod + ".csv";
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