using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Properties;
using Soundcloud_Playlist_Downloader.Views;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Configuration;
using Soundcloud_Playlist_Downloader.Utils;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class DownloadUtils
    {
        public bool ConvertToMp3;
        public bool Highqualitysong;
        public bool ExcludeM4A;
        public bool ExcludeAac;
        ManifestUtils ManifestUtil;
        public ClientIDsUtils ClientIDsUtil;
        public DownloadUtils(ClientIDsUtils clientIDsUtil, bool excludeM4A, bool excludeAac, bool convertToMp3, ManifestUtils manifestUtil, bool highqualitysong)
        {
            ExcludeM4A = excludeM4A;
            ExcludeAac = excludeAac;
            Highqualitysong = highqualitysong;
            ManifestUtil = manifestUtil;
            ConvertToMp3 = convertToMp3;
            ClientIDsUtil = clientIDsUtil;
        }   

        public void DownloadSongs(IList<Track> tracksToDownload)
        {
            var trackProgress = new ConcurrentDictionary<string, string>();

            ManifestUtil.ProgressUtil.SongsToDownload = tracksToDownload.Count;
            if (ManifestUtil.ProgressUtil.SongsToDownload == 0) return;
            var exceptions = new ConcurrentQueue<Exception>();
            var cts = new CancellationTokenSource();
            if (Settings.Default.ConcurrentDownloads == 0)
                throw new Exception("Number for concurrent downloads must be at least 1");
            var po = new ParallelOptions
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
                            trackProgress.AddOrUpdate(track.id.ToString(), "[~] " + track.Title, (key, oldValue) => track.Title);
                            ManifestUtil.ProgressUtil.TrackProgress = trackProgress.Values;

                            if (!DownloadTrackAndTag(ref track)) return;
                            track.IsDownloaded = true;
                            trackProgress.AddOrUpdate(track.id.ToString(), track.Title, (key, oldValue) => "[✓] " + track.Title);
                            ManifestUtil.ProgressUtil.TrackProgress = trackProgress.Values;

                            ProcessUpdateManifestDelegate pumd = ManifestUtil.UpdateManifest;
                            pumd(track);
                        }
                        catch (Exception e)
                        {
                            var exc = new Exception($"Exception while downloading track '{track.Title}' from artist '{track.Artist}'", e);
                            exceptions.Enqueue(exc);
                            trackProgress.AddOrUpdate(track.id.ToString(),  track.Title, (key, oldValue) => "[X] " + track.Title);
                            ManifestUtil.ProgressUtil.TrackProgress = trackProgress.Values;
                            //EventLog.WriteEntry(Application.ProductName, exc.ToString());
                            ManifestUtil.ProgressUtil.IsError = true;
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

        public string GetEffectiveDownloadUrl(string streamUrl, string downloadUrl, int id, bool downloadable)
        {
            string url;
            if (string.IsNullOrWhiteSpace(streamUrl))
            {
                //WARNING       On rare occaisions the stream url is not available, blame this on the SoundCloud API
                //              We can manually create the stream url anyway because we have the song id
                //NOTE          This shouldn't be necessary anymore, since we changed the client_id to another one that actually works
                streamUrl = $"https://api.soundcloud.com/tracks/{id}/stream";
            }
            if (Highqualitysong) //user has selected to download high quality songs if available
            {
                url = !string.IsNullOrWhiteSpace(downloadUrl) && downloadable
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

        public bool DownloadTrackAndTag(ref Track song)
        {
            if (!ManifestUtil.ProgressUtil.IsActive) return false;
            if (song?.LocalPath == null)
                return false;
            Directory.CreateDirectory(Path.GetDirectoryName(song.LocalPath));

            using (var client = new WebClient())
            {               
                if (song.IsHD)
                {
                    string extension = DetermineExtension(song);

                    if (ConvertToMp3 && Highqualitysong &&
                        DetermineAllowedFormats().Contains(extension))
                    {
                        //get the wav song as byte data, as we won't store it just yet
                        var soundbytes = client.DownloadData(song.EffectiveDownloadUrl +
                                                             $"?client_id={ClientIDsUtil.ClientIdCurrentValue}");
                        //convert to mp3 & then write bytes to file
                        var succesfulConvert = AudioConverterUtils.ConvertAllTheThings(soundbytes, ref song, extension);
                        if (!succesfulConvert)
                            //something has gone wrong, download the stream url instead of download url 
                        {
                            song.LocalPath += ".mp3";
                            client.DownloadFile(song.stream_url + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}", song.LocalPath);
                        }
                    }
                    else if (extension == ".mp3") //get the high res mp3 without converting
                    {
                        song.LocalPath += extension;
                        client.DownloadFile(song.EffectiveDownloadUrl + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}", song.LocalPath);
                    }
                    else //get the low res mp3 if all above not possible
                    {
                        song.LocalPath += ".mp3";
                        client.DownloadFile(song.stream_url + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}", song.LocalPath);
                    }
                }
                else
                {
                    song.LocalPath += ".mp3";
                    client.DownloadFile(song.stream_url + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}", song.LocalPath);
                }
            }
            MetadataTaggingUtils.TagIt(song);
            Interlocked.Increment(ref ManifestUtil.ProgressUtil.SongsDownloaded);
            return true;
        }

        private List<string> DetermineAllowedFormats()
        {
            var formats = new List<string>
            {
                ".wav", ".aiff", ".aif", ".m4a", ".aac"            
            };
            if (ExcludeAac)
                formats.Remove(".aac");
            if (ExcludeM4A)
                formats.Remove(".m4a");
            return formats;
        }

        public string DetermineExtension(Track song)
        {
            try
            {
                WebRequest requestEffectiveDownloadUrl = WebRequest.Create(song.EffectiveDownloadUrl + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}");
                return GetExtensionFromWebRequest(requestEffectiveDownloadUrl);
            }
            catch (Exception)
            {
                // ignored
            }
            if (song.stream_url == null)
                //all hope is lost when there is no stream url, return to safety
                return "";

            var requeststreamUrl = WebRequest.Create(song.stream_url + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}");
            return GetExtensionFromWebRequest(requeststreamUrl);
        }
        public static string GetExtensionFromWebRequest(WebRequest request)
        {
            string extension = "";
            request.Method = "HEAD";
            using (var response = request.GetResponse())
            {
                try
                {
                    var disposition = new ContentDisposition(response.Headers["Content-Disposition"]);
                    extension = Path.GetExtension(disposition.FileName);
                }
                catch (FormatException)
                {
                    //If it fails to get extention from disposition (if ContentDisposition works it gives more reliable results)
                    extension = $".{response.Headers["x-amz-meta-file-type"]}";
                }
            }
            return extension;
        }

        public string ParseUserIdFromProfileUrl(string url)
        {        
            return new JsonUtils(ManifestUtil, ClientIDsUtil.ClientIdCurrentValue).RetrieveUserIdFromUserName(GetUserNameFromProfileUrl(url));
        }
        
        private string GetUserNameFromProfileUrl(string url)
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
                ManifestUtil.ProgressUtil.IsError = true;
                throw new Exception("Invalid profile url: " + e.Message);
            }
        }

        public static string GetPlaylistIdFromHTML(string url)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            using (WebClient client = new WebClient())           
            {
                client.Headers.Add("Accept-Language", " en-US");
                client.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
                doc.LoadHtml(client.DownloadString(url));
            }
            foreach (var node in doc.DocumentNode.SelectNodes("/html/head/meta"))
            {
                if (node.Attributes["property"]?.Value == "al:android:url")
                {
                    return new String(node.Attributes["content"].Value.Where(Char.IsDigit).ToArray());
                }
            }
            return null;
        }
    }
}
