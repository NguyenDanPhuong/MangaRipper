using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using MangaRipper.Core.Models;

namespace MangaRipper.Presenters
{
    public interface IMainView
    {
        Func<string, Task> FindChaptersClicked { get; set; }
        void SetChapters(IEnumerable<Chapter> chapters);
        void SetChaptersProgress(string progress);
        void SetStatusText(string statusMessage);
        void ShowMessageBox(string caption, string text, MessageBoxButtons buttons, MessageBoxIcon icon);
    }
}