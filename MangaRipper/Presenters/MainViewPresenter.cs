using System;
using System.Threading.Tasks;
using MangaRipper.Core.Providers;

namespace MangaRipper.Presenters
{
    public class MainViewPresenter
    {
        private IMainView View { get; set; }
        public MainViewPresenter(IMainView view)
        {
            View = view;
            View.FindChaptersClicked = async x =>
            {
                var worker = FrameworkProvider.GetWorker();
                var progressInt = new Progress<int>(progress => View.SetChaptersProgress(progress + @"%"));
                var chapters = await worker.FindChapterListAsync(x, progressInt);
                View.SetChapters(chapters);
            };
        }
    }
}