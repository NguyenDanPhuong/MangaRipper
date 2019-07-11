using System.Collections.Generic;

namespace MangaRipper.Core.Models
{
    /// <summary>
    /// When chapters added to download list. They are added as tasks.
    /// A task include a chapter, and download information (percent downloaded, save to location, output format...)
    /// </summary>
    public class DownloadChapterRequest
    {
        public string Name { get; }
        public string Url { get; }
        public string SaveToFolder { get; private set; }
        public IEnumerable<OutputFormat> Formats { get; private set; }


        public DownloadChapterRequest(string chapterName, string chapterUrl, string saveToFolder, IEnumerable<OutputFormat> formats)
        {
            Name = chapterName;
            Url = chapterUrl;
            SaveToFolder = saveToFolder;
            Formats = formats;
        }
    }
}
