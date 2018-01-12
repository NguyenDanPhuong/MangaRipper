using CloudFlareUtilities;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Interfaces
{
    /// <summary>
    /// Support download web page to string and image file to folder.
    /// </summary>
    public class Downloader : IDownloader
    {
        public CookieCollection Cookies { get; set; }
        public string Referrer { get; set; }

        /// <summary>
        /// Download single web page to string.
        /// </summary>
        /// <param name="url">The URL to download</param>
        /// <returns></returns>
        public async Task<string> DownloadStringAsync(string url, CancellationToken token)
        {
            return await DownloadStringAsyncInternal(url, token);
        }

        public async Task<string> DownloadToFolder(string url, string folder, CancellationToken cancellationToken)
        {
            var request = CreateRequest();
            using (var response = await request.GetAsync(url, cancellationToken))
            {
                var fileNameFromServer = response.Content.Headers.ContentDisposition?.FileName?.Trim().Trim(new char[] { '"' });
                var fileName = string.IsNullOrEmpty(fileNameFromServer) ?
                    Path.Combine(folder, GetFilenameFromUrl(url)) : Path.Combine(folder, fileNameFromServer);
                using (var streamReader = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(streamReader);
                    return fileName;
                }
            }
        }

        private string GetFilenameFromUrl(string url)
        {
            var uri = new Uri(url);
            return Path.GetFileName(uri.LocalPath);
        }

        private async Task<string> DownloadStringAsyncInternal(string url, CancellationToken cancellationToken)
        {
            var request = CreateRequest();
            using (var response = await request.GetAsync(url, cancellationToken))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        private HttpClient CreateRequest()
        {
            var firstHandle = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                Credentials = CredentialCache.DefaultCredentials,
                AllowAutoRedirect = false,
                CookieContainer = new CookieContainer()
            };
            if (Cookies != null)
                firstHandle.CookieContainer.Add(Cookies);

            var cloudFlareHandler = new ClearanceHandler(firstHandle);
            var request = new HttpClient(cloudFlareHandler);
            if (Referrer != null)
            {
                request.DefaultRequestHeaders.Referrer = new Uri(Referrer);
            }
            return request;
        }
    }
}
