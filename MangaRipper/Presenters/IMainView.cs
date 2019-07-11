using System.Collections.Generic;
using MangaRipper.Models;

namespace MangaRipper.Presenters
{
    public interface IMainView
    {
        void SetChapters(IEnumerable<ChapterRow> chapters);
        void SetChaptersProgress(string progress);
    }
}