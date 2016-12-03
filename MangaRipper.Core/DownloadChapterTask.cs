using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class DownloadChapterTask
    {
        public Chapter Chapter { get; private set; }
        public string SaveToFolder { get; private set; }
        public IEnumerable<OutputFormat> Formats { get; private set;}
        public DownloadChapterTask(Chapter chapter, string saveToFolder, IEnumerable<OutputFormat> formats)
        {
            Chapter = chapter;
            SaveToFolder = saveToFolder;
            Formats = formats;
        }
    }
}
