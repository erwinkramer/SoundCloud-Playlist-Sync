using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Soundcloud_Playlist_Downloader.Properties;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class DownloadUtils
    {
        //private string CLIENT_ID = "93a4fae1bd98b84c9b4f6bf1cc838b4f";
        //new key should fix same reason as stated here: 
        //https://stackoverflow.com/questions/29914622/get-http-mp3-stream-from-every-song/30018216#30018216
        public const string ClientId = "376f225bf427445fc4bfb6b99b72e0bf";
        public static bool IsActive { get; set; }
        public static int SongsToDownload { get; set; }
        public static int SongsDownloaded { get; set; }
        private static readonly object SongsDownloadedLock = new object();

        public static void DownloadSongs(IList<Track> alltracks, string apiKey, string directoryPath)
        {
            var trackLock = new object();
            SongsToDownload = alltracks.Count(x => x.HasToBeDownloaded);
            Parallel.ForEach(alltracks.Where(x => x.HasToBeDownloaded),
                new ParallelOptions { MaxDegreeOfParallelism = Settings.Default.ConcurrentDownloads },
                track =>
                {
                    try
                    {
                        if (!DownloadTrack(track, apiKey)) return;
                        lock (trackLock)
                        {
                            track.HasToBeDownloaded = false;
                            ManifestUtils.UpdateSyncManifest(track, directoryPath);
                        }
                    }
                    catch (Exception e)
                    {
                        PlaylistSync.IsError = true;
                        ExceptionHandlerUtils.HandleException(e);
                    }
                });
        }

        public static string GetEffectiveDownloadUrl(string streamUrl, string downloadUrl, int id)
        {
            string url;
            if (streamUrl == null)
            {
                //WARNING       On rare occaisions the stream url is not available, blame this on the SoundCloud API
                //              We can manually create the stream url anyway because we have the song id
                //NOTE          This shouldn't be necessary anymore, since we changed the client_id to another one that actually works
                streamUrl = $"https://api.soundcloud.com/tracks/{id}/stream";
            }
            if (Form1.Highqualitysong) //user has selected to download high quality songs if available
            {
                url = !string.IsNullOrWhiteSpace(downloadUrl)
                    ? downloadUrl
                    : streamUrl; //check if high quality url (download_url) is available
            }
            else
            {
                url = streamUrl; //else just get the low quality MP3 (stream_url)
            }
            if (!string.IsNullOrWhiteSpace(url))
            {
                return url.Replace("\r", "").Replace("\n", "");
            }
            return null;
        }

        public static bool DownloadTrack(Track song, string apiKey)
        {
            if (!IsActive) return false;

            using (var client = new WebClient())
            {
                if (song?.LocalPath != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(song.LocalPath));

                    if (song.IsHD)
                    {
                        string extension = null;
                        try
                        {
                            var request = WebRequest.Create(song.EffectiveDownloadUrl +
                                                            $"?client_id={apiKey}");
                            request.Method = "HEAD";
                            using (var response = request.GetResponse())
                            {
                                extension = "." + response.Headers["x-amz-meta-file-type"];
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionHandlerUtils.HandleException(e);

                            //the download link might have been invalid, so we get the stream download instead
                            if (song.stream_url == null)
                                //all hope is lost when there is no stream url, return to safety
                                return false;

                            var request = WebRequest.Create(song.stream_url + $"?client_id={apiKey}");

                            request.Method = "HEAD";
                            using (var response = request.GetResponse())
                            {
                                extension = "." + response.Headers["x-amz-meta-file-type"];
                            }
                        }
                        var allowedFormats = new List<string>();
                        allowedFormats.AddRange(new[] { ".wav", ".aiff", ".aif", ".m4a", ".aac" });
                        if (Form1.excludeAAC)
                        {
                            allowedFormats.Remove(".aac");
                        }
                        if (Form1.excludeM4A)
                        {
                            allowedFormats.Remove(".m4a");
                        }
                        if (Form1.ConvertToMp3 && Form1.Highqualitysong && allowedFormats.Contains(extension))
                        {
                            //get the wav song as byte data, as we won't store it just yet
                            var soundbytes = client.DownloadData(song.EffectiveDownloadUrl +
                                                                 $"?client_id={apiKey}");
                            //convert to mp3 & then write bytes to file
                            var succesfulConvert = AudioConverterUtils.ConvertAllTheThings(soundbytes, ref song, extension);
                            if (!succesfulConvert)
                            //something has gone wrong, download the stream url instead of download url 
                            {
                                song.LocalPath += ".mp3";
                                client.DownloadFile(song.stream_url + $"?client_id={apiKey}", song.LocalPath);
                            }
                        }
                        else if (extension == ".mp3") //get the high res mp3 without converting
                        {
                            song.LocalPath += extension;
                            client.DownloadFile(song.EffectiveDownloadUrl + $"?client_id={apiKey}", song.LocalPath);
                        }
                        else //get the low res mp3 if all above not possible
                        {
                            song.LocalPath += extension;
                            client.DownloadFile(song.stream_url + $"?client_id={apiKey}", song.LocalPath);
                        }
                    }
                    else
                    {
                        song.LocalPath += ".mp3";
                        client.DownloadFile(song.stream_url + $"?client_id={apiKey}", song.LocalPath);
                    }
                    try
                    {
                        MetadataTaggingUtils.TagIt(ref song);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandlerUtils.HandleException(e);
                    }
                }
                lock (SongsDownloadedLock)
                {
                    SongsDownloaded++;
                }
            }
            return true;
        }
    }
}
