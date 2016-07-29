using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public abstract class TitleBase : ITitle
    {

        protected virtual List<string> ParseChapterAddresses(string html)
        {
            return null;
        }

        abstract protected List<IChapter> ParseChapterObjects(string html);

        public string Address
        {
            get;
            protected set;
        }

        public TitleBase(string address)
        {
            Address = address;
        }

        public async Task<IList<IChapter>> PopulateChapterAsync(IProgress<int> progress)
        {
            progress.Report(0);

            string html = await Downloader.DownloadStringAsync(Address);

            var sb = new StringBuilder();
            sb.AppendLine(html);

            List<string> uris = ParseChapterAddresses(html);

            if (uris != null)
            {
                int count = 0;
                foreach (string item in uris)
                {
                    string content = await Downloader.DownloadStringAsync(item);
                    sb.AppendLine(content);
                    count++;
                    progress.Report(count * 100 / uris.Count);
                }
            }

            var chapters = ParseChapterObjects(sb.ToString());

            progress.Report(100);
            return chapters;
        }
    }
}
