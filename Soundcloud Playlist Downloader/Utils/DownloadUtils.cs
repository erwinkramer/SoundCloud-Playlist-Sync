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
        public bool ChooseHighqualitysong;
        public bool ExcludeM4A;
        public bool ExcludeAac;
        ManifestUtils ManifestUtil;
        public int ConcurrentDownloads;
        public ClientIDsUtils ClientIDsUtil;
        public static HttpClient httpClient = new HttpClient();
       
        public DownloadUtils(ClientIDsUtils clientIDsUtil, bool excludeM4A, bool excludeAac, bool convertToMp3, ManifestUtils manifestUtil, bool highqualitysong, int concurrentDownloads)
        {
            ExcludeM4A = excludeM4A;
            ExcludeAac = excludeAac;
            ChooseHighqualitysong = highqualitysong;
            ManifestUtil = manifestUtil;
            ConvertToMp3 = convertToMp3;
            ClientIDsUtil = clientIDsUtil;
            ConcurrentDownloads = concurrentDownloads;
        }   

        public static HttpRequestMessage RequestMessageWithHeaders(string uri, HttpMethod method, string oAuthToken = null)
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = method;
            httpRequestMessage.RequestUri = new Uri(uri);
            httpRequestMessage.Headers.Add("Accept-Language", "en-US");
            httpRequestMessage.Headers.Add("Accept", "text/html, application/xhtml+xml, */*");
            httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36");
            
            if(oAuthToken != null)
                httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", oAuthToken);
            
            return httpRequestMessage;
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

                            DownloadTrackAndTag(ref track);             
                            //if not downloadable
                            //ManifestUtil.ProgressUtil.AddOrUpdateNotDownloadableTrack(track);
                            //Interlocked.Decrement(ref ManifestUtil.ProgressUtil.SongsToDownload);

                            track.IsDownloaded = true;
                            ManifestUtil.ProgressUtil.AddOrUpdateSuccessFullTrack(track);
                            ProcessUpdateManifestDelegate pumd = ManifestUtil.UpdateManifest;
                            pumd(track);
                        }
                        catch (Exception e)
                        {
                            ManifestUtil.ProgressUtil.AddOrUpdateFailedTrack(track, e);
                            ManifestUtil.FileSystemUtil.LogTrackException(track, e);

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
            var track = new JsonUtils(ManifestUtil, ClientIDsUtil).RetrieveJsonTrackFromV2Url(id);
            foreach(var transcoding in track.media.transcodings)
            {
                if (transcoding.format.protocol == "progressive")
                {
                    return new JsonUtils(ManifestUtil, ClientIDsUtil).GetDownloadUrlFromProgressiveUrl(transcoding.url);
                }
            }
            return null;
        }

        public string GetEffectiveDownloadUrlForHQ(string downloadUrl, out string extension)
        {
            downloadUrl = RemoveCarriageReturnAndLineFeed(downloadUrl + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}");
            extension = DetermineExtensionForHQ(downloadUrl);
            return downloadUrl;
        }

        public string RemoveCarriageReturnAndLineFeed(string value)
        {
            return value.Replace("\r", "").Replace("\n", "");
        }

        public void DownloadTrackAndTag(ref Track song)
        {
            if (song?.LocalPath == null)
                throw new Exception("Local path empty");

            Directory.CreateDirectory(Path.GetDirectoryName(song.LocalPath));

            if (!ChooseHighqualitysong || (ChooseHighqualitysong && !song.downloadable && !song.IsHD) )
            {
                song.EffectiveDownloadUrl = GetEffectiveDownloadUrlForStream(song.id);
                PersistStreamToDisk(ref song, httpClient.GetStreamAsync(song.EffectiveDownloadUrl).Result, false);
                MetadataTaggingUtils.TagIt(song);
                Interlocked.Increment(ref ManifestUtil.ProgressUtil.SongsDownloaded);
                return;
            }
  
            song.EffectiveDownloadUrl = GetEffectiveDownloadUrlForHQ(song.download_url, out string extensionForHQ);

            var highQualitySoundMemoryStream = new MemoryStream();
            using (var highQualitySoundStream = httpClient.GetStreamAsync(song.EffectiveDownloadUrl).Result)
            {
                highQualitySoundStream.CopyToAsync(highQualitySoundMemoryStream).GetAwaiter().GetResult();
                highQualitySoundMemoryStream.Position = 0;
            }

            if (!ConvertToMp3 || (ConvertToMp3 && !DetermineAllowedFormatsForConversion().Contains(extensionForHQ)))
            {
                //not able to convert or not chosen
                PersistStreamToDisk(ref song, highQualitySoundMemoryStream, true, extensionForHQ);
                MetadataTaggingUtils.TagIt(song);
                Interlocked.Increment(ref ManifestUtil.ProgressUtil.SongsDownloaded);
                return;
            }
         
            try
            {
                AudioConverterUtils.ConvertHighQualityAudioFormats(highQualitySoundMemoryStream, ref song, extensionForHQ);
                highQualitySoundMemoryStream.DisposeAsync();
            }
            catch (Exception e)
            {
                ManifestUtil.FileSystemUtil.LogTrackException(song, e, false);
                PersistStreamToDisk(ref song, highQualitySoundMemoryStream, true, extensionForHQ);
            }

            //high quality track converted or not converted because of exception
            MetadataTaggingUtils.TagIt(song);
            Interlocked.Increment(ref ManifestUtil.ProgressUtil.SongsDownloaded);
            return;
        }


        private void PersistStreamToDisk(ref Track song, Stream soundSteam, bool isHQ, string extensionForHQ = null)
        {
            if (isHQ)
            {
                soundSteam.Position = 0;
                song.LocalPath += extensionForHQ;
            }
            else
                song.LocalPath += ".mp3";

            using (soundSteam)
            using (var fs = new FileStream(song.LocalPath, FileMode.Create))
            {
                soundSteam.CopyToAsync(fs).GetAwaiter().GetResult();
            }
        }

        private List<string> DetermineAllowedFormatsForConversion()
        {
            var formats = new List<string>
            {
                ".wav", ".aiff", ".aif", ".m4a", ".aac", ".mp3"
            };
            if (ExcludeAac)
                formats.Remove(".aac");
            if (ExcludeM4A)
                formats.Remove(".m4a");
            return formats;
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

        public static string DetermineExtensionForHQ(string downloadUrl)
        {
            try
            {
                var result = httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead).Result;
                try
                {
                    return Path.GetExtension(result.Content.Headers.ContentDisposition.FileName.Replace("\"", String.Empty));
                }
                catch (Exception) { }
                try
                {
                    //If it fails to get extention from disposition (if ContentDisposition works it gives more reliable results)
                    return $".{result.Content.Headers.GetValues("x-amz-meta-file-type").First().Replace("\"", String.Empty)}";
                }
                catch (Exception) { }
            }
            catch (Exception)
            {
                // ignored
            }
            return ".mp3";
        }

        public string ParseUserIdFromProfileUrl(string url)
        {        
            return new JsonUtils(ManifestUtil, ClientIDsUtil).RetrieveUserIdFromUserName(GetUserNameFromProfileUrl(url));
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
            var page = DownloadUtils.httpClient.SendAsync(DownloadUtils.RequestMessageWithHeaders(url, HttpMethod.Get)).Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            doc.LoadHtml(page);
            return doc;
        }
    }
}
