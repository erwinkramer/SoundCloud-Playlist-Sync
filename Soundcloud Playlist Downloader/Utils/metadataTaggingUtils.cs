using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Soundcloud_Playlist_Downloader.JsonObjects;
using TagLib;
using File = TagLib.File;
using Tag = TagLib.Id3v2.Tag;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class MetadataTaggingUtils
    {      
        public static void TagIt(Track song)
        {
            // metadata tagging
            Tag.DefaultVersion = 3;
            Tag.ForceDefaultVersion = true;
            // Possible values for DefaultVersion are 2(id3v2.2), 3(id3v2.3) or 4(id3v2.4)
            // it seems that id3v2.4 is more prone to misinterpret utf-8. id3v2.3 seems most stable. 

            File tagFile = File.Create(song.LocalPath, ReadStyle.None);
            
            var creationDate = DateTime.Today; //If somehow the datetime string can't be parsed it will just use today
            if (!string.IsNullOrEmpty(song.created_at))
            {
                DateTime.TryParse(song.created_at, out creationDate);
            }

            if (tagFile.Writeable)
            {
                //Make use of Conductor field to write soundcloud song ID and user ID for future features (conductor field is never used anyway)
                tagFile.Tag.Conductor = "SC_SONG_ID," + song.id + ",SC_USER_ID," + song.user_id;

                //tag all other metadata fields
                tagFile.Tag.Title = song.Title;
                tagFile.Tag.Year = Convert.ToUInt32(creationDate.Year);

                if (!string.IsNullOrEmpty(song.Username))
                {
                    tagFile.Tag.AlbumArtists = new[] {song.Username};
                    tagFile.Tag.Performers = new[] {song.Username};
                }
                else
                {
                    tagFile.Tag.AlbumArtists = new[] {"<blank>"}; //<blank> is how an empty user shows up in SoundCloud
                    tagFile.Tag.Performers = new[] {"<blank>"};
                }

                if (song.bpm.HasValue)
                {
                    var bpm = (float) song.bpm;
                    tagFile.Tag.BeatsPerMinute = Convert.ToUInt32(bpm);
                }
                if (!string.IsNullOrEmpty(song.permalink_url))
                {
                    tagFile.Tag.Composers = new[] {song.permalink_url};
                }

                if (!string.IsNullOrEmpty(song.license))
                {
                    tagFile.Tag.Copyright = song.license;
                }

                if (!string.IsNullOrEmpty(song.genre))
                {
                    tagFile.Tag.Genres = new []{song.genre};
                }
                if (!string.IsNullOrEmpty(song.tag_list))
                {
                    tagFile.Tag.Genres = BuildTagList(song).ToArray();
                }
                if (!string.IsNullOrEmpty(song.description))
                {
                    tagFile.Tag.Comment = song.description;
                }
                if (!string.IsNullOrEmpty(song.artwork_url))
                {
                    GetArtwork(ref tagFile, ref song);
                }
                else
                {
                    GetAvatarImg(ref tagFile, ref song);
                }
                tagFile.Save();
                tagFile.Dispose();
            }

            // Sets file creation time to creation time that matches with Soundcloud track
            System.IO.File.SetCreationTime(song.LocalPath, creationDate);
            // Set last write time to original file creation date
            System.IO.File.SetLastWriteTime(song.LocalPath, creationDate);         
        }

        private static List<string> BuildTagList(Track song)
        {
            var listTags = new List<string>();
            if (!string.IsNullOrEmpty(song.genre))
            {
                listTags.Add(song.genre);
            }
            //NOTE      Tags behave very similar as genres in SoundCloud, 
            //          so tags will be added to the genre part of the metadata
            //WARNING   Tags are seperated by \" when a single tag includes a whitespace! (for instance: New Wave)
            //          Single worded tags are seperated by a single whitespace, this has led me to make
            //          this code longer than I initially thought it would be (could perhaps made easier)
            //FEATURES  Rare occasions, where the artist uses tags that include the seperation tags SoundCloud uses;
            //          like \" or \"Hip-Hop\", are handled, but NOT necessary, because the quote (") is an illegal sign to use in tags

            var tag = "";
            var partOfLongertag = false;

            foreach (var word in song.tag_list.Split(' '))
            {
                if (word.EndsWith("\""))
                {
                    tag += " " + word.Substring(0, word.Length - 1);
                    partOfLongertag = false;
                    listTags.Add(tag);
                    tag = "";
                }
                else if (word.StartsWith("\""))
                {
                    partOfLongertag = true;
                    tag += word.Substring(1, word.Length - 1);
                }
                else if (partOfLongertag)
                {
                    tag += " " + word;
                }
                else
                {
                    tag = word;
                    listTags.Add(tag);
                    tag = "";
                }
            }
            return listTags;
        }

        public static void GetAvatarImg(ref File tagFile, ref Track song)
        {
            //download user profile avatar image
            var avatarFilepath = Path.GetTempFileName();

            var highResAvatarUrl = song.user.avatar_url.Replace("large.jpg", "t500x500.jpg");
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {
                    using (var download = DownloadUtils.httpClient.GetAsync(highResAvatarUrl).Result)
                    using (var fs = new FileStream(avatarFilepath, FileMode.Create))
                    {
                        download.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                    }
                    var artwork = new Picture(avatarFilepath) {
                        Type = PictureType.FrontCover,
                        Description = "cover"
                };
                    tagFile.Tag.Pictures = new [] {artwork};
                    break;
                }
                catch (Exception)
                {
                    // ignored
                }
                Thread.Sleep(50); // Pause 50ms before new attempt
            }

            if (System.IO.File.Exists(avatarFilepath))
            {
                System.IO.File.Delete(avatarFilepath);
            }
        }

        public static void GetArtwork(ref File tagFile, ref Track song)
        {
            // download artwork
            var artworkFilepath = Path.GetTempFileName();

            var highResArtworkUrl = song.artwork_url.Replace("large.jpg", "t500x500.jpg");
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {
                    using (var download = DownloadUtils.httpClient.GetAsync(highResArtworkUrl).Result)
                    using (var fs = new FileStream(artworkFilepath, FileMode.Create))
                    {
                        download.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                    }
                    var artwork = new Picture(artworkFilepath)
                    {
                        Type = PictureType.FrontCover,
                        Description = "cover"
                    };
                    tagFile.Tag.Pictures = new [] {artwork};
                    break;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                Thread.Sleep(50); // Pause 50ms before new attempt
            }

            if (System.IO.File.Exists(artworkFilepath))
            {
                System.IO.File.Delete(artworkFilepath);
            }
        }
    }
}