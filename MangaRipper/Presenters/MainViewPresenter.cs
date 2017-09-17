using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using MangaRipper.Core.Providers;
using NLog;
using MangaRipper.Core.Models;
using System.ComponentModel;
using System.Linq;
using MangaRipper.Core.DataTypes;
using System.Collections.Generic;
using MangaRipper.Helpers;

namespace MangaRipper.Presenters
{
    public class MainViewPresenter
    {
        // TODO Implement MVP (passive)
        // So we can separate UI can UI's logic
        // Then we can unit test presenter (it likes UI Test :)) ) 
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IMainView View { get; set; }
        private readonly ApplicationConfiguration _appConf = new ApplicationConfiguration();
        private BindingList<DownloadChapterTask> _downloadQueue = new BindingList<DownloadChapterTask>();
        public MainViewPresenter(IMainView view)
        {
            View = view;
            View.FindChaptersClicked = OnFindChapters;
            View.AddDownloadClicked = OnAddDownloadClicked;
            View.StartDownloadClicked = OnStartDownload;
            View.RemoveDownloadClicked = OnRemoveDownload;
            View.MainFormLoaded = OnMainFormLoaded;
            View.MainFormClosing = OnMainFormClosing;
        }

        private void OnMainFormLoaded()
        {
            _downloadQueue = _appConf.LoadDownloadChapterTasks();
            View.SetDownloadQueue(_downloadQueue);
        }

        private void OnMainFormClosing()
        {
            _appConf.SaveDownloadChapterTasks(_downloadQueue);
        }

        private void OnRemoveDownload(IEnumerable<DownloadChapterTask> obj)
        {
            obj
                .Where(c => !c.IsBusy)
                .ToList()
                .ForEach(c =>
            {
                _downloadQueue.Remove(c);
            });
        }

        private void OnAddDownloadClicked(IEnumerable<Chapter> chapters, string destination, OutputFormat[] formats)
        {
            foreach (var chapter in chapters.Where(item => _downloadQueue.All(r => r.Chapter.Url != item.Url)))
            {
                _downloadQueue.Add(new DownloadChapterTask(chapter, destination, formats));
            }
        }

        private async Task OnFindChapters(string obj)
        {
            try
            {
                var worker = FrameworkProvider.GetWorker();
                var progressInt = new Progress<int>(progress => View.SetChaptersProgress(progress + @"%"));
                var chapters = await worker.FindChapterListAsync(obj, progressInt);
                View.SetChapters(chapters);
            }
            catch (OperationCanceledException ex)
            {
                View.SetStatusText(@"Download cancelled! Reason: " + ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                View.SetStatusText(@"Download cancelled! Reason: " + ex.Message);
                View.ShowMessageBox(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task OnStartDownload()
        {
            while (_downloadQueue.Count > 0)
            {
                var chapter = _downloadQueue.First();
                var worker = FrameworkProvider.GetWorker();

                await worker.RunDownloadTaskAsync(chapter, new Progress<int>(c =>
                {
                    chapter.Percent = c;
                    View.RefreshTaskGrid();
                }));

                _downloadQueue.Remove(chapter);
            }
        }
    }
}