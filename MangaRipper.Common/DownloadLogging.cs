using MangaRipper.Core.Logging;
using MangaRipper.Core.Plugins;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Infrastructure
{
    public class DownloadLogging : IHttpDownloader
    {
        private readonly IHttpDownloader downloader;
        private readonly ILogger<DownloadLogging> logger;

        public DownloadLogging(IHttpDownloader downloader, ILogger<DownloadLogging> logger)
        {
            this.downloader = downloader;
            this.logger = logger;
        }

        public Task<string> GetStringAsync(string url, CancellationToken token)
        {
            logger.Info($"> DownloadStringAsync: {url}");
            return downloader.GetStringAsync(url, token);
        }

        public Task<string> GetFileAsync(string url, string folder, CancellationToken cancellationToken)
        {
            logger.Info($"> DownloadToFolder: {url}. Folder: {folder}");
            return downloader.GetFileAsync(url, folder, cancellationToken);
        }
    }
}
