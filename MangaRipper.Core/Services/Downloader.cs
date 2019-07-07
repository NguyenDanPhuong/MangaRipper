using CloudFlareUtilities;
using MangaRipper.Core.FilenameDetectors;
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
        private readonly IFilenameDetector filenameDetector;

        public Downloader(IFilenameDetector filenameDetector)
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
            var cookieContainer = new CookieContainer();

            cookieContainer.Add(new Cookie("isAdult", "1", "/", "www.mangahere.cc"));

            var firstHandle = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                Credentials = CredentialCache.DefaultCredentials,
                AllowAutoRedirect = false,
                CookieContainer = cookieContainer
            };

            var cloudFlareHandler = new ClearanceHandler(firstHandle);
            var request = new HttpClient(cloudFlareHandler);
            return request;
        }
    }
}
