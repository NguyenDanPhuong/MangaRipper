using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public abstract class TitleBase : ITitle
    {

        protected virtual List<Uri> ParseChapterAddresses(string html)
        {
            return null;
        }

        abstract protected List<IChapter> ParseChapterObjects(string html);

        public List<IChapter> Chapters
        {
            get;
            protected set;
        }

        public Uri Address
        {
            get;
            protected set;
        }

        public IWebProxy Proxy { get; set; }

        public TitleBase(Uri address)
        {
            Address = address;
        }

        public Task PopulateChapterAsync(Progress<int> progress)
        {
            return Task.Factory.StartNew(() =>
            {
                progress.ReportProgress(0);

                var client = new WebClient();
                client.Proxy = Proxy;
                client.Encoding = Encoding.UTF8;
                string html = client.DownloadString(Address);

                var sb = new StringBuilder();
                sb.AppendLine(html);

                List<Uri> uris = ParseChapterAddresses(html);

                if (uris != null)
                {
                    int count = 0;
                    foreach (Uri item in uris)
                    {
                        string content = client.DownloadString(item);
                        sb.AppendLine(content);
                        count++;
                        progress.ReportProgress(count * 100 / uris.Count);
                    }
                }

                Chapters = ParseChapterObjects(sb.ToString());

                progress.ReportProgress(100);
            });
        }
    }
}
