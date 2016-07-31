using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class MangaFoxService : IMangaService
    {
        private string Link { get; set; }

        public SiteInformation GetInformation()
        {
            return new SiteInformation("MangaFox", "http://mangafox.me/", "English");
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

        public async Task<IList<string>> FindImanges(Chapter chapter, IProgress<ChapterProgress> progress, CancellationToken cancellationToken)
        {
            progress.Report(new ChapterProgress(chapter, 0));
            var downloader = new Downloader();
            var parser = new Parser();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Link);
            var pages = parser.Parse(@"<option value=""(?<Value>[^""]+)"" (|selected=""selected"")>\d+</option>", input, "Value");

            // transform pages link
            pages = pages.Select(p => {
                var value = new Uri(new Uri(chapter.Link), (p + ".html")).AbsoluteUri;
                return value;
            }).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(pages, new Progress<int>(), cancellationToken);
            var images = parser.Parse("<img src=\"(?<Value>[^\"]+)\"[ ]+onerror", pageData, "Value");

            progress.Report(new ChapterProgress(chapter, 100));
            return images;
        }
    }
}
