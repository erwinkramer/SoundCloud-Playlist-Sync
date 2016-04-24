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
        public static void Synchronize(IList<Track> tracks, string clientId, string directoryPath)
        {
            List<Track> tracksToDownload = new List<Track>();

            //define all local paths by combining the sanitzed artist (if checked by user) with the santized title
            foreach (var track in tracks)
            {
                track.LocalPath = FilesystemUtils.BuildTrackLocalPath(track, directoryPath);
            }

            // deletes, retags, or sets track in tracksToDownload for download
            AnalyseManifestTracks(directoryPath, tracks, tracksToDownload);

            // determine which new tracks should be downloaded
            NewTracksToDownload(directoryPath, tracks, tracksToDownload);

            // download the relevant tracks and continuously update the manifest
            DownloadUtils.DownloadSongs(tracksToDownload, clientId, directoryPath);
      
            PlaylistUtils.CreateSimpleM3U(directoryPath); //Create playlist file

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
        private static void NewTracksToDownload(string directoryPath, IList<Track> allSongs, List<Track> tracksToDownload)
        {
            var manifestPath = ManifestUtils.DetermineManifestPath(directoryPath);
            List<Track> manifest = new List<Track>();
            if (File.Exists(manifestPath))
            {
                manifest = ManifestUtils.LoadManifestFromFile(directoryPath);               
            }                      
            //all who's id is not in the manifest  
            tracksToDownload.AddRange(allSongs.Where(c => manifest.All(d => c.id != d.id)).ToList());
        }
       
        private static void AnalyseManifestTracks(string directoryPath, IList<Track> allTracks, List<Track> tracksToDownload)
        {
            var manifestPath = ManifestUtils.DetermineManifestPath(directoryPath);
            try
            {
                if (!File.Exists(manifestPath)) return;
                List<Track> manifest = ManifestUtils.LoadManifestFromFile(directoryPath);
                for (int index = 0; index < manifest.Count; index++)
                {
                    string fullPathSong = Path.Combine(directoryPath, manifest[index].LocalPath);
                    var compareTrack = allTracks.FirstOrDefault(i => i.id == manifest[index].id);
                    if (compareTrack == null)
                    {
                        if (SoundcloudSyncMainForm.SyncMethod == 2)
                        {
                            DeleteFile(fullPathSong);
                            manifest.Remove(manifest[index]);
                        }
                        continue;
                    }

                    if (manifest[index].duration <= compareTrack.duration)
                    //If the duration is shorter suspect a change from full song to sample song
                    {
                        if (!compareTrack.IsHD && manifest[index].IsHD)
                        {
                            // ignore 
                        }
                        else
                        {
                            DeleteFile(fullPathSong);
                            tracksToDownload.Add(manifest[index]);
                            continue;
                        }
                    }
                    if (manifest[index].Title != compareTrack.Title)
                    {                    
                        Track retagTrack = manifest[index];
                        MetadataTaggingUtils.ReTag(ref retagTrack, compareTrack);
                        manifest[index] = retagTrack;
                    }

                    if (!File.Exists(fullPathSong))
                    {
                        tracksToDownload.Add(manifest[index]);
                    }
                }
                ManifestUtils.WriteManifestToFile(manifest, directoryPath);             
            }
            catch (Exception e)
            {
                SoundcloudSync.IsError = true;
                throw new Exception("Unable to read manifest to determine tracks to delete; exception: " + e);
            }
        }
        private static void DeleteFile(string fullPathSong)
        {
            if (File.Exists(fullPathSong))
            {
                File.Delete(fullPathSong);
                DeleteEmptyDirectory(fullPathSong);
            }
        }        
        private static bool DeleteEmptyDirectory(string filenameWithPath)
        {
            if (!SoundcloudSyncMainForm.FoldersPerArtist)
                return false;
            var path = Path.GetDirectoryName(filenameWithPath);
            if (path != null && !Directory.EnumerateFileSystemEntries(path).Any()) //folder = empty
            {
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
            return false;
        }
    }
}
