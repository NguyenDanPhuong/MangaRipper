using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MangaRipper.Core;
using MangaRipper.Core.Models;
using MangaRipper.Models;
using NLog;

namespace MangaRipper.Presenters
{
    public class MainViewPresenter
    {
        // TODO Implement MVP (passive)
        // So we can separate UI can UI's logic
        // Then we can unit test presenter (it likes UI Test :)) ) 
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IMainView View { get; set; }

        private IWorkerController worker;
        private IList<ChapterRow> chapterRows = new List<ChapterRow>();
        private IList<DownloadRow> downloadRows = new List<DownloadRow>();

        public MainViewPresenter(IMainView view, IWorkerController wc)
        {
            View = view;
            worker = wc;
        }

        public async Task GetChapterListAsync(string mangaUrl, bool hasPrefix)
        {
            var progress = new Progress<string>(p => View.SetChaptersProgress(p));
            chapterRows = (await worker.GetChapterListAsync(mangaUrl, progress, new CancellationTokenSource().Token))
                .Select(c => new ChapterRow(c))
                .ToList();
            GeneratePrefix(hasPrefix);
            View.SetChapters(chapterRows);
        }

        public void ChangePrefix(bool hasPrefix)
        {
            GeneratePrefix(hasPrefix);
            View.SetChapters(chapterRows);
        }

        private void GeneratePrefix(bool hasPrefix)
        {
            int count = 1;
            for (int i = chapterRows.Count() - 1; i >= 0; i--)
            {
                chapterRows[i].Prefix = hasPrefix ? count++ : 0;
            }
        }

        public async Task<DownloadChapterResponse> GetChapterAsync(DownloadChapterRequest task, IProgress<string> progress, CancellationToken cancellationToken)
        {
            return await worker.GetChapterAsync(task, progress, cancellationToken);
        }
    }
}