using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MangaRipper.Core.Models;

namespace MangaRipper.Core.Controllers
{
    public interface IWorkerController
    {
        void Cancel();
        Task<IEnumerable<Chapter>> FindChapterListAsync(string mangaPath, IProgress<int> progress);
        Task<DownloadTaskResult> RunDownloadTaskAsync(DownloadChapterTask task, IProgress<int> progress);
    }
}