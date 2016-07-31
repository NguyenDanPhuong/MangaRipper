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
        private HttpWebRequest CreateRequest(string url)
        {
            var uri = new Uri(url);
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Referer = uri.AbsoluteUri;
            return request;
        }


        public async Task<string> DownloadStringAsync(string url)
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
        }

        internal async Task<string> DownloadStringAsync(IEnumerable<string> urls, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();
            foreach (var url in urls)
            {
                string input = await DownloadStringAsync(url);
                sb.Append(input);
                cancellationToken.ThrowIfCancellationRequested();
            }

            return sb.ToString();
        }

        public async Task DownloadFileAsync(string url, string fileName, CancellationToken cancellationToken)
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
        }
    }
}
