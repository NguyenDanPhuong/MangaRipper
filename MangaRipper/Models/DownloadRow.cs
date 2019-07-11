using MangaRipper.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace MangaRipper.Models
{
    public class DownloadRow
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string SaveToFolder { get; set; }
        public IEnumerable<OutputFormat> Formats { get; set; }
        public bool IsBusy { get; set; }
        public string Progress { get; internal set; }

        public string PropFormats
        {
            get
            {
                var s = Formats.Select(format => format.ToString()).ToList();
                return string.Join(", ", s);
            }
        }
    }
}
