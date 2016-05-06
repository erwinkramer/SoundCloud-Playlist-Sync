using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Soundcloud_Playlist_Downloader.Utils
{
    internal class ExceptionHandlerUtils
    {
        public static void HandleException(Exception e)
        {
            var exception = e as WebException;
            if (exception != null)
            {
                var text = "";
                var scrubbedtext = "";
                var w = exception;
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

        public static string GetExceptionMessages(Exception e, string msgs = "")
        {
            if (e == null) return string.Empty;
            if (msgs == "") msgs = e.Message;

            IEnumerable<Exception> innerExceptions = Enumerable.Empty<Exception>();
            if (e is AggregateException && (e as AggregateException).InnerExceptions.Any())
            {
                innerExceptions = (e as AggregateException).InnerExceptions;
            }
            else
            {
                if (e.InnerException != null)
                    msgs += "\r\nInnerException: " + e.InnerException.Message;
            }
            foreach (var innerEx in innerExceptions)
            {
                if (innerEx == null) continue;
                msgs += "\r\nInnerException: " + innerEx.Message;
            }
            return msgs;
        }

        public static string ScrubHtml(string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }
    }
}