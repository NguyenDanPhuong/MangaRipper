using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;

namespace MangaRipper.Plugin.ReadOPM
{
    /// <summary>
    /// Support find chapters, images from MangaStream
    /// </summary>
    public class ReadOPM : IMangaService
    {
        private static ILogger logger;
        private readonly IDownloader downloader;
        private readonly IXPathSelector selector;

        public ReadOPM(ILogger myLogger, IDownloader downloader, IXPathSelector selector)
        {
            logger = myLogger;
            this.downloader = downloader;
            this.selector = selector;
        }
        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress,
            CancellationToken cancellationToken)
        {
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            var title = selector.Select(input, "//h1").InnerHtml;
            var chaps = selector
                .SelectMany(input, "//ul[contains(@class, 'chapters-list')]/li/a")
                .Select(n =>
                {
                    string url = n.Attributes["href"];
                    string name = null;//selector.Select(n.InnerHtml, "//span[contains(@class, 'chapter__no')]").InnerHtml;
                    return new Chapter(name, url) { Manga = "One Punch Man" };
                }).ToList();

            var chap_names = selector
                .SelectMany(input, "//ul[contains(@class, 'chapters-list')]/li/a/span[contains(@class, 'chapter__no')]")
                .Select(n => n.InnerHtml)
                .ToList();

            chaps.ForEach(c => c.Name = chap_names[chaps.IndexOf(c)]);

            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress,
            CancellationToken cancellationToken)
        {
            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url, cancellationToken);
            var images = selector.SelectMany(input, "//div[contains(@class,'img_container')]/img")
                .Select(n => n.Attributes["src"]);

            return images;
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(ReadOPM), "http://ww3.readopm.com/", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("ww3.readopm.com");
        }
    }
}
