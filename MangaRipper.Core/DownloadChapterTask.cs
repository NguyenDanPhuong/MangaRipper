using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class DownloadChapterTask
    {
        public string Name => Chapter.Name;
        public string Link => Chapter.Link;
        public Chapter Chapter { get; private set; }
        public string SaveToFolder { get; private set; }
        public IEnumerable<OutputFormat> Formats { get; private set; }
        public bool IsBusy { get; set; }
        public int Percent { get; set; }
        public DownloadChapterTask(Chapter chapter, string saveToFolder, IEnumerable<OutputFormat> formats)
        {
            Chapter = chapter;
            SaveToFolder = saveToFolder;
            Formats = formats;
        }
    }
}
