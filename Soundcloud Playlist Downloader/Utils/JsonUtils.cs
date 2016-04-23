using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class JsonUtils
    {
        public static string RetrieveJson(string url, string clientId = null, int? limit = null, int? offset = null)
        {
            string json = null;
            if (limit == 0)
                limit = null;

            if (string.IsNullOrEmpty(url))
                return null;
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    if (!url.Contains("client_id="))
                    {
                        url += (url.Contains("?") ? "&" : "?") + "client_id=" + clientId;
                    }
                    if (limit != null)
                    {
                        url += "&limit=" + limit;
                    }
                    if (offset != null)
                    {
                        url += "&offset=" + offset;
                    }

                    if (limit != null)
                        url += "&linked_partitioning=1"; //will add next_href to the response

                    json = client.DownloadString(url);
                }
            }
            catch (Exception e)
            {
                PlaylistSync.IsError = true;
                ExceptionHandlerUtils.HandleException(e);
            }

            return json;
        }
    }
}
