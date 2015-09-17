using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Soundcloud_Playlist_Downloader
{
    class metadataTagging
    {
        public static void tagIt(ref JsonPoco.Track song)
        {
            //Sets file creation time to creation time that matches with Soundcloud track
            //If somehow the datetime string can't be parsed it will just use the current (now) datetime
            DateTime dt = DateTime.Now;
            DateTime.TryParse(song.created_at, out dt);
            File.SetCreationTime(song.LocalPath, dt);

            // metadata tagging
            TagLib.File tagFile = null;

            TagLib.Id3v2.Tag.DefaultVersion = 2;
            TagLib.Id3v2.Tag.ForceDefaultVersion = true;
            // Possible values for DefaultVersion are 2(id3v2.2), 3(id3v2.3) or 4(id3v2.4)
            // it seems that id3v2.4 is more prone to misinterpret utf-8. id3v2.2 seems most stable. 
            tagFile = TagLib.File.Create(song.LocalPath);

            if (tagFile.Writeable)
            {
                //Make use of Conductor field to write soundcloud song ID and user ID (conductor field is never used anyway)
                tagFile.Tag.Conductor = "SC_SONG_ID," + song.id + ",SC_USER_ID," + song.user_id;

                //tag all other metadata fields
                tagFile.Tag.Title = song.Title;
                List<string> listGenreAndTags = new List<string>();

                if (!String.IsNullOrEmpty(song.Username))
                {
                    tagFile.Tag.AlbumArtists = new string[] { song.Username };
                    tagFile.Tag.Performers = new string[] { song.Username };
                }

                if (!String.IsNullOrEmpty(song.license))
                {
                    tagFile.Tag.Copyright = song.license;
                }

                if (!String.IsNullOrEmpty(song.genre))
                {
                    listGenreAndTags.Add(song.genre);
                    tagFile.Tag.Genres = listGenreAndTags.ToArray();
                }
                if (!String.IsNullOrEmpty(song.tag_list))
                {
                    //NOTE      Tags behave very similar as genres in SoundCloud, 
                    //          so tags will be added to the genre part of the metadata
                    //WARNING   Tags are seperated by \" when a single tag includes a whitespace! (for instance: New Wave)
                    //          Single worded tags are seperated by a single whitespace, this has led me to make
                    //          this code longer than I initially thought it would be (could perhaps made easier)
                    //FEATURES  Rare occasions, where the artist uses tags that include the seperation tags SoundCloud uses;
                    //          like \" or \"Hip-Hop\", are handled, but NOT necessary, because the quote (") is an illegal sign to use in tags

                    string tag = "";
                    bool partOfLongertag = false;

                    foreach (string word in song.tag_list.Split(' '))
                    {
                        if (word.EndsWith("\""))
                        {
                            tag += " " + word.Substring(0, word.Length - 1);
                            partOfLongertag = false;
                            listGenreAndTags.Add(tag);
                            tag = "";
                            continue;
                        }
                        else if (word.StartsWith("\""))
                        {
                            partOfLongertag = true;
                            tag += word.Substring(1, word.Length - 1);
                        }
                        else if (partOfLongertag == true)
                        {
                            tag += " " + word;
                        }
                        else
                        {
                            tag = word;
                            listGenreAndTags.Add(tag);
                            tag = "";
                        }
                    }
                    tagFile.Tag.Genres = listGenreAndTags.ToArray();
                }

                if (!String.IsNullOrEmpty(song.description))
                {
                    tagFile.Tag.Comment = song.description;
                }
                if (!String.IsNullOrEmpty(song.artwork_url))
                {
                    getArtwork(ref tagFile, ref song);
                }
                tagFile.Save();
                tagFile.Dispose();

                
            }
        }

        public static void getArtwork(ref TagLib.File tagFile, ref JsonPoco.Track song)
        {
            // download artwork
            string artworkFilepath = Path.GetTempFileName();

            string highResArtwork_url = song.artwork_url.Replace("large.jpg", "t500x500.jpg");
            for (int attempts = 0; attempts < 5; attempts++)
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        web.DownloadFile(highResArtwork_url, artworkFilepath);
                    }
                    TagLib.Picture artwork = new TagLib.Picture(artworkFilepath);
                    artwork.Type = TagLib.PictureType.FrontCover;
                    tagFile.Tag.Pictures = new[] { artwork };
                    break;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                System.Threading.Thread.Sleep(50); // Pause 50ms before new attempt
            }

            if (artworkFilepath != null && File.Exists(artworkFilepath))
            {
                File.Delete(artworkFilepath);
            }
        }
    }
}
