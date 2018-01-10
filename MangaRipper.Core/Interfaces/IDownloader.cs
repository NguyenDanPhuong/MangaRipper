using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Interfaces
{
    public interface IDownloader
    {
        CookieCollection Cookies { get; set; }
        string Referrer { get; set; }

        Task<string> DownloadStringAsync(string url, CancellationToken token);
        Task<string> DownloadToFolder(string url, string folder, CancellationToken cancellationToken);
    }
}