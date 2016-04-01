using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Soundcloud_Playlist_Downloader
{
    class ExceptionHandler
    {
        public static void handleException(Exception e)
        {
            Debug.WriteLine(e);
            if (e is WebException)
            {
                string text = "";
                string scrubbedtext = "";
                WebException w = (WebException)e;
                using (WebResponse response = w.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Debug.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        text = reader.ReadToEnd();
                    }
                }
                scrubbedtext = ScrubHtml(text);
                Debug.WriteLine(scrubbedtext);

                throw new Exception("Soundcloud API seems to be down, please check: http://status.soundcloud.com/ or https://developers.soundcloud.com/docs#errors for more information."
                + Environment.NewLine + Environment.NewLine + "The following error was thrown: "
                + Environment.NewLine + scrubbedtext);
            }
            else
            {
                throw new Exception("The following error was thrown: "
                + Environment.NewLine + e);
            }
        }

        public static string ScrubHtml(string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }     
    }
}
