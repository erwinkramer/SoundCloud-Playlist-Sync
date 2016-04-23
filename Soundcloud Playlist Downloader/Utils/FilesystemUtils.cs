using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class FilesystemUtils
    {
        public static bool IsPathWithinLimits(string fullPathAndFilename)
        {
            //In the Windows API the maximum length for a path is MAX_PATH, which is defined as 260 characters.
            //We'll make it 250 because there will be an extention and, in some cases, an HQ tag appended to the filename.  
            const int maxPathLength = 250;
            return fullPathAndFilename.Length <= maxPathLength;
        }
        public static string GetTrackLocalPath(Track track, string directoryPath)
        {
            string path;
            var validArtist = CoerceValidFileName(track.Artist, true);
            var validArtistFolderName = TrimDotsAndSpacesForFolderName(validArtist);
            var validTitle = CoerceValidFileName(track.Title, true);
            var filenameWithArtist = validArtist + " - " + validTitle;

            if (Form1.FoldersPerArtist)
            {
                if (Form1.IncludeArtistInFilename) //include artist name
                {
                    while (!FilesystemUtils.IsPathWithinLimits(path = Path.Combine(directoryPath, validArtistFolderName,
                        filenameWithArtist)))
                    {
                        filenameWithArtist = filenameWithArtist.Remove(filenameWithArtist.Length - 2);
                        //shorten to fit into max size of path
                    }
                }
                else
                {
                    while (!FilesystemUtils.IsPathWithinLimits(path = Path.Combine(directoryPath, validArtistFolderName,
                        validTitle)))
                    {
                        validTitle = validTitle.Remove(validTitle.Length - 2);
                        //shorten to fit into max size of path
                    }
                }
            }
            else
            {
                if (Form1.IncludeArtistInFilename) //include artist name
                {
                    while (!FilesystemUtils.IsPathWithinLimits(path = Path.Combine(directoryPath, filenameWithArtist)))
                    {
                        filenameWithArtist = filenameWithArtist.Remove(filenameWithArtist.Length - 2);
                        //shorten to fit into max size of path
                    }
                }
                else
                {
                    while (!FilesystemUtils.IsPathWithinLimits(path = Path.Combine(directoryPath, validTitle)))
                    {
                        validTitle = validTitle.Remove(validTitle.Length - 2);
                        //shorten to fit into max size of path
                    }
                }
            }
            if (track.IsHD)
            {
                path += " (HQ)";
            }

            return path;
        }

        public static string CoerceValidFileName(string filename, bool checkForReplaceCharacters)
        {
            if (checkForReplaceCharacters && Form1.ReplaceIllegalCharacters)
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

        // <summary>
        ///     Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        ///     http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>
        public static string TrimDotsAndSpacesForFolderName(string foldername)
        {
            var trimmed = foldername.Trim('.', ' ');

            if (string.IsNullOrEmpty(trimmed))
                trimmed = "(blank)"; //if completely trimmed, make something that's not an empty string
            return trimmed;
        }
    }
}
