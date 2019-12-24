using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Soundcloud_Playlist_Downloader.JsonObjects;
using Soundcloud_Playlist_Downloader.Properties;
using System.Linq;
using Soundcloud_Playlist_Downloader.Language;

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

        public void DownloadSongs(IList<Track> tracksToDownload)
        {
            Interlocked.Add(ref ManifestUtil.ProgressUtil.SongsToDownload, tracksToDownload.Count);

            if (ManifestUtil.ProgressUtil.SongsToDownload == 0) return;
            var cts = new CancellationTokenSource();
            if (ConcurrentDownloads == 0)
                throw new Exception(LanguageManager.Language["STR_DOWNLOAD_SONG_EX"]);
            var po = new ParallelOptions
            {
                CancellationToken = cts.Token,
                MaxDegreeOfParallelism = ConcurrentDownloads
            };
            try
            {
                Parallel.ForEach(tracksToDownload, po,
                    track =>
                    {
                        try
                        {
                            ManifestUtil.ProgressUtil.AddOrUpdateInProgressTrack(track);

                            if (!DownloadTrackAndTag(ref track)) return;

                            track.IsDownloaded = true;

                            ManifestUtil.ProgressUtil.AddOrUpdateSuccessFullTrack(track);
                            ProcessUpdateManifestDelegate pumd = ManifestUtil.UpdateManifest;
                            pumd(track);
                        }
                        catch (Exception e)
                        {                          
                            ManifestUtil.ProgressUtil.AddOrUpdateFailedTrack(track, e);
                            ManifestUtil.FileSystemUtil.LogTrackWithError(track, e);

                            //EventLog.WriteEntry(Application.ProductName, exc.ToString());
                            if (ProgressUtils.MaximumExceptionsCount <= ManifestUtil.ProgressUtil.CurrentAmountOfExceptions)
                            {
                                ManifestUtil.ProgressUtil.IsError = true;
                                cts.Cancel();
                                po.CancellationToken.ThrowIfCancellationRequested();
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
                cts.Dispose();
            }
        }

        public string GetEffectiveDownloadUrl(string downloadUrl, int id, bool downloadable)
        {
            if (Highqualitysong && !string.IsNullOrWhiteSpace(downloadUrl) && downloadable) //user has selected to download high quality songs if available
            {
                return RemoveCarriageReturnAndLineFeed(downloadUrl + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}");
            }

            var track = new JsonUtils(ManifestUtil, ClientIDsUtil.ClientIdCurrentValue).RetrieveJsonTrackFromV2Url(id);
            foreach(var transcoding in track.media.transcodings)
            {
                if (transcoding.format.protocol == "progressive")
                {
                    return new JsonUtils(ManifestUtil, ClientIDsUtil.ClientIdCurrentValue).GetDownloadUrlFromProgressiveUrl(transcoding.url);
                }
            }
            return null;
        }

        public string RemoveCarriageReturnAndLineFeed(string value)
        {
            return value.Replace("\r", "").Replace("\n", "");
        }

        public bool DownloadTrackAndTag(ref Track song)
        {
            if (!ManifestUtil.ProgressUtil.IsActive) return false;
            if (song?.LocalPath == null)
                return false;
            Directory.CreateDirectory(Path.GetDirectoryName(song.LocalPath));

            song.EffectiveDownloadUrl = GetEffectiveDownloadUrl(song.download_url, song.id, song.downloadable);
            if (string.IsNullOrWhiteSpace(song.EffectiveDownloadUrl))
                return false;

            using (var client = new WebClient())
            {               
                string extension = DetermineExtension(song);
                bool succesfulConvert = false;
                if (song.IsHD && ConvertToMp3 && Highqualitysong &&
                    DetermineAllowedFormats().Contains(extension))
                {
                    //get the wav song as byte data, as we won't store it just yet
                    var soundbytes = client.DownloadData(song.EffectiveDownloadUrl);
                    //convert to mp3 & then write bytes to file
                    succesfulConvert = AudioConverterUtils.ConvertAllTheThings(soundbytes, ref song, extension);
                }
                
                if(!succesfulConvert) 
                {
                    song.LocalPath += ".mp3";
                    client.DownloadFile(song.EffectiveDownloadUrl, song.LocalPath);
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
            if (!song.downloadable)
                return ".mp3";
            try
            {
                WebRequest requestEffectiveDownloadUrl = WebRequest.Create(song.EffectiveDownloadUrl);
                return GetExtensionFromWebRequest(requestEffectiveDownloadUrl);
            }
            catch (Exception)
            {
                // ignored
            }
            return ".mp3";
        }

        public bool IsDownloadable(string downloadUrl)
        {
            var requestdownloadUrl = WebRequest.Create(downloadUrl + $"?client_id={ClientIDsUtil.ClientIdCurrentValue}");
            return IsValidUrl(requestdownloadUrl);
        }

        public static bool IsValidUrl(WebRequest request)
        {
            bool succeeded = true;
            request.Method = "HEAD";
            try
            {
                using (var response = request.GetResponse())
                {
                    succeeded = true;
                }
            }
            catch (WebException)
            {
                succeeded = false;
            }
            return succeeded;
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
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Accept-Language", " en-US");
                client.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36");
                doc.LoadHtml(client.DownloadString(url));
            }
            return doc;
        }
    }
}
