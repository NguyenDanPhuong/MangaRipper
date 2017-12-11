using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using MangaRipper.Core.Controllers;

namespace MangaRipper.Presenters
{
    public class MainViewPresenter
    {
        // TODO Implement MVP (passive)
        // So we can separate UI can UI's logic
        // Then we can unit test presenter (it likes UI Test :)) ) 
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IMainView View { get; set; }

        private WorkerController worker;

        public MainViewPresenter(IMainView view, WorkerController wc)
        {
            View = view;
            worker = wc;
        }

        public async Task OnFindChapters(string obj)
        {
            try
            {
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
                View.ShowMessageBox(ex.Source, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Information);
                View.EnableTheButtonsAfterError();
            }
        }
    }
}