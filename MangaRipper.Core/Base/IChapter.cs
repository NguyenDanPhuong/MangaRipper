using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public interface IChapter
    {
        string Name
        {
            get;
            set;
        }

        string Address
        {
            get;
        }

        string SaveTo
        {
            get;
        }

        bool IsBusy
        {
            get;
        }

        IWebProxy Proxy
        {
            get;
            set;
        }

        Task DownloadImageAsync(string fileName, CancellationToken cancellationToken, IProgress<ChapterProgress> progress);
    }
}
