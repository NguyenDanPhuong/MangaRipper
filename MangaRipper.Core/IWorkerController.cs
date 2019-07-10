using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Models;

namespace MangaRipper.Core
{
    public interface IWorkerController
    {
        Task<IEnumerable<Chapter>> GetChapterListAsync(string mangaPath, IProgress<int> progress, CancellationToken cancellationToken);
        Task<DownloadTaskResult> GetChapterAsync(DownloadChapterTask task, IProgress<int> progress, CancellationToken cancellationToken);
    }
}