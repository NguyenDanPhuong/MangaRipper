using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    class MangaHereService : IMangaService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public async Task<IList<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new Parser();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a class=\"color_0077\" href=\"(?<Value>http://[^\"]+)\"[^<]+>(?<Text>[^<]+)</a>", input, "Name", "Value");
            progress.Report(100);
            return chaps;
        }

        public async Task<IList<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new Parser();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Link);
            var pages = parser.Parse(@"<option value=""(?<Value>[^""]+)"" (|selected=""selected"")>\d+</option>", input, "Value");

            // transform pages link
            pages = pages.Select(p =>
            {
                var value = new Uri(new Uri(chapter.Link), p).AbsoluteUri;
                return value;
            }).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(pages, new Progress<int>((count) =>
            {
                var f = (float)count / pages.Count;
                int i = Convert.ToInt32(f * 100);
                progress.Report(i);
            }), cancellationToken);
            var images = parser.Parse("<img src=\"(?<Value>[^\"]+)\"[ ]+onerror", pageData, "Value");

            return images;
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation("MangaHere", "http://www.mangahere.co", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangahere.co");
        }
    }
}
