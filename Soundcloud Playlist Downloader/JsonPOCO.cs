using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Soundcloud_Playlist_Downloader.JsonPoco
{

    public class PaginatedCollectionPlaylist
    {
        public PlaylistItem[] collection { get; set; }
        public string next_href { get; set; }
    }
    public class PaginatedCollectionRegular
    {
        public Track[] collection { get; set; }
        public string next_href { get; set; }
    }

    public class NextInfo
    {
        public string next_href { get; set; }
    }

    public class PlaylistRoot
    {
        public PlaylistItem[] PlaylistItems { get; set; }
    }

    public class PlaylistItem
    {
        public int duration { get; set; }
        public object release_day { get; set; }
        public string permalink_url { get; set; }
        public string genre { get; set; }
        public string permalink { get; set; }
        public object purchase_url { get; set; }
        public object release_month { get; set; }
        public object description { get; set; }
        public string uri { get; set; }
        public object label_name { get; set; }
        public string tag_list { get; set; }
        //public List<string> tag_list { get; set; }
        public object release_year { get; set; }
        public int track_count { get; set; }
        public int user_id { get; set; }
        public string last_modified { get; set; }
        public string license { get; set; }
        public Track[] tracks { get; set; }
        public object playlist_type { get; set; }
        public int id { get; set; }
        public bool? downloadable { get; set; }
        public string sharing { get; set; }
        public string created_at { get; set; }
        public object release { get; set; }
        public string kind { get; set; }
        public string title { get; set; }
        public object type { get; set; }
        public object purchase_title { get; set; }
        public Created_With created_with { get; set; }
        public object artwork_url { get; set; }
        public object ean { get; set; }
        public bool? streamable { get; set; }
        public User user { get; set; }
        public string embeddable_by { get; set; }
        public object label_id { get; set; }
    }


    public class TrackCreatedWith
    {
        public int id { get; set; }
        public string kind { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public string permalink_url { get; set; }
        public string external_url { get; set; }
    }

    public class Created_With
    {
        public string permalink_url { get; set; }
        public string name { get; set; }
        public string external_url { get; set; }
        public string uri { get; set; }
        public string creator { get; set; }
        public int id { get; set; }
        public string kind { get; set; }
    }


    public class User
    {
        public string permalink_url { get; set; }
        public string permalink { get; set; }
        public string username { get; set; }
        public string uri { get; set; }
        public string last_modified { get; set; }
        public int id { get; set; }
        public string kind { get; set; }
        public string avatar_url { get; set; }
    }

    public class Track
    {
        public string kind { get; set; }
        public int id { get; set; }
        public string created_at { get; set; }
        public int user_id { get; set; }
        public int duration { get; set; }
        public bool commentable { get; set; }
        public string state { get; set; }
        public int original_content_size { get; set; }
        public string last_modified { get; set; }
        public string sharing { get; set; }
        public string tag_list { get; set; }
        public string permalink { get; set; }
        public bool? streamable { get; set; }
        public string embeddable_by { get; set; }
        public bool downloadable { get; set; }
        public string purchase_url { get; set; }
        public int? label_id { get; set; }
        public string purchase_title { get; set; }
        public string genre { get; set; }
      
        private string _title = null;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                //_title = Sanitize(value);
            }
        }
        public string EffectiveDownloadUrl
        {
            get
            {
                string url = string.Empty;
                if(stream_url == null)
                {
                    //WARNING       On rare occaisions the stream url is not available, blame this on the SoundCloud API
                    //              We can manually create the stream url anyway because we have the song id
                    //NOTE          This shouldn't be necessary anymore, since we changed the client_id to another one that actually works
                    stream_url = "https://api.soundcloud.com/tracks/" + id + "/stream";
                }
                if (Form1.Highqualitysong) //user has selected to download high quality songs if available
                {
                    url = !string.IsNullOrWhiteSpace(download_url) ? 
                        download_url : stream_url; //check if high quality url (download_url) is available
                }
                else
                {
                    url = stream_url; //else just get the low quality MP3 (stream_url)
                }
                if (!string.IsNullOrWhiteSpace(url))
                {
                    return url.Replace("\r", "").Replace("\n", "");
                }
                else
                {
                    return null;
                }
            }
        }

        public string LocalPath { get; set; }

        // song is considered HD when there is a download_url available
        public bool IsHD { get { return download_url == EffectiveDownloadUrl; } }

        //public string Sanitize(string input)
        //{
        //    Regex regex = new Regex(@"[^\w\s\d-]");
        //    return input != null ?
        //        regex.Replace(input.Replace("&amp;", "and")
        //            .Replace("&", "and").Replace(".", "_"),
        //           string.Empty)
        //        : null;
        //}

        /// <summary>
        /// Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>

        public string CoerceValidFileName(string filename, bool checkForReplaceCharacters)
        {
            if (checkForReplaceCharacters && Form1.ReplaceIllegalCharacters)
            {
                filename = AlterChars(filename);
            };

            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
                                    {
                                        "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                                        "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                                        "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
                                    };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format("^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

            return sanitisedNamePart;
        }

        public static string trimDotsAndSpacesForFolderName(string foldername)
        {
            return foldername.Trim('.',' ');
        }

        public static string staticCoerceValidFileName(string filename, bool checkForReplaceCharacters)
        {
            if (checkForReplaceCharacters && Form1.ReplaceIllegalCharacters)
            {
                filename = AlterChars(filename);
            };

            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
                                    {
                                        "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                                        "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                                        "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
                                    };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format("^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

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

        public string description { get; set; }
        public string label_name { get; set; }
        public string release { get; set; }
        public string track_type { get; set; }
        public string key_signature { get; set; }
        public string isrc { get; set; }
        public string video_url { get; set; }
        public float? bpm { get; set; }
        public int? release_year { get; set; }
        public int? release_month { get; set; }
        public int? release_day { get; set; }
        public string original_format { get; set; }
        public string license { get; set; }
        public string uri { get; set; }
        public User user { get; set; }
        public string permalink_url { get; set; }
        public string artwork_url { get; set; }
        public string waveform_url { get; set; }
        public string stream_url
        {
            get; set;
        }
        public int playback_count { get; set; }
        public int download_count { get; set; }
        public int favoritings_count { get; set; }
        public int comment_count { get; set; }
        public string attachments_uri { get; set; }
        public string policy { get; set; }
        public string download_url { get; set; }
        public Label label { get; set; }
        public string[] available_country_codes { get; set; }
        public TrackCreatedWith TrackCreatedWith { get; set; }

        public string Artist { get { return Username; } }
        public string Username
        {
            get
            {
                return user.username;
            }
            set
            {
                user.username = value;
                //user.username = Sanitize(value);
            }
        }

        public bool HasToBeDownloaded { get; set; }
    }

   

    public class Label
    {
        public int id { get; set; }
        public string kind { get; set; }
        public string permalink { get; set; }
        public string username { get; set; }
        public string last_modified { get; set; }
        public string uri { get; set; }
        public string permalink_url { get; set; }
        public string avatar_url { get; set; }
    }
}
