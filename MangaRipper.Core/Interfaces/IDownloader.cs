using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Interfaces
{
    public interface IDownloader
    {
        Task<string> DownloadStringAsync(string url, CancellationToken token);
        Task<string> DownloadToFolder(string url, string folder, int count, CancellationToken cancellationToken);
    }
}