using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Models;

namespace MangaRipper.Core
{
    public interface IWorkerController
    {
        Task<IReadOnlyList<Chapter>> GetChapterListAsync(string mangaPath, IProgress<string> progress, CancellationToken cancellationToken);
        Task<DownloadChapterResponse> GetChapterAsync(DownloadChapterRequest task, IProgress<string> progress, CancellationToken cancellationToken);
    }
}