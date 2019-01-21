using Soundcloud_Playlist_Downloader.Language;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var webException = (WebException)e;
                using (var response = webException.Response as HttpWebResponse)
                {
                    var responseUri = response.ResponseUri.AbsoluteUri;
                    text = string.Format(LanguageManager.Language["STR_EXCEPTION_WEB1"], responseUri, response.StatusCode, response.StatusDescription);

                }

                throw new Exception(string.Format(LanguageManager.Language["STR_EXCEPTION_WEB2"].Replace("\\n", "\n"), text));
            }
            throw new Exception(string.Format(LanguageManager.Language["STR_EXCEPTION_WEB3"].Replace("\\n", "\n"), e));
        }

        public static string GetInnerExceptionMessages(Exception e)
        {
            if (e == null) return string.Format("({0})", LanguageManager.Language["STR_EXCEPTION_GET1"]);
            string message = "";
            IEnumerable<Exception> aggregateInnerExceptions = Enumerable.Empty<Exception>();
            if (e is AggregateException && (e as AggregateException).InnerExceptions.Any())
            {
                aggregateInnerExceptions = (e as AggregateException).InnerExceptions;
            }
            else if (e.InnerException != null)
            {
                message += string.Format("\r\n{0}: {1}", LanguageManager.Language["STR_EXCEPTION_GET2"], e.InnerException.Message);
            }
            foreach (var aggInnerEx in aggregateInnerExceptions)
            {
                if (aggInnerEx == null) continue;
                message += string.Format("\r\n{0}: {1}", LanguageManager.Language["STR_EXCEPTION_GET3"], aggInnerEx.Message);
            }
            if (message == "")
                return string.Format("({0})", LanguageManager.Language["STR_EXCEPTION_GET4"]);
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