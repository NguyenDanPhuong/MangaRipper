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


        public string Name { get; }
        public string Url { get; }
        public string SaveToFolder { get; private set; }
        public IEnumerable<OutputFormat> Formats { get; private set; }


        public DownloadChapterTask(string chapterName, string chapterUrl, string saveToFolder, IEnumerable<OutputFormat> formats)
        {
            Name = chapterName;
            Url = chapterUrl;
            SaveToFolder = saveToFolder;
            Formats = formats;
        }
    }
}
