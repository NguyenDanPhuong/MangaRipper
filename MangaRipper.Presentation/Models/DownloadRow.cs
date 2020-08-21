using MangaRipper.Core.Models;
using MangaRipper.Presentation;
using System.Collections.Generic;
using System.Linq;

namespace MangaRipper.Models
{
    public class DownloadRow : BaseViewModel
    {
        private string progress;
        private bool isBusy;

        public DownloadRow()
        {
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public string SaveToFolder { get; set; }
        public IEnumerable<OutputFormat> Formats { get; set; }
        public bool IsBusy
        {
            get => isBusy; set
            {
                isBusy = value;
                NotifyPropertyChanged();
            }
        }
        public string Progress
        {
            get => progress; internal set
            {
                progress = value;
                NotifyPropertyChanged();
            }
        }
    }
}
