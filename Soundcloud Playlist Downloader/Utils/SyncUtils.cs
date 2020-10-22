﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Language;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class SyncUtils
    {
        private bool _createPlaylists;
        public ManifestUtils ManifestUtil;
        public DownloadUtils DownloadUtil;
        public PlaylistUtils PlaylistUtil;

        public SyncUtils(bool createPlaylists, ManifestUtils manifestUtil, DownloadUtils downloadUtil, PlaylistUtils playlistUtil)
        {
            PlaylistUtil = playlistUtil;
            DownloadUtil = downloadUtil;
            ManifestUtil = manifestUtil;
            _createPlaylists = createPlaylists;
        }

        public void Synchronize(IList<Track> tracks)
        {
            var tracksToDownload = new List<Track>();

            // define all local paths by combining the sanitzed artist (if checked by user) with the santized title
            FinalizePropertiesForTracks(tracks);

            // determine which new tracks should be downloaded
            NewTracksToDownload(tracks, tracksToDownload);

            // deletes, retags, or sets track in tracksToDownload for download
            AnalyseManifestTracks(tracks, tracksToDownload);

            // download the relevant tracks and continuously update the manifest
            DownloadUtil.DownloadSongs(tracksToDownload);
      
            if(_createPlaylists) PlaylistUtil.CreateSimpleM3U(); //Create playlist file       
        }



        private void NewTracksToDownload(IList<Track> allSongs, List<Track> tracksToDownload)
        {
            var manifestPath = ManifestUtil.DetermineManifestPath();
            List<Track> manifest;
            if (File.Exists(manifestPath))
            {
                manifest = ManifestUtil.LoadManifestFromFile();

                //all who's id is not in the manifest  
                tracksToDownload.AddRange(allSongs.Where(c => manifest.All(d => c.id != d.id)).ToList());
            }
            else
            {
                tracksToDownload.AddRange(allSongs);
            }
        }
       
        private void AnalyseManifestTracks(IList<Track> allTracks, List<Track> tracksToDownload)
        {
            Track oldTrack = null;
            var manifestPath = ManifestUtil.DetermineManifestPath();
            try
            {
                if (!File.Exists(manifestPath)) return;
                var manifest = ManifestUtil.LoadManifestFromFile();
                for (int index = 0; index < manifest.Count; index++)
                {
                    oldTrack = manifest[index];

                    oldTrack.LocalPath = Path.Combine(ManifestUtil.FileSystemUtil.Directory.FullName, oldTrack.LocalPathRelative);
                    var matchedTrack = allTracks.FirstOrDefault(i => i.id == oldTrack.id);
                    if (matchedTrack == null)
                    {
                        if (ManifestUtil.SyncMethod == 1) continue;
                        manifest.Remove(oldTrack);
                        index--;
                        DeleteFile(oldTrack.LocalPath);
                        continue;
                    }
                    if (!File.Exists(oldTrack.LocalPath))
                    {
                        manifest.Remove(oldTrack);
                        index--;
                        tracksToDownload.Add(matchedTrack);
                        continue;
                    }

                    //If the duration is shorter than before; suspect a change from full song to sample song
                    if (matchedTrack.duration < oldTrack.duration) continue;

                    if ((matchedTrack.IsHD == true) && (oldTrack.IsHD == false)) //track changed to HD
                    {
                        manifest.Remove(oldTrack);
                        index--;
                        DeleteFile(oldTrack.LocalPath);
                        tracksToDownload.Add(matchedTrack);
                        continue;
                    }
                    IEqualityComparer<SoundcloudBaseTrack> comparer = new CompareUtils();                
                    if (!comparer.Equals(oldTrack, matchedTrack))
                    {
                        ManifestUtil.ReplaceJsonManifestObject(ref manifest, matchedTrack, oldTrack, index);
                        Directory.CreateDirectory(Path.GetDirectoryName(matchedTrack.LocalPath));

                        if(!string.Equals(oldTrack.LocalPath, matchedTrack.LocalPath, StringComparison.OrdinalIgnoreCase))
                        {
                            File.Move(oldTrack.LocalPath, matchedTrack.LocalPath, true);
                            DeleteEmptyDirectory(oldTrack.LocalPath);
                        }

                        MetadataTaggingUtils.TagIt(matchedTrack);
                        continue;
                    }            
                }
                ManifestUtil.WriteManifestToFile(manifest);             
            }
            catch (Exception e)
            {
                ManifestUtil.ProgressUtil.IsError = true;
                throw new Exception(string.Format(LanguageManager.Language["STR_EXCEPTION_SYNC"], oldTrack?.EffectiveDownloadUrl, oldTrack?.LocalPath, e));
            }
        }
        private void DeleteFile(string fullPathSong)
        {
            if (!File.Exists(fullPathSong)) return;
            File.Delete(fullPathSong);
            DeleteEmptyDirectory(fullPathSong);
        }        
        private bool DeleteEmptyDirectory(string filenameWithPath)
        {
            if (ManifestUtil.FileSystemUtil.FoldersPerArtist) return false;
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

        public void FinalizePropertiesForTracks(IList<Track> tracks)
        {
            Parallel.ForEach(tracks, track => {
                FinalizePropertiesForTrack(track);
            });
        }

        public void FinalizePropertiesForTrack(Track track)
        {  
            //assume it's downloadable when a purchase link is not available
            if (track.downloadable == true && track.purchase_url == null)
            {                         
                //really make sure it's downloadable           
                track.downloadable = DownloadUtil.IsDownloadable(track.download_url);
                track.IsHD = true;
            }
            else
            {
                track.downloadable = false;
                track.IsHD = false;
            }
            track.LocalPath = ManifestUtil.FileSystemUtil.BuildTrackLocalPath(track);
        }
    }
}