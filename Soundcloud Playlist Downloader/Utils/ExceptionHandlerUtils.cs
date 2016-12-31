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

        public static string GetInnerExceptionMessages(Exception e)
        {
            if (e == null) return "(exception is null)";
            string message = "";
            IEnumerable<Exception> aggregateInnerExceptions = Enumerable.Empty<Exception>();
            if (e is AggregateException && (e as AggregateException).InnerExceptions.Any())
            {
                aggregateInnerExceptions = (e as AggregateException).InnerExceptions;
            }
            else if (e.InnerException != null)
            {
                message += "\r\nInnerException: " + e.InnerException.Message;
            }
            foreach (var aggInnerEx in aggregateInnerExceptions)
            {
                if (aggInnerEx == null) continue;
                message += "\r\nAggregateInnerException: " + aggInnerEx.Message;
            }
            if (message == "")
                return "(no inner exceptions found)";
            return message;
        }

        public static string ScrubHtml(string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }
    }
}