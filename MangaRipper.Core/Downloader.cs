using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class Downloader
    {
        public static IWebProxy Proxy { get; set; }

        public static int MaxJobs { get; set; }

        public static SemaphoreSlim semaphore = new SemaphoreSlim(5);

        private static HttpWebRequest CreateRequest(string url)
        {
            var uri = new Uri(url);
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Proxy = Proxy;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Referer = uri.AbsoluteUri;
            return request;
        }

        public static async Task<string> DownloadStringAsync(string url)
        {
            var task = Task.Run(async () =>
            {
                var request = CreateRequest(url);
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                        return await streamReader.ReadToEndAsync();
                    }
                }
            });

            return await task;
        }


        public static async Task DownloadFileAsync(string url, string fileName, CancellationToken cancellationToken)
        {
            var task = Task.Run(async () =>
            {
                var request = CreateRequest(url);
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    using (var streamReader = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        await responseStream.CopyToAsync(streamReader, 81920, cancellationToken);
                    }
                }
            });

            await task;
        }
    }
}
