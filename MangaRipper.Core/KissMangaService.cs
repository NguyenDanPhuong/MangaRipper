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
    /// <summary>
    /// Support find chapters, images from MangaFox
    /// </summary>
    class KissMangaService : IMangaService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SiteInformation GetInformation()
        {
            return new SiteInformation("KissManga", "http://kissmanga.com/", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("kissmanga.com");
        }

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new Downloader();
            var parser = new Parser();

            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a href=\"(?<Value>/Manga/[^\"]+)\" title=\"[^\"]+\">\r(?<Name>[^<]+)</a>", input, "Name", "Value");
            chaps = chaps.Select(c =>
            {
                var uri = new Uri(new Uri("http://kissmanga.com"), c.Url);
                return new Chapter(c.Name, uri.AbsoluteUri);
            });
            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new Downloader();
            var parser = new Parser();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url);
            var images = parser.Parse(@"<option value=""(?<Value>[^""]+)"" (|selected=""selected"")>\d+</option>", input, "Value");
            // transform pages link
            images = images.Select(p =>
            {
                var value = new Uri(new Uri(chapter.Url), (p + ".html")).AbsoluteUri;
                return value;
            }).ToList();

            progress.Report(100);
            return images;
        }
    }
}
