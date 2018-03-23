using MangaRipper.Core.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Infrastructure
{
    public class DownloadLogging : IDownloader
    {
        private readonly IDownloader downloader;
        private readonly ILogger logger;

        public DownloadLogging(IDownloader downloader, ILogger logger)
        {
            this.downloader = downloader;
            this.logger = logger;
        }

        public Task<string> DownloadStringAsync(string url, CancellationToken token)
        {
            logger.Info($"> DownloadStringAsync: {url}");
            return downloader.DownloadStringAsync(url, token);
        }

        public Task<string> DownloadToFolder(string url, string folder, int count, CancellationToken cancellationToken)
        {
            logger.Info($"> DownloadToFolder: {url}. Folder: {folder}");
            return downloader.DownloadToFolder(url, folder, count, cancellationToken);
        }
    }
}
