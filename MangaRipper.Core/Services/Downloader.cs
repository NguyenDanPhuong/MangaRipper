using CloudFlareUtilities;
using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Providers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                GetAllCloudFlareCookies(response);
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
                GetAllCloudFlareCookies(response);
                return await response.Content.ReadAsStringAsync();
            }
        }

        private HttpClient CreateRequest()
        {
            var cookieContainer = new CookieContainer();
            // Set CloudFlare cookies if we have one
            if (CacheProvider.Instance.CacheExists("__cfduid") && CacheProvider.Instance.CacheExists("cf_clearance"))
            {
                cookieContainer.Add(CacheProvider.Instance.GetCacheValue("__cfduid"));
                cookieContainer.Add(CacheProvider.Instance.GetCacheValue("cf_clearance"));
            }

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

        /// <summary>
        /// Get cookies which allow us to pass the CloudFlare calculation on next request
        /// </summary>
        /// <param name="response">Take the domain and cookies from it</param>
        private void GetAllCloudFlareCookies(HttpResponseMessage response)
        {
            List<string> cookies = response.Headers
                .Where(pair => pair.Key == "Set-Cookie")
                .SelectMany(pair => pair.Value)
                .ToList();

            foreach (var cookie in cookies)
            {
                if (cookie.Contains("__cfduid") || cookie.Contains("cf_clearance"))
                {
                    var splitNum = cookie.IndexOf('=');
                    var key = cookie.Substring(0, splitNum);
                    var domain = response.RequestMessage.RequestUri;
                    var value = new Cookie(key, cookie.Substring(splitNum + 1))
                    {
                        Domain = domain.Host
                    };

                    CacheProvider.Instance.SetCacheValue(key, value);
                }
            }
        }
    }
}
