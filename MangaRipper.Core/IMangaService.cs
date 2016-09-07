using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    /// <summary>
    /// We have many manga services, each service support downloading from one site.
    /// </summary>
    public interface IMangaService
    {
        SiteInformation GetInformation();
        bool Of(string link);
        Task<IList<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken);
        Task<IList<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken);
    }
}
