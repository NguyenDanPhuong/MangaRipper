using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Plugin.MangaReader
{
    /// <summary>
    /// Support find chapters and images from MangaReader
    /// </summary>
    public class MangaReader : IMangaService
    {
        private static ILogger logger;
        private readonly Downloader downloader;
        private readonly IXPathSelector selector;

        public MangaReader(ILogger myLogger, Downloader downloader, IXPathSelector selector)
        {
            logger = myLogger;
            this.downloader = downloader;
            this.selector = selector;
        }
        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            var chaps = selector.SelectMany(input, "//table[@id='listing']//a")
                .Select(n => new Chapter(n.InnerHtml, n.Attributes["href"]));
            // reverse chapters order and remove duplicated chapters in latest section
            chaps = chaps.Reverse().GroupBy(x => x.Url).Select(g => g.First()).ToList();
            // transform pages link
            chaps = chaps.Select(c => new Chapter(c.Name, new Uri(new Uri(manga), c.Url).AbsoluteUri)).ToList();
            progress.Report(100);
            return chaps;
        }
        
        public async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url, cancellationToken);
            var pages = selector.SelectMany(input, "//select[@id='pageMenu']/option")
                .Select(n => n.Attributes["value"]);

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
            var images = selector.SelectMany(pageData, "//img[@id='img']").Select(n => n.Attributes["src"]);

            return images;
        }


        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaReader), "http://www.mangareader.net", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangareader.net");
        }
    }
}
