using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Language;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class PlaylistUtils
    {
        private ManifestUtils _manifestUtil;
        public PlaylistUtils(ManifestUtils manifestUtil)
        {
            _manifestUtil = manifestUtil;
        }
        private static string GeneratedBy { get { return LanguageManager.Language["STR_PLISTUTIL_GENBY"]; } }
        private static string Definition { get { return LanguageManager.Language["STR_PLISTUTIL_DEF"]; } }
        public bool[] CreateSimpleM3U()
        {
            var completed = new bool[5];
            var manifest = _manifestUtil.LoadManifestFromFile();
            if (manifest.Count < 1) return completed;
            string manifestDirectoryFullName = _manifestUtil.FileSystemUtil.Directory.FullName;

            WriteM3UtoFile(
                new List<string>(SortOnMostLiked(manifest)), 
                Path.Combine(manifestDirectoryFullName, LanguageManager.Language["STR_PLISTUTIL_M3U_ML"] + ".m3u8"), 
                out completed[0]
                );
              
            WriteM3UtoFile(
                new List<string>(SortOnMostPlayed(manifest)), 
                Path.Combine(manifestDirectoryFullName, LanguageManager.Language["STR_PLISTUTIL_M3U_MP"] + ".m3u8"), 
                out completed[1]
                );

            WriteM3UtoFile(
                new List<string>(RecentlyChanged(manifest)), 
                Path.Combine(manifestDirectoryFullName, LanguageManager.Language["STR_PLISTUTIL_M3U_RC"] + ".m3u8"), 
                out completed[2]
                );

            WriteM3UtoFile(
                new List<string>(RecentlyAdded(manifest)),
                Path.Combine(manifestDirectoryFullName, LanguageManager.Language["STR_PLISTUTIL_M3U_RD"] + ".m3u8"),
                out completed[3]
                );

            WriteM3UtoFile(
               new List<string>(SortOnSoundcloudIndexes(manifest)),
               Path.Combine(manifestDirectoryFullName, LanguageManager.Language["STR_PLISTUTIL_M3U_OBS"] + ".m3u8"),
               out completed[4]
               );

            return completed;
        }       
        public static void WriteM3UtoFile(IList<string> newM3U, string m3uPath, out bool updateSuccesful)
        {
            updateSuccesful = false;
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {             
                    File.WriteAllLines(m3uPath, newM3U);
                    updateSuccesful = true;
                    break;                    
                }
                catch (Exception)
                {
                    // ignored
                }
                Thread.Sleep(50); // Pause 50ms before new attempt
            }
            if (!updateSuccesful)
            {
                throw new Exception(LanguageManager.Language["STR_PLISTUTIL_M3U_ERROR"]);
            }
        }
        public static IList<string> SortOnMostLiked(List<Track> manifest)
        {
            IList<string> newM3U = (
                from m in manifest.AsParallel()
                orderby m.favoritings_count ascending
                select m.LocalPathRelative).ToList();
            newM3U.Insert(0, string.Format("{0} {1}. {2}", Definition, LanguageManager.Language["STR_PLISTUTIL_SORTML"], GeneratedBy));
            return newM3U;
        }
        public static IList<string> SortOnMostPlayed(List<Track> manifest)
        {
            IList<string> newM3U = (
                from m in manifest.AsParallel()
                orderby m.playback_count descending 
                select m.LocalPathRelative).ToList();
            newM3U.Insert(0, string.Format("{0} {1}. {2}", Definition, LanguageManager.Language["STR_PLISTUTIL_SORTMP"], GeneratedBy));
            return newM3U;
        }

        public static IList<string> SortOnSoundcloudIndexes(List<Track> manifest)
        {
            IList<string> newM3U = (
                from m in manifest.AsParallel()
                orderby m.IndexFromSoundcloud ascending
                select m.LocalPathRelative).ToList();
            newM3U.Insert(0, string.Format("{0} {1}. {2}", Definition, LanguageManager.Language["STR_PLISTUTIL_SORTSO"], GeneratedBy));
            return newM3U;
        }
        public static IList<string> RecentlyAdded(List<Track> manifest)
        {
            IList<string> newM3U = (
                from m in manifest.AsParallel()
                orderby m.DownloadDateTimeUtc descending
                select m.LocalPathRelative).ToList();
            newM3U.Insert(0, string.Format("{0} {1}. {2}", Definition, LanguageManager.Language["STR_PLISTUTIL_SORTRD"], GeneratedBy));
            return newM3U;
        }
        public static IList<string> RecentlyChanged(List<Track> manifest)
        {
            IList<string> newM3U = (
                from m in manifest.AsParallel()
                orderby m.ModifiedDateTimeUtc descending
                select m.LocalPathRelative).ToList();
            newM3U.Insert(0, string.Format("{0} {1}. {2}", Definition, LanguageManager.Language["STR_PLISTUTIL_SORTRC"], GeneratedBy));
            return newM3U;          
        }
    }
}