using System.Collections.Generic;
using MangaRipper.Models;

namespace MangaRipper.Presenters
{
    public interface IMainView
    {
        void SetChapterRows(IEnumerable<ChapterRow> chapters);
        void SetDownloadRows(IEnumerable<DownloadRow> chapters);
        void SetChaptersProgress(string progress);
        string GetSavePath(ChapterRow chapter);
    }
}