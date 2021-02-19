using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Soundcloud_Playlist_Downloader.JsonObjects;
using System.Linq;
using Soundcloud_Playlist_Downloader.Language;
using System.Net.Http;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class DownloadUtils
    {
        public bool ConvertToMp3;
        public bool Highqualitysong;
        public bool ExcludeM4A;
        public bool ExcludeAac;
        ManifestUtils ManifestUtil;
        public int ConcurrentDownloads;
        public ClientIDsUtils ClientIDsUtil;
        public static HttpClient httpClient = new HttpClient();
        public static HttpClient httpClientWithBrowserheaders = CreateHttpClientWithBrowserheaders();
       
        public DownloadUtils(ClientIDsUtils clientIDsUtil, bool excludeM4A, bool excludeAac, bool convertToMp3, ManifestUtils manifestUtil, bool highqualitysong, int concurrentDownloads)
        {
            ExcludeM4A = excludeM4A;
            ExcludeAac = excludeAac;
            Highqualitysong = highqualitysong;
            ManifestUtil = manifestUtil;
            ConvertToMp3 = convertToMp3;
            ClientIDsUtil = clientIDsUtil;
            ConcurrentDownloads = concurrentDownloads;
        }   

        public static HttpClient CreateHttpClientWithBrowserheaders()
        {
            var httpClientWithBrowserheaders = new HttpClient();
            httpClientWithBrowserheaders.DefaultRequestHeaders.Add("Accept-Language", "en-US");
            httpClientWithBrowserheaders.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", "2-291834-4570680-lpD214XFqfKQZS6d");
            httpClientWithBrowserheaders.DefaultRequestHeaders.Add("Accept", "text/html, application/xhtml+xml, */*");
            httpClientWithBrowserheaders.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36");
            return httpClientWithBrowserheaders;
        }

        public void DownloadSongs(IList<Track> tracksToDownload, CancellationTokenSource syncCancellationSource)
        {
            Interlocked.Add(ref ManifestUtil.ProgressUtil.SongsToDownload, tracksToDownload.Count);

            if (ManifestUtil.ProgressUtil.SongsToDownload == 0) return;
            if (ConcurrentDownloads == 0)
                throw new Exception(LanguageManager.Language["STR_DOWNLOAD_SONG_EX"]);
            var po = new ParallelOptions
            {
                CancellationToken = syncCancellationSource.Token,
                MaxDegreeOfParallelism = ConcurrentDownloads
            };
            try
            {
                Parallel.ForEach(tracksToDownload, po,
                    track =>
                    {
                        try
                        {
                            po.CancellationToken.ThrowIfCancellationRequested();
                            ManifestUtil.ProgressUtil.AddOrUpdateInProgressTrack(track);

                            if (!DownloadTrackAndTag(ref track))
                            {
                                ManifestUtil.ProgressUtil.AddOrUpdateNotDownloadableTrack(track);
                                Interlocked.Decrement(ref ManifestUtil.ProgressUtil.SongsToDownload);
                                return;
                            }

                            track.IsDownloaded = true;
                            ManifestUtil.ProgressUtil.AddOrUpdateSuccessFullTrack(track);
                            ProcessUpdateManifestDelegate pumd = ManifestUtil.UpdateManifest;
                            pumd(track);
                        }
                        catch (Exception e)
                        {                          
                            ManifestUtil.ProgressUtil.AddOrUpdateFailedTrack(track, e);
                            ManifestUtil.FileSystemUtil.LogTrackWithError(track, e);

                            double exceptionPercentage = ((double)ManifestUtil.ProgressUtil.CurrentAmountOfExceptions / (double)ManifestUtil.ProgressUtil.SongsProcessing) * 100;
                            if (exceptionPercentage  >= ProgressUtils.MaximumExceptionThreshHoldPercentage)
                            {
                                ManifestUtil.ProgressUtil.HasErrors = true;
                                syncCancellationSource.Cancel();
                            }
                        }
                    });
            }
            catch (OperationCanceledException e)
            {
                ManifestUtil.ProgressUtil.ThrowAllExceptionsWithRootException(e);
            }
            finally
            {
            }
        }

        public string GetEffectiveDownloadUrlForStream(int id)
        {
            var track = new JsonUtils(ManifestUtil, ClientIDsUtil.ClientIdCurrentValue).RetrieveJsonTrackFromV2Url(id);
            foreach(var transcoding in track.media.transcodings)
            {
                if (transcoding.format.protocol == "hls" && transcoding.format.mime_type == "audio/mpeg")
                {
                    return new JsonUtils(ManifestUtil, ClientIDsUtil.ClientIdCurrentValue).GetDownloadUrlFromProgressiveUrl(transcoding.url);
                }
            }
            return null;
        }

        public string GetEffectiveDownloadUrlForHQ(string downloadUrl, bool downloadable)
        {
            if (Highqualitysong && !string.IsNullOrWhiteSpace(downloadUrl) && downloadable) //user has selected to download high quality songs if available
            {
                return RemoveCarriageReturnAndLineFeed(downloadUrl + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}");
            }
            return null;
        }

        public string RemoveCarriageReturnAndLineFeed(string value)
        {
            return value.Replace("\r", "").Replace("\n", "");
        }

        public bool DownloadTrackAndTag(ref Track song)
        {
            if (song?.LocalPath == null)
                return false;
            Directory.CreateDirectory(Path.GetDirectoryName(song.LocalPath));

            song.EffectiveDownloadUrl = GetEffectiveDownloadUrlForHQ(song.download_url, song.downloadable);
            string extension = DetermineExtension(song);
            var allowedExtension = DetermineAllowedFormats().Contains(extension);

            bool succesfulConvert = false;
            if (!string.IsNullOrWhiteSpace(song.EffectiveDownloadUrl) && song.IsHD && ConvertToMp3 && allowedExtension)
            {
                //get the wav song as byte data, as we won't store it just yet
                var soundbytes = httpClient.GetByteArrayAsync(song.EffectiveDownloadUrl).Result;
                //convert to mp3 & then write bytes to file
                succesfulConvert = AudioConverterUtils.ConvertAllTheThings(soundbytes, ref song, extension);
            }
                
            if(!succesfulConvert) 
            {
                song.EffectiveDownloadUrl = GetEffectiveDownloadUrlForStream(song.id);
                if (string.IsNullOrWhiteSpace(song.EffectiveDownloadUrl))
                    return false;

                song.LocalPath += ".mp3";
                using (var download = httpClient.GetAsync(song.EffectiveDownloadUrl).Result)
                using (var fs = new FileStream(song.LocalPath, FileMode.Create))
                {
                    download.Content.CopyToAsync(fs).GetAwaiter().GetResult();
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

        public static string DetermineExtension(Track song)
        {
            if (!song.downloadable)
                return ".mp3";
            try
            {
                return GetExtensionFromWebRequest(song.EffectiveDownloadUrl);
            }
            catch (Exception)
            {
                // ignored
            }
            return ".mp3";
        }

        public bool IsDownloadable(string downloadUrl)
        {
            return IsValidUrl(downloadUrl + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}");
        }

        public static bool IsValidUrl(string url)
        {
            try
            {
                if (httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result.StatusCode == HttpStatusCode.OK)
                    return true;
            }
            catch (WebException)
            {
            }
            return false;
        }


        public static string GetExtensionFromWebRequest(string requestUrl)
        {
            string extension = "";
            var result = httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead).Result;
            try
            {
                extension = Path.GetExtension(result.Content.Headers.ContentDisposition.FileName.Replace("\"", String.Empty));
            }
            catch (FormatException)
            {
                //If it fails to get extention from disposition (if ContentDisposition works it gives more reliable results)
                extension = $".{result.Content.Headers.GetValues("x-amz-meta-file-type").First().Replace("\"", String.Empty)}";
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
                ManifestUtil.ProgressUtil.HasErrors = true;
                throw new Exception("Invalid profile url: " + e.Message);
            }
        }

        public static string GetPlaylistIdFromHTML(string url)
        {
            var document = DownloadPageFromUrl(url);
            foreach (var node in document.DocumentNode.SelectNodes("/html/head/meta"))
            {
                if (node.Attributes["property"]?.Value == "al:android:url")
                {
                    return new String(node.Attributes["content"].Value.Where(Char.IsDigit).ToArray());
                }
            }
            return null;
        }

        public static HtmlAgilityPack.HtmlDocument DownloadPageFromUrl(string url)
        {
            var doc = new HtmlAgilityPack.HtmlDocument(); 
            doc.LoadHtml(httpClientWithBrowserheaders.GetStringAsync(url).Result);
            return doc;
        }
    }
}
