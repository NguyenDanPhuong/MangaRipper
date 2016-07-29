using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class Downloader
    {
        public static IWebProxy Proxy { get; set; }

        public static int MaxJobs { get; set; }

        public static async Task<string> DownloadStringAsync(string url)
        {
            var uri = new Uri(url);
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Proxy = Proxy;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Referer = uri.AbsoluteUri;
            using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                    return await streamReader.ReadToEndAsync();
                }
            }
        }
    }
}
