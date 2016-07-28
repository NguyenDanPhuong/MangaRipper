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

        Uri Address
        {
            get;
        }

        IWebProxy Proxy
        {
            get;
            set;
        }

        Task PopulateChapterAsync(Progress<int> progress);
    }
}
