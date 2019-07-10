using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Plugins
{
    public interface IHttpDownloader
    {
        Task<string> GetStringAsync(string url, CancellationToken token);
        Task<string> GetFileAsync(string url, string folder, CancellationToken cancellationToken);
    }
}