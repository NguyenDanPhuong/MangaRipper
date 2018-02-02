using CloudFlareUtilities;
using MangaRipper.Core.FilenameDetectors;
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
        private readonly FilenameDetector filenameDetector;

        public Downloader(FilenameDetector filenameDetector)
        {
            this.filenameDetector = filenameDetector;
        }

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
                var filename = filenameDetector.GetFilename(url, response.Content.Headers);
                var fileNameWithPath = Path.Combine(folder, filename);
                using (var streamReader = new FileStream(fileNameWithPath, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(streamReader);
                    return fileNameWithPath;
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

            var cloudFlareHandler = new ClearanceHandler(firstHandle);
            var request = new HttpClient(cloudFlareHandler);
            return request;
        }
    }
}
