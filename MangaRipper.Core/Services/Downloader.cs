using CloudFlareUtilities;
using MangaRipper.Core.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Services
{
    /// <summary>
    /// Support download web page to string and image file to folder.
    /// </summary>
    public class Downloader
    {
        private readonly ILogger logger;

        public CookieCollection Cookies { get; set; }
        public string Referrer { get; set; }


        public Downloader(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Download single web page to string.
        /// </summary>
        /// <param name="url">The URL to download</param>
        /// <returns></returns>
        public async Task<string> DownloadStringAsync(string url, CancellationToken token)
        {
            logger.Info($"> DownloadStringAsync: {url}");
            return await DownloadStringAsyncInternal(url, token);
        }

        public async Task DownloadToFolder(string url, string folder, CancellationToken cancellationToken)
        {
            var request = CreateRequest();
            using (var response = await request.GetAsync(url, cancellationToken))
            {
                var fileNameFromServer = response.Content.Headers.ContentDisposition.FileName.Trim().Trim(new char[] { '"' });
                var file = Path.Combine(folder, fileNameFromServer);
                await DownloadToFile(url, file, cancellationToken);
            }
        }

        /// <summary>
        /// Download file and save to folder
        /// </summary>
        /// <param name="url">The URL to download</param>
        /// <param name="fileName">Save to filename</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task DownloadToFile(string url, string fileName, CancellationToken token)
        {
            logger.Info($"> DownloadFileAsync begin: {url} - {fileName}");
            var result = await DownloadFileAsyncInternal(url, fileName, token);
            logger.Info($"> DownloadFileAsync result: {url} - {result}");
        }

        private async Task<string> DownloadFileAsyncInternal(string url, string fileName, CancellationToken token)
        {
            var request = CreateRequest();
            using (var response = await request.GetAsync(url, token))
            {
                using (var streamReader = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(streamReader);
                    return response.StatusCode.ToString();
                }
            }
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
