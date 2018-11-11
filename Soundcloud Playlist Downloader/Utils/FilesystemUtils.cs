using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Soundcloud_Playlist_Downloader.JsonObjects;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class FilesystemUtils
    {
        public DirectoryInfo Directory;
        public DirectoryInfo OriginalDirectory;
        public string Format;
        public bool FoldersPerArtist;
        public bool ReplaceIllegalCharacters;
        public string LogPath;
        public bool ErrorsLogged;

        public FilesystemUtils(DirectoryInfo targetDirectory, string format, bool foldersPerArtist, bool replaceIllegalCharacters)
        {
            Format = format;
            ReplaceIllegalCharacters = replaceIllegalCharacters;
            FoldersPerArtist = foldersPerArtist;
            Directory = targetDirectory;
            OriginalDirectory = new DirectoryInfo(targetDirectory.FullName);
            ErrorsLogged = false;
            LogPath = $"{OriginalDirectory.FullName}\\log{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.txt";
        }

        public void LogTrackWithError(Track trackWithErrors, Exception e)
        {
            string textError = e.Message;
            if(e.InnerException?.InnerException != null)
                textError = e.InnerException?.InnerException?.Message;

            string textTrack = "Title: " + trackWithErrors.Title + ", Artist: " + trackWithErrors.Artist;
            UpdateLog($"{textTrack} Exception: {textError}." + Environment.NewLine);
            ErrorsLogged = true;
        }

        public void UpdateLog(string logLine)
        {
            var updateSuccesful = false;
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {
                    File.AppendAllText(LogPath, logLine);
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
            throw new Exception("Unable to update log");
        }

        public string GetAllErrorsFromLog()
        {
            return File.ReadAllText(LogPath).ToString();
        }

        public void ResetDirectoryInfo()
        {
            Directory = OriginalDirectory;
        }

        public void ChangeDirectoryInfo(string targetDirectory)
        {
            var targetLastFolder = Path.GetFileName(targetDirectory).TrimEnd('\\');

            targetDirectory = OriginalDirectory.FullName + '\\' + targetLastFolder;

            Directory = new DirectoryInfo(targetDirectory);
        }

        public string MakeRelativePath(string fullpath)
        {
            return fullpath.Replace(Directory.FullName, "").Substring(1);
        }

        public static bool IsPathWithinLimits(string fullPathAndFilename)
        {
            //In the Windows API the maximum length for a path is MAX_PATH, which is defined as 260 characters.
            //We'll make it 255 because there will be an extention (like .mp3).  
            const int maxPathLength = 255;
            return fullPathAndFilename.Length <= maxPathLength;
        }
        public static string BuildName(string Format, Track track, bool ReplaceIllegalCharacters)
        {
            var filename = Format.
                Replace("%title%", track.Title).
                Replace("%user%", track.Artist).
                Replace("%index%", (track.IndexFromSoundcloud + 1).ToString()).
                Replace("%genre%", track.genre).
                Replace("%ext%", track.original_format).
                Replace("%quality%", track.IsHD ? "(HQ)" : null).
                Replace("%label_name%", track.label_name).
                Replace("%desc%", track.description);
            filename = filename.Replace("%date%", DateTime.Parse(track.created_at).ToString("yyyy-MM-dd"));
            filename = filename.Replace("%time%", DateTime.Parse(track.created_at).ToString("HH.mm.ss"));

            return CoerceValidFileName(filename, ReplaceIllegalCharacters);
        }

        public string BuildTrackLocalPath(Track track)
        {
            string path = null;
            var validArtist = CoerceValidFileName(track.Artist, ReplaceIllegalCharacters); // true && ReplaceIllegalCharacters
            var validArtistFolderName = TrimDotsAndSpacesForFolderName(validArtist);
            var filename = BuildName(Format, track, ReplaceIllegalCharacters);

            Console.WriteLine(validArtistFolderName);
            if (FoldersPerArtist)
            {
                while (!IsPathWithinLimits(path = Path.Combine(Directory.FullName, validArtistFolderName,
                    filename)))
                {
                    filename = filename.Remove(filename.Length - 2);
                    //shorten to fit into max size of path
                }
            }
            else
            {
                while (!IsPathWithinLimits(path = Path.Combine(Directory.FullName, filename)))
                {
                    filename = filename.Remove(filename.Length - 2);
                    //shorten to fit into max size of path
                }
            }
            return path;
        }

        public static string CoerceValidFileName(string filename, bool checkForReplaceCharacters)
        {
            if (checkForReplaceCharacters)
            {
                filename = AlterChars(filename);
            }

            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = $@"[{invalidChars}]+";

            var reservedWords = new[]
            {
                "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = $"^{reservedWord}\\.";
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.",
                    RegexOptions.IgnoreCase);
            }

            if (String.IsNullOrEmpty(sanitisedNamePart))
                //if completely sanitized, make something that's not an empty string
                sanitisedNamePart = "(blank)";
            return sanitisedNamePart;
        }

        public static string AlterChars(string word)
        {
            //replace the following characters with characters from 'Halfwidth and Fullwidth Forms'
            //  / ? < > \ : * | "
            //the new characters are not visible in Visual Studio, but are perfectly visible in the file system
            word = word.
                Replace("/", "／").
                Replace("?", "？").
                Replace("<", "＜").
                Replace(">", "＞").
                Replace("\\", "＼").
                Replace(":", "：").
                Replace("*", "＊").
                Replace("|", "｜").
                Replace("\"", "＂");
            return word;
        }

        /// <summary>
        ///     Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        ///     http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>
        public static string TrimDotsAndSpacesForFolderName(string foldername)
        {
            var trimmed = foldername.Trim('.', ' ');

            if (String.IsNullOrEmpty(trimmed))
                trimmed = "(blank)"; //if completely trimmed, make something that's not an empty string
            return trimmed;
        }
    }
}