using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public interface ITitle
    {
        List<IChapter> Chapters
        {
            get;
        }

        string Address
        {
            get;
        }

        Task PopulateChapterAsync(IProgress<int> progress);
    }
}
