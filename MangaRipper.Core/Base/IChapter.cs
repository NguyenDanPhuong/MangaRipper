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

        Uri Address
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

        Task DownloadImageAsync(string fileName, CancellationToken cancellationToken, Progress<ChapterProgress> progress);
    }
}
