using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Soundcloud_Playlist_Downloader.Utils
{
    internal class ExceptionHandlerUtils
    {
        public static void HandleException(Exception e)
        {
            if (e is WebException)
            {
                var text = "";
                var scrubbedtext = "";
                var w = (WebException) e;
                using (var response = w.Response)
                {
                    using (var data = response.GetResponseStream())
                        if (data != null)
                            using (var reader = new StreamReader(data))
                            {
                                text = reader.ReadToEnd();
                            }
                }
                scrubbedtext = ScrubHtml(text);

                throw new Exception("Soundcloud API seems to be down, please check: http://status.soundcloud.com/ or https://developers.soundcloud.com/docs#errors for more information."
                                    + Environment.NewLine + Environment.NewLine + "The following error was thrown: "
                                    + Environment.NewLine + scrubbedtext);
            }
            throw new Exception("The following error was thrown: "
                                + Environment.NewLine + e);
        }

        public static string ScrubHtml(string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }
    }
}