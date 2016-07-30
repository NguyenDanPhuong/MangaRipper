using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public interface IManga
    {
        SiteInformation GetInformation();
        bool Of(string link);
        Task<IList<Chapter>> FindChapters(string manga);
        Task<IList<string>> FindImanges(Chapter chapter);
    }
}
