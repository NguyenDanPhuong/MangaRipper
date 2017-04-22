using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using MangaRipper.Core.Models;
using MangaRipper.Core.DataTypes;
using System.ComponentModel;

namespace MangaRipper.Presenters
{
    public interface IMainView
    {
        Action<IEnumerable<Chapter>, string, OutputFormat[]> AddDownloadClicked { get; set; }
        Func<Task> StartDownloadClicked { get; set; }
        Action<IEnumerable<DownloadChapterTask>> RemoveDownloadClicked { get; set; }
        Action MainFormClosing { get; set; }
        Action MainFormLoaded { get; set; }
        void RefreshTaskGrid();

        Func<string, Task> FindChaptersClicked { get; set; }
        void SetChapters(IEnumerable<Chapter> chapters);
        void SetChaptersProgress(string progress);

        void SetStatusText(string statusMessage);
        void ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
        void SetDownloadQueue(BindingList<DownloadChapterTask> downloadQueue);
    }
}