using CloudFlareUtilities;
using MangaRipper.Core.FilenameDetectors;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Plugins
{
    /// <summary>
    /// Support download web page to string and image file to folder.
    /// </summary>
    public class HttpDownloader : IHttpDownloader
    {
        private readonly IFilenameDetector filenameDetector;

        public HttpDownloader(IFilenameDetector filenameDetector)
        {
            this.filenameDetector = filenameDetector;
        }

        public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken)
        {
            var request = CreateRequest();
            using (var response = await request.GetAsync(url, cancellationToken))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetFileAsync(string url, string folder, CancellationToken cancellationToken)
        {
            var request = CreateRequest();
            using (var response = await request.GetAsync(url, cancellationToken))
            {
                var filename = filenameDetector.GetFilename(response);
                var fileNameWithPath = Path.Combine(folder, filename);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                using (var streamReader = new FileStream(fileNameWithPath, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(streamReader);
                    return fileNameWithPath;
                }
            }
        }

        private HttpClient CreateRequest()
        {
            var cookieContainer = new CookieContainer();

            cookieContainer.Add(new Cookie("isAdult", "1", "/", "www.mangahere.cc"));
            cookieContainer.Add(new Cookie("isAdult", "1", "/", "fanfox.net"));

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
