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

        /// <summary>
        /// Download the file to specific (temp) folder
        /// </summary>
        /// <param name="url">Image's Url</param>
        /// <param name="folder">Temporary folder to keep the downloaded images</param>
        /// <param name="count">Image order. Used for naming</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Path to the image</returns>
        public async Task<string> DownloadToFolder(string url, string folder, int count,  CancellationToken cancellationToken)
        {
            var request = CreateRequest();
            using (var response = await request.GetAsync(url, cancellationToken))
            {
                var filename = filenameDetector.GetFilename(url, response.Content.Headers);
                // change the filename only if count is positive
                if (count >= 0)
                {
                    filename = count.ToString("D3") + filename.Substring(filename.LastIndexOf('.'));
                }
                var fileNameWithPath = Path.Combine(folder, filename);
                using (var streamReader = new FileStream(fileNameWithPath, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(streamReader);
                    return fileNameWithPath;
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

            var cloudFlareHandler = new ClearanceHandler(firstHandle);
            var request = new HttpClient(cloudFlareHandler);
            return request;
        }
    }
}
