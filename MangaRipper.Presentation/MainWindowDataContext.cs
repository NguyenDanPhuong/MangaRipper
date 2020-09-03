using MangaRipper.Core;
using MangaRipper.Core.Extensions;
using MangaRipper.Core.Models;
using MangaRipper.Core.Plugins;
using MangaRipper.Helpers;
using MangaRipper.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace MangaRipper.Presentation
{
    public class MainWindowDataContext : BaseViewModel
    {
        private AddWindowDataContext AddWindowVM { get; set; } = new AddWindowDataContext();
        private IEnumerable<ChapterRow> chapterRows;
        private ICollection<DownloadRow> downloadRows;
        private CancellationTokenSource cancellationTokenSource;

        public string Url { get; set; } = "http://fanfox.net/manga/world_s_end_harem/";
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

        public ICollection<DownloadRow> DownloadRows
        {
            get => downloadRows; set
            {
                downloadRows = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand GetChaptersCommand { get; set; }
        public ICommand AddSelectedCommand { get; set; }
        public ICommand AddAllCommand { get; set; }
        public ICommand RemoveSelectedCommand { get; set; }
        public ICommand RemoveAllCommand { get; set; }
        public ICommand StartDownloadCommand { get; set; }
        public ICommand StopDownloadCommand { get; set; }
        public ICommand AddRemovePrefixCommand { get; set; }
        
        public MainWindowDataContext(IEnumerable<IPlugin> pluginList, IWorkerController worker)
        {
            GetChaptersCommand = new RelayCommand(async x =>
            {
                var chapters = await worker.GetChapterListAsync(Url, new Progress<string>(x => x.ToString()), new System.Threading.CancellationToken());
                ChapterRows = chapters.Select(c => new ChapterRow(c));
            });
            AddRemovePrefixCommand = new RelayCommand( x =>
            {
                
            });
            AddSelectedCommand = new RelayCommand(x =>
            {
                IList items = (IList)x;
                var collection = items.Cast<ChapterRow>();
                var a = new AddToDownloadListWindow
                {
                    DataContext = AddWindowVM
                };
                var isOk = a.ShowDialog();
                if (isOk.GetValueOrDefault())
                {
                    DownloadRows = CreateDownloadRows(collection, new OutputFormat[] { OutputFormat.Folder }, AddWindowVM.SaveToFolder);
                }
            });

            AddAllCommand = new RelayCommand(x =>
            {
                var a = new AddToDownloadListWindow
                {
                    DataContext = AddWindowVM
                };
                var isOk = a.ShowDialog();
                if (isOk.GetValueOrDefault())
                {
                    var rows  = CreateDownloadRows(ChapterRows, new OutputFormat[] { OutputFormat.Folder }, AddWindowVM.SaveToFolder).Reverse().ToList();
                    DownloadRows = rows;
                }
            });

            RemoveSelectedCommand = new RelayCommand(x =>
            {
                IList items = (IList)x;
                var collection = items.Cast<DownloadRow>();
            });
            RemoveAllCommand = new RelayCommand(x =>
            {
                DownloadRows = new List<DownloadRow>();
            });
            StartDownloadCommand = new RelayCommand(async x =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                while (DownloadRows.Count() > 0 && cancellationToken.IsCancellationRequested == false)
                {
                    var downloading = downloadRows.First();

                    var request = new DownloadChapterRequest(downloading.Name, downloading.Url, downloading.SaveToFolder, downloading.Formats);

                    var updateProgress = new Progress<string>(c =>
                    {
                        downloading.Progress = c;
                    });

                    downloading.IsBusy = true;
                    var taskResult = await worker.GetChapterAsync(request, updateProgress, cancellationToken);
                    downloading.IsBusy = false;
                    downloading.Progress = "";

                    if (!taskResult.Error)
                    {
                        DownloadRows.Remove(downloading);
                    }
                }
            });
            StopDownloadCommand = new RelayCommand(x =>
            {
                cancellationTokenSource?.Cancel();
            });

            SupportedSites = pluginList.Select(x => x.GetInformation()).ToList();
        }

        internal ICollection<DownloadRow> CreateDownloadRows(IEnumerable<ChapterRow> items, OutputFormat[] formats, string location)
        {
            var downloadRs = new List<DownloadRow>();
            foreach (var chapter in items.Where(item => downloadRs.All(r => r.Url != item.Url)))
            {
                var savePath = Path.Combine(location, chapter.DisplayName.RemoveFileNameInvalidChar());
                var task = new DownloadRow
                {
                    Name = chapter.DisplayName,
                    Url = chapter.Url,
                    SaveToFolder = savePath,
                    Formats = formats
                };
                downloadRs.Add(task);
            }
            return downloadRs;
        }
    }
}
