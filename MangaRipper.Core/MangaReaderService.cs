using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    /// <summary>
    /// Support find chapters and images from MangaReader
    /// </summary>
    class MangaReaderService : IMangaService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new Parser();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a href=\"(?<Value>[^\"]+)\">(?<Name>[^<]+)</a> :", input, "Name", "Value");
            // reverse chapters order and remove duplicated chapters in latest setion
            chaps = chaps.Reverse().GroupBy(x => x.Url).Select(g => g.First()).ToList();
            // transform pages link
            chaps = chaps.Select(c =>
            {
                return new Chapter(c.Name, new Uri(new Uri(manga), c.Url).AbsoluteUri);
            }).ToList();
            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new Parser();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url);
            var pages = parser.Parse(@"<option value=""(?<Value>[^""]+)""(| selected=""selected"")>\d+</option>", input, "Value");

            // transform pages link
            pages = pages.Select(p =>
            {
                var value = new Uri(new Uri(chapter.Url), p).AbsoluteUri;
                return value;
            }).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(pages, new Progress<int>((count) =>
            {
                var f = (float)count / pages.Count();
                int i = Convert.ToInt32(f * 100);
                progress.Report(i);
            }), cancellationToken);
            var images = parser.Parse(@"<img id=""img"" width=""\d+"" height=""\d+"" src=""(?<Value>[^""]+)""", pageData, "Value");

            return images;
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation("MangaReader", "http://www.mangareader.net", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangareader.net");
        }
    }
}
