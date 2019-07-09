using MangaRipper.Core.Logging;
using MangaRipper.Core.Plugins;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Infrastructure
{
    public class DownloadLogging : IHttpDownloader
    {
        private readonly IHttpDownloader downloader;
        private readonly ILogger logger;

        public DownloadLogging(IHttpDownloader downloader, ILogger logger)
        {
            this.downloader = downloader;
            this.logger = logger;
        }

        public Task<string> DownloadStringAsync(string url, CancellationToken token)
        {
            logger.Info($"> DownloadStringAsync: {url}");
            return downloader.DownloadStringAsync(url, token);
        }

        public Task<string> DownloadFileAsync(string url, string folder, CancellationToken cancellationToken)
        {
            logger.Info($"> DownloadToFolder: {url}. Folder: {folder}");
            return downloader.DownloadFileAsync(url, folder, cancellationToken);
        }
    }
}
