using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Plugins
{
    public interface IHttpDownloader
    {
        Task<string> DownloadStringAsync(string url, CancellationToken token);
        Task<string> DownloadToFolder(string url, string folder, CancellationToken cancellationToken);
    }
}