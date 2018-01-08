using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Plugin.MangaFox
{
    /// <summary>
    /// Support find chapters, images from MangaFox
    /// </summary>
    public class MangaFox : IMangaService
    {
        private readonly ILogger Logger;
        private readonly Downloader downloader;
        private readonly IXPathSelector selector;

        public MangaFox(ILogger myLogger, Downloader downloader, IXPathSelector selector)
        {
            Logger = myLogger;
            this.downloader = downloader;
            this.selector = selector;
        }
        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaFox), "http://mangafox.la", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("mangafox.la");
        }

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            Logger.Info($@"> FindChapters(): {manga}");
            progress.Report(0);

            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            var title = selector.Select(input, "//meta[@property='og:title']").Attributes["content"];
            var chaps = selector.SelectMany(input, "//*[self::h3 or self::h4]/a[@class='tips']")
                .Select(n => new Chapter(n.InnerHtml, n.Attributes["href"]) { Manga = title });
            progress.Report(100);

            // Insert missing URI schemes in each chapter's URI.
            // Provisional solution, the current implementation may not be the best way to go about it.
            chaps = chaps.Select(chap =>
            {
                chap.Url = $"http:{chap.Url}";
                return chap;
            });

            return chaps;
        }

        public async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);

            var pages = (await FindPagesInChapter(chapter.Url, cancellationToken));
            pages = TransformPagesUrl(chapter.Url, pages);

            // find all images in pages
            int current = 0;
            var images = new List<string>();
            foreach (var page in pages)
            {
                var pageHtml = await downloader.DownloadStringAsync(page, cancellationToken);
                var image = selector
                .Select(pageHtml, "//img[@id='image']").Attributes["src"];

                images.Add(image);
                var f = (float)++current / pages.Count();
                int i = Convert.ToInt32(f * 100);
                progress.Report(i);
            }
            progress.Report(100);
            return images;
        }

        private async Task<IEnumerable<string>> FindPagesInChapter(string chapterUrl, CancellationToken cancellationToken)
        {
            var input = await downloader.DownloadStringAsync(chapterUrl, cancellationToken);
            return selector.SelectMany(input, "//form[@id='top_bar']//select[contains(@class,'m')]/option[@value != '0']")
                .Select(n => n.Attributes["value"]);
        }

        private IEnumerable<string> TransformPagesUrl(string chapterUrl, IEnumerable<string> pages)
        {
            return pages.Select(p =>
            {
                var value = new Uri(new Uri(chapterUrl), (p + ".html")).AbsoluteUri;

                return value;
            });
        }
    }
}
