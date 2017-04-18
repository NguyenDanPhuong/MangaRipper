using System;
using System.Collections.Generic;
using MangaRipper.Core.Models;

namespace MangaRipper.Presenters
{
    public interface IMainView
    {
        Action<string> FindChaptersClicked { get; set; }
        void SetChapters(IEnumerable<Chapter> chapters);
        void SetChaptersProgress(string progress);
    }
}