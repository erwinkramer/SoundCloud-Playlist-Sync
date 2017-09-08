using System;
using System.IO;
using System.Text.RegularExpressions;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Views;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class FilesystemUtils
    {
        public DirectoryInfo Directory;
        public bool IncludeArtistInFilename;
        public bool IncludeDateInFilename;
        public bool FoldersPerArtist;
        public bool ReplaceIllegalCharacters;
        public FilesystemUtils(DirectoryInfo targetDirectory, bool includeArtistInFilename, bool foldersPerArtist, bool replaceIllegalCharacters, bool includeDateInFilename)
        {
            IncludeDateInFilename = includeDateInFilename;
            ReplaceIllegalCharacters = replaceIllegalCharacters;
            FoldersPerArtist = foldersPerArtist;
            IncludeArtistInFilename = includeArtistInFilename;
            Directory = targetDirectory;
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
        public string BuildTrackLocalPath(Track track)
        {
            string path;
            var validArtist = CoerceValidFileName(track.Artist, true);
            var validArtistFolderName = TrimDotsAndSpacesForFolderName(validArtist);
            var validTitle = CoerceValidFileName(track.Title, true);
            var filename = string.Empty;

            if (IncludeDateInFilename)
            {
                var dateTimeCreatedAt = DateTime.Parse(track.created_at);
                filename += dateTimeCreatedAt.ToString("yyyy-MM-dd HH.mm.ss ");
            }

            if (IncludeArtistInFilename)
                filename += validArtist + " - ";

            if (track.IsHD)
                validTitle += " (HQ)";

            filename += validTitle;           

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

        public string CoerceValidFileName(string filename, bool checkForReplaceCharacters)
        {
            if (checkForReplaceCharacters && ReplaceIllegalCharacters)
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
