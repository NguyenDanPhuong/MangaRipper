using MangaRipper.Core;
using MangaRipper.Core.Models;
using MangaRipper.Core.Plugins;
using MangaRipper.Helpers;
using MangaRipper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MangaRipper.Presentation
{
    public class MainWindowDataContext : BaseViewModel
    {
        private readonly IEnumerable<IPlugin> pluginList;
        private readonly IWorkerController worker;
        private readonly ApplicationConfiguration applicationConfiguration;
        private IEnumerable<ChapterRow> chapterRows;

        public string Url { get; set; }
        public IEnumerable<SiteInformation> SupportedSites { get; set; }
        public IEnumerable<ChapterRow> ChapterRows
        {
            get => chapterRows;
            set
            {
                chapterRows = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand GetChaptersCommand { get; set; }
        public ICommand AddSelectedCommand { get; set; }
        public ICommand AddAllCommand { get; set; }

        public MainWindowDataContext()
        {

        }
        public MainWindowDataContext(IEnumerable<IPlugin> pluginList, IWorkerController worker, ApplicationConfiguration applicationConfiguration)
        {
            this.pluginList = pluginList;
            this.worker = worker;
            this.applicationConfiguration = applicationConfiguration;

            GetChaptersCommand = new RelayCommand(async x =>
            {
                var chapters = await worker.GetChapterListAsync(Url, new Progress<string>(x => x.ToString()), new System.Threading.CancellationToken());
                ChapterRows = chapters.Select(c => new ChapterRow(c));

            });

            AddSelectedCommand = new RelayCommand(async x =>
            {
                var p = (IEnumerable<ChapterRow>)x;
            });

            AddAllCommand = new RelayCommand(async x =>
            {

            });

            Url = "http://fanfox.net/manga/world_s_end_harem/";
            SupportedSites = pluginList.Select(x => x.GetInformation()).ToList();
        }
    }

    public class RelayCommand : ICommand
    {
        #region Fields 
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        #endregion // Fields 
        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }
        #endregion // Constructors 
        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter) { _execute(parameter); }
        #endregion // ICommand Members 
    }
}
