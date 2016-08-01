using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    class MangaFoxService : IMangaService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SiteInformation GetInformation()
        {
            return new SiteInformation("MangaFox", "http://mangafox.me", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("mangafox.me");
        }

        public async Task<IList<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new Downloader();
            var parser = new Parser();

            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a href=\"(?<Value>[^\"]+)\" title=\"(|[^\"]+)\" class=\"tips\">(?<Name>[^<]+)</a>", input, "Name", "Value");
            progress.Report(100);
            return chaps;
        }

        public async Task<IList<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new Downloader();
            var parser = new Parser();

            // find all pages in a chapter
            Trace.WriteLine("FindImanges > DownloadStringAsync: " + chapter.Link);
            string input = await downloader.DownloadStringAsync(chapter.Link);
            Trace.WriteLine("FindImanges > Input Length: " + input.Length);
            var newInput = input.Substring(13000, 3000);
            Trace.WriteLine(newInput);
            var pages = parser.Parse(@"<option value=""(?<Value>[^""]+)"" (|selected=""selected"")>\d+</option>", input, "Value");
            Trace.WriteLine("FindImanges > Before Count: " + pages.Count);
            // transform pages link
            pages = pages.Select(p =>
            {
                var value = new Uri(new Uri(chapter.Link), (p + ".html")).AbsoluteUri;
                Trace.WriteLine("FindImanges > Transform: " + p + " " + value);
                return value;
            }).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(
                pages,
                new Progress<int>((count) =>
                {
                    var f = (float) count / pages.Count;
                    int i = Convert.ToInt32(f * 100);
                    progress.Report(i);
                }),
                cancellationToken);
            var images = parser.Parse("<img src=\"(?<Value>[^\"]+)\"[ ]+onerror", pageData, "Value");

            progress.Report(100);
            return images;
        }
    }
}
