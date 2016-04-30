using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Properties;
using Soundcloud_Playlist_Downloader.Views;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class DownloadUtils
    {
        //OLD CLIENT_ID = "93a4fae1bd98b84c9b4f6bf1cc838b4f";
        //NEW key should fix same reason as stated here: 
        //https://stackoverflow.com/questions/29914622/get-http-mp3-stream-from-every-song/30018216#30018216
        public const string ClientId = "376f225bf427445fc4bfb6b99b72e0bf";
        public static bool IsActive { get; set; }
        public static int SongsToDownload { get; set; }
        public static int SongsDownloaded = 0;

        public static void DownloadSongs(IList<Track> tracksToDownload, string apiKey, string directoryPath)
        {
            SongsToDownload = tracksToDownload.Count;
            if (SongsToDownload == 0) return;
            var exceptions = new ConcurrentQueue<Exception>();
            CancellationTokenSource cts = new CancellationTokenSource();
            ParallelOptions po = new ParallelOptions
            {
                CancellationToken = cts.Token,
                MaxDegreeOfParallelism = Settings.Default.ConcurrentDownloads
            };
            try
            {
                Parallel.ForEach(tracksToDownload, po,
                    track =>
                    {
                        try
                        {
                            if (!DownloadTrackAndTag(ref track, apiKey)) return;
                            track.IsDownloaded = true;
                            ProcessUpdateManifestDelegate pumd = ManifestUtils.UpdateManifest;
                            pumd(track, directoryPath);
                        }
                        catch (Exception e)
                        {
                            exceptions.Enqueue(e);
                            SoundcloudSync.IsError = true;
                            po.CancellationToken.ThrowIfCancellationRequested();
                            cts.Cancel();
                        }
                    });
            }
            catch (OperationCanceledException e)
            {
                if (exceptions.Count > 0) throw new AggregateException(e.Message, exceptions);
            }
            finally
            {
                cts.Dispose();
            }
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
            if (SoundcloudSyncMainForm.Highqualitysong) //user has selected to download high quality songs if available
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

        public static bool DownloadTrackAndTag(ref Track song, string apiKey)
        {
            if (!IsActive) return false;
            if (song?.LocalPath == null)
                return false;
            Directory.CreateDirectory(Path.GetDirectoryName(song.LocalPath));

            using (var client = new WebClient())
            {               
                if (song.IsHD)
                {
                    string extension = DetermineExtension(song, apiKey);

                    if (SoundcloudSyncMainForm.ConvertToMp3 && SoundcloudSyncMainForm.Highqualitysong &&
                        DetermineAllowedFormats().Contains(extension))
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
            }
            MetadataTaggingUtils.TagIt(ref song);
            Interlocked.Increment(ref SongsDownloaded);
            return true;
        }

        private static List<string> DetermineAllowedFormats()
        {
            var formats = new List<string>
            {
                ".wav", ".aiff", ".aif", ".m4a", ".aac"            
            };
            if (SoundcloudSyncMainForm.ExcludeAac)
                formats.Remove(".aac");
            if (SoundcloudSyncMainForm.ExcludeM4A)
                formats.Remove(".m4a");
            return formats;
        }

        public static string DetermineExtension(Track song, string apiKey)
        {
            try
            {
                WebRequest requestEffectiveDownloadUrl = WebRequest.Create(song.EffectiveDownloadUrl + $"?client_id={apiKey}");
                return GetExtensionFromWebRequest(requestEffectiveDownloadUrl);
            }
            catch (Exception e)
            {
                // ignored
            }
            if (song.stream_url == null)
                //all hope is lost when there is no stream url, return to safety
                return "";

            var requeststreamUrl = WebRequest.Create(song.stream_url + $"?client_id={apiKey}");
            return GetExtensionFromWebRequest(requeststreamUrl);
        }
        public static string GetExtensionFromWebRequest(WebRequest request)
        {
            string extension;
            request.Method = "HEAD";
            using (var response = request.GetResponse())
            {
                extension = "." + response.Headers["x-amz-meta-file-type"];
            }
            return extension;
        }

        public static string ParseUserIdFromProfileUrl(string url)
        {
            try
            {
                var startingPoint = "soundcloud.com/";
                var startingIndex = url.IndexOf(startingPoint, StringComparison.Ordinal) + startingPoint.Length;
                var endingIndex = url.Substring(startingIndex).Contains("/")
                    ? url.Substring(startingIndex).IndexOf("/", StringComparison.Ordinal) + startingIndex
                    : url.Length;

                return url.Substring(startingIndex, endingIndex - startingIndex);
            }
            catch (Exception e)
            {
                SoundcloudSync.IsError = true;
                throw new Exception("Invalid profile url: " + e.Message);
            }
        }
    }
}
