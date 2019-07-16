using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MangaRipper.Core;
using MangaRipper.Core.Models;
using MangaRipper.Helpers;
using MangaRipper.Models;
using NLog;

namespace MangaRipper.Presenters
{
    public class MainViewPresenter
    {
        private IMainView View { get; set; }

        private IWorkerController worker;
        private readonly ApplicationConfiguration applicationConfiguration;
        private IList<ChapterRow> chapterRows = new List<ChapterRow>();
        private IList<DownloadRow> downloadRows = new List<DownloadRow>();

        private CancellationTokenSource cancellationTokenSource;

        internal MainViewPresenter(IMainView view, IWorkerController wc, ApplicationConfiguration applicationConfiguration)
        {
            View = view;
            worker = wc;
            this.applicationConfiguration = applicationConfiguration;
        }

        public async Task GetChapterListAsync(string mangaUrl, bool hasPrefix)
        {
            var progress = new Progress<string>(p => View.SetChaptersProgress(p));
            chapterRows = (await worker.GetChapterListAsync(mangaUrl, progress, new CancellationTokenSource().Token))
                .Select(c => new ChapterRow(c))
                .ToList();
            GeneratePrefix(hasPrefix);
            View.SetChapterRows(chapterRows);
        }

        public void ChangePrefix(bool hasPrefix)
        {
            GeneratePrefix(hasPrefix);
            View.SetChapterRows(chapterRows);
        }

        private void GeneratePrefix(bool hasPrefix)
        {
            int count = 1;
            for (int i = chapterRows.Count() - 1; i >= 0; i--)
            {
                chapterRows[i].Prefix = hasPrefix ? count++ : 0;
            }
        }

        internal void RemoveAllChapterRows()
        {
            downloadRows.Clear();
            View.SetDownloadRows(downloadRows);
        }

        internal void Remove(DownloadRow chapter)
        {
            downloadRows.Remove(chapter);
            View.SetDownloadRows(downloadRows);
        }

        internal void CreateDownloadRows(List<ChapterRow> items, OutputFormat[] formats)
        {
            foreach (var chapter in items.Where(item => downloadRows.All(r => r.Url != item.Url)))
            {
                var savePath = View.GetSavePath(chapter);
                var task = new DownloadRow
                {
                    Name = chapter.DisplayName,
                    Url = chapter.Url,
                    SaveToFolder = savePath,
                    Formats = formats
                };
                downloadRows.Add(task);
            }

            View.SetDownloadRows(downloadRows);
        }

        internal async Task StartDownloadChaptersAsync()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            while (downloadRows.Count > 0 && cancellationToken.IsCancellationRequested == false)
            {
                var downloading = downloadRows.First();

                var request = new DownloadChapterRequest(downloading.Name, downloading.Url, downloading.SaveToFolder, downloading.Formats);

                var updateProgress = new Progress<string>(c =>
                {
                    downloading.Progress = c;
                    View.SetDownloadRows(downloadRows);
                });

                downloading.IsBusy = true;
                var taskResult = await worker.GetChapterAsync(request, updateProgress, cancellationToken);  
                downloading.IsBusy = false;
                downloading.Progress = "";
                if (!taskResult.Error)
                {
                    downloadRows.Remove(downloading);
                }
                View.SetDownloadRows(downloadRows);
            }
        }

        internal void StopDownload()
        {
            cancellationTokenSource?.Cancel();
        }

        internal CommonSettings LoadCommon()
        {
            return applicationConfiguration.LoadCommonSettings();
        }

        internal void SaveCommon(CommonSettings commonSettings)
        {
            applicationConfiguration.SaveCommonSettings(commonSettings);
        }
        internal void LoadDownloadChapterTasks()
        {
            var downloadRequests = applicationConfiguration.LoadDownloadChapterTasks();
            downloadRows = downloadRequests.ToList();
            View.SetDownloadRows(downloadRows);
        }

        internal void SaveDownloadChapterTasks()
        {
            applicationConfiguration.SaveDownloadChapterTasks(downloadRows);
        }

        internal IEnumerable<string> LoadBookMarks()
        {
            return applicationConfiguration.LoadBookMarks();
        }

        internal void SaveBookmarks(IEnumerable<string> sc)
        {
            applicationConfiguration.SaveBookmarks(sc);
        }
    }
}