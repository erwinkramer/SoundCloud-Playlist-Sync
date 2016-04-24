using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Soundcloud_Playlist_Downloader.JsonObjects;
using TagLib;
using File = TagLib.File;
using Tag = TagLib.Id3v2.Tag;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class MetadataTaggingUtils
    {
        public static void ReTag(ref Track retagTrack, Track compareTrack)
        {
            retagTrack.ModifiedDateTimeUtc = DateTime.UtcNow;
            retagTrack.Title = compareTrack.Title;
            retagTrack.artwork_url = compareTrack.artwork_url;
            retagTrack.favoritings_count = compareTrack.favoritings_count;
            TagIt(retagTrack);
        }
        public static void TagIt(Track song)
        {
            // metadata tagging
            File tagFile = null;

            Tag.DefaultVersion = 2;
            Tag.ForceDefaultVersion = true;
            // Possible values for DefaultVersion are 2(id3v2.2), 3(id3v2.3) or 4(id3v2.4)
            // it seems that id3v2.4 is more prone to misinterpret utf-8. id3v2.2 seems most stable. 
            tagFile = File.Create(song.LocalPath);

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

                var listGenreAndTags = new List<string>();

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
                    listGenreAndTags.Add(song.genre);
                    tagFile.Tag.Genres = listGenreAndTags.ToArray();
                }
                if (!string.IsNullOrEmpty(song.tag_list))
                {
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
                            listGenreAndTags.Add(tag);
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
                            listGenreAndTags.Add(tag);
                            tag = "";
                        }
                    }
                    tagFile.Tag.Genres = listGenreAndTags.ToArray();
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

        public static void GetAvatarImg(ref File tagFile, ref Track song)
        {
            //download user profile avatar image
            var avatarFilepath = Path.GetTempFileName();

            var highResAvatarUrl = song.user.avatar_url.Replace("large.jpg", "t500x500.jpg");
            for (var attempts = 0; attempts < 5; attempts++)
            {
                try
                {
                    using (var web = new WebClient())
                    {
                        web.DownloadFile(highResAvatarUrl, avatarFilepath);
                    }
                    var artwork = new Picture(avatarFilepath) {Type = PictureType.FrontCover};
                    tagFile.Tag.Pictures = new IPicture[] {artwork};
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
                    using (var web = new WebClient())
                    {
                        web.DownloadFile(highResArtworkUrl, artworkFilepath);
                    }
                    var artwork = new Picture(artworkFilepath) {Type = PictureType.FrontCover};
                    tagFile.Tag.Pictures = new IPicture[] {artwork};
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