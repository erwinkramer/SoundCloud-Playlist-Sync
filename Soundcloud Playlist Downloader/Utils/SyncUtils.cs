using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Views;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class SyncUtils
    {
        public static void Synchronize(IList<Track> tracks)
        {
            List<Track> tracksToDownload = new List<Track>();

            // define all local paths by combining the sanitzed artist (if checked by user) with the santized title
            foreach (var track in tracks)
            {
                track.LocalPath = FilesystemUtils.BuildTrackLocalPath(track);
            }

            // determine which new tracks should be downloaded
            NewTracksToDownload(tracks, tracksToDownload);

            // deletes, retags, or sets track in tracksToDownload for download
            AnalyseManifestTracks(tracks, tracksToDownload);  

            // download the relevant tracks and continuously update the manifest
            DownloadUtils.DownloadSongs(tracksToDownload);
      
            PlaylistUtils.CreateSimpleM3U(); //Create playlist file

            var songsNotDownloaded = tracksToDownload.Count(x => x.IsDownloaded == false);        
            if (songsNotDownloaded > 0 && DownloadUtils.IsActive) // validation
            {
                SoundcloudSync.IsError = true;
                throw new Exception(
                    "Some tracks failed to download. You might need to try a few more times before they can download correctly. " +
                    "The following tracks were not downloaded:" + Environment.NewLine +
                    string.Join(Environment.NewLine,
                        tracksToDownload.Where(x => x.IsDownloaded == false)
                            .Select(x => "Title: " + x.Title + ", Artist: " + x.Artist)
                        ));
            }
        }
        private static void NewTracksToDownload(IList<Track> allSongs, List<Track> tracksToDownload)
        {
            var manifestPath = ManifestUtils.DetermineManifestPath();
            List<Track> manifest = new List<Track>();
            if (File.Exists(manifestPath))
            {
                manifest = ManifestUtils.LoadManifestFromFile();
                //all who's id is not in the manifest  
                tracksToDownload.AddRange(allSongs.Where(c => manifest.All(d => c.id != d.id)).ToList());
            }
            else
            {
                tracksToDownload.AddRange(allSongs);
            }
        }
       
        private static void AnalyseManifestTracks(IList<Track> allTracks, List<Track> tracksToDownload)
        {
            var manifestPath = ManifestUtils.DetermineManifestPath();
            try
            {
                if (!File.Exists(manifestPath)) return;
                List<Track> manifest = ManifestUtils.LoadManifestFromFile();
                for (int index = 0; index < manifest.Count; index++)
                {
                    manifest[index].LocalPath = Path.Combine(FilesystemUtils.Directory.FullName, manifest[index].LocalPathRelative);
                    var compareTrack = allTracks.FirstOrDefault(i => i.id == manifest[index].id);
                    if (compareTrack == null)
                    {
                        if (SoundcloudSyncMainForm.SyncMethod == 1) return;
                        manifest.Remove(manifest[index]);
                        DeleteFile(manifest[index].LocalPath);
                        continue;
                    }
                    if (!File.Exists(manifest[index].LocalPath))
                    {
                        manifest.Remove(manifest[index]);
                        tracksToDownload.Add(compareTrack);
                        continue;
                    }
                    //If the duration is shorter than before; suspect a change from full song to sample song
                    if (manifest[index].duration > compareTrack.duration) continue;

                    if (compareTrack.IsHD && !manifest[index].IsHD) //track changed to HD
                    {
                        manifest.Remove(manifest[index]);
                        DeleteFile(manifest[index].LocalPath);
                        tracksToDownload.Add(compareTrack);
                        continue;
                    }
                    IEqualityComparer<SoundcloudBaseTrack> comparer = new CompareUtils();                
                    if (!comparer.Equals(manifest[index], compareTrack))
                    {
                        var OldPath = manifest[index].LocalPath;
                        ManifestUtils.ReplaceJsonManifestObject(ref manifest, compareTrack, manifest[index], index);
                        Directory.CreateDirectory(Path.GetDirectoryName(manifest[index].LocalPath));
                        File.Move(OldPath, manifest[index].LocalPath);
                        DeleteEmptyDirectory(OldPath);
                        MetadataTaggingUtils.TagIt(manifest[index]);
                        continue;
                    }            
                }
                ManifestUtils.WriteManifestToFile(manifest);             
            }
            catch (Exception e)
            {
                SoundcloudSync.IsError = true;
                throw new Exception("Unable to read manifest to determine tracks to delete; exception: " + e);
            }
        }
        private static void DeleteFile(string fullPathSong)
        {
            if (!File.Exists(fullPathSong)) return;
            File.Delete(fullPathSong);
            DeleteEmptyDirectory(fullPathSong);
        }        
        private static bool DeleteEmptyDirectory(string filenameWithPath)
        {
            if (!SoundcloudSyncMainForm.FoldersPerArtist) return false;
            var path = Path.GetDirectoryName(filenameWithPath);
            if (path == null || Directory.EnumerateFileSystemEntries(path).Any()) return false;
            try
            {
                Directory.Delete(path, false); //recursive not true because should be already empty
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
