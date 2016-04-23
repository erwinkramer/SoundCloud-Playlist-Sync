using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class SyncUtils
    {
        public static void Synchronize(IList<Track> tracks, string clientId, string directoryPath)
        {
            //define all local paths by combining the sanitzed artist (if checked by user) with the santized title
            foreach (var track in tracks)
            {
                track.LocalPath = FilesystemUtils.GetTrackLocalPath(track, directoryPath);
            }

            // determine which tracks should be deleted or re-added
            DeleteOrAddRemovedTrack(directoryPath, tracks);

            // determine which tracks should be downloaded
            DetermineTracksToDownload(directoryPath, ref tracks);

            // download the relevant tracks and continuously update the manifest
            DownloadUtils.DownloadSongs(tracks, clientId, directoryPath);

            //Create playlist file
            var completed = PlaylistUtils.CreateSimpleM3U(tracks, directoryPath);

            var songstodownload = tracks.Count(x => x.HasToBeDownloaded);
            // validation
            if (songstodownload > 0 && DownloadUtils.IsActive)
            {
                PlaylistSync.IsError = true;
                throw new Exception(
                    "Some tracks failed to download. You might need to try a few more times before they can download correctly. " +
                    "The following tracks were not downloaded:" + Environment.NewLine +
                    string.Join(Environment.NewLine,
                        tracks.Where(x => x.HasToBeDownloaded)
                            .Select(x => "Title: " + x.Title + ", Artist: " + x.Artist)
                        ));
            }
        }
        private static void DetermineTracksToDownload(string directoryPath, ref IList<Track> allSongs)
        {
            var manifestPath = ManifestUtils.DetermineManifestPath(directoryPath);
            IList<string> streamUrls = new List<string>();
            IList<string> songsDownloaded = new List<string>();
            if (File.Exists(manifestPath))
            {
                songsDownloaded = File.ReadAllLines(manifestPath);
                foreach (var track in File.ReadAllLines(manifestPath))
                {
                    streamUrls.Add(ManifestUtils.ParseTrackPath(track, 0));
                }
            }
            foreach (var track in allSongs)
            {
                if (!streamUrls.Contains(track.EffectiveDownloadUrl))
                    track.HasToBeDownloaded = true;
                else if (songsDownloaded.Count > 0)
                {
                    // we need to add the extention to the local path for further use
                    // the only way we can know what the extention was when previously downloaded 
                    // is by checking the file directly, or by checking the manifest file, 
                    // we will do the latter
                    track.LocalPath += PlaylistUtils.GetExtension(songsDownloaded, track.LocalPath);
                }
            }
        }
       
        private static void DeleteOrAddRemovedTrack(string directoryPath, IList<Track> allTracks)
        {
            var manifestPath = ManifestUtils.DetermineManifestPath(directoryPath);
            try
            {
                if (File.Exists(manifestPath))
                {
                    var songsDownloaded = File.ReadAllLines(manifestPath);
                    IList<string> newManifest = new List<string>();

                    foreach (var songDownloaded in songsDownloaded)
                    {
                        var localTrackpath = ManifestUtils.ParseTrackPath(songDownloaded, 1);
                        var localPathDownloadedSongRelative = directoryPath + localTrackpath.Replace(directoryPath, "");
                        var songId =
                            new string(
                                ManifestUtils.ParseTrackPath(songDownloaded, 0)
                                    .ToCharArray()
                                    .Where(char.IsDigit)
                                    .ToArray());
                        var neutralPath = Path.ChangeExtension(localPathDownloadedSongRelative, null);
                        Track soundCloudTrack = null;
                        soundCloudTrack = allTracks.FirstOrDefault(song => song.stream_url.Contains("/" + songId + "/"));

                        var trackArtistOrNameChanged = false;
                        //WARNING      If we want to look if allTracks contains the downloaded file we need to trim the extention
                        //              because allTracks doesn't store the extention of the path                            
                        trackArtistOrNameChanged = !allTracks.Any(song => song.LocalPath.Contains(neutralPath));

                        //file does not exist anymore, it will be redownloaded by not adding it to the newManifest
                        if (!File.Exists(localPathDownloadedSongRelative))
                        {
                            continue;
                        }                       
                        //song is changed on SoundCloud (only checks artist and filename), redownload and remove old one.
                        if (trackArtistOrNameChanged && soundCloudTrack != null)
                        {
                            var localIsHd = ManifestUtils.ParseTrackPath(songDownloaded, 0).EndsWith("download");
                            if (soundCloudTrack.IsHD || (soundCloudTrack.IsHD == false && localIsHd == false))
                            // do not download Low Quality if HQ is already downloaded, even if the track is changed!
                            {
                                if (File.Exists(localPathDownloadedSongRelative))
                                {
                                    File.Delete(localPathDownloadedSongRelative);
                                    DeleteEmptyDirectory(localPathDownloadedSongRelative);
                                }
                                continue;
                            }
                        }
                        //file exists locally but not externally and can be removed
                        if (Form1.SyncMethod == 2 && soundCloudTrack == null)
                        {
                            File.Delete(localPathDownloadedSongRelative);
                            DeleteEmptyDirectory(localPathDownloadedSongRelative);
                        }
                        else
                        {
                            newManifest.Add(songDownloaded);
                        }
                    }
                    // the manifest is updated again later, but might as well update it here
                    // to save the deletions in event of crash or abort
                    File.WriteAllLines(manifestPath, newManifest);
                }
            }
            catch (Exception e)
            {
                PlaylistSync.IsError = true;
                throw new Exception("Unable to read manifest to determine tracks to delete; exception: " + e);
            }
        }

        private static bool DeleteEmptyDirectory(string filenameWithPath)
        {
            if (!Form1.FoldersPerArtist)
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
