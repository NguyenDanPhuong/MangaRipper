using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public interface ITitle
    {
        string Address
        {
            get;
        }

        Task<IList<IChapter>> PopulateChapterAsync(IProgress<int> progress, CancellationToken cancellationToken);
    }
}
