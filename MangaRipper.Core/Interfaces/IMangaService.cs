using MangaRipper.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Interfaces
{
    /// <summary>
    /// We have many manga services (web site), each service support finding chapters URL and images URL.
    /// </summary>
    public interface IMangaService
    {
        /// <summary>
        /// The information of service
        /// </summary>
        /// <returns></returns>
        SiteInformation GetInformation();

        /// <summary>
        /// Is a link is from this service.
        /// If it is. We can use this service to fetch chapters and download
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        bool Of(string link);

        /// <summary>
        /// Find all chapters inside a manga
        /// </summary>
        /// <param name="manga"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken);

        /// <summary>
        /// Find all images inside a chapter.
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken);
    }
}
