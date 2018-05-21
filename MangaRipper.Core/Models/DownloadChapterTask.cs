using MangaRipper.Core.Renaming;
using System.Collections.Generic;
using System.Linq;

namespace MangaRipper.Core.Models
{
    /// <summary>
    /// When chapters added to download list. They are added as tasks.
    /// A task include a chapter, and download information (percent downloaded, save to location, output format...)
    /// </summary>
    public class DownloadChapterTask
    {
        public string Name => Chapter.DisplayName;
        public string Url => Chapter.Url;
        public string PropFormats
        {
            get
            {
                var s = Formats.Select(format => format.ToString()).ToList();
                return string.Join(", ", s);
            }
        }

        public Chapter Chapter { get; private set; }
        public string SaveToFolder { get; private set; }
        public IEnumerable<OutputFormat> Formats { get; private set; }
        public bool IsBusy { get; set; }
        public int Percent { get; set; }

        public IRenamer Renamer { get; set; }

        public DownloadChapterTask(Chapter chapter, string saveToFolder, IEnumerable<OutputFormat> formats)
        {
            Chapter = chapter;
            SaveToFolder = saveToFolder;
            Formats = formats;
        }
    }
}
