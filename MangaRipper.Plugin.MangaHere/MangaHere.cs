using MangaRipper.Core;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Plugin.MangaHere
{

    /// <summary>
    /// Support find chapters and images from MangaHere
    /// </summary>
    public class MangaHere : IMangaService
    {
        private static ILogger logger;
        private readonly Downloader downloader;
        private readonly IXPathSelector selector;
        private readonly IRetry retry;

        public MangaHere(ILogger myLogger, Downloader downloader, IXPathSelector selector, IRetry retry)
        {
            logger = myLogger;
            this.downloader = downloader;
            this.selector = selector;
            this.retry = retry;
        }
        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            // find all chapters in a manga
            var chapters = await retry.DoAsync(() =>
            {
                return DownloadAndParseChapters(manga, cancellationToken);
            }, TimeSpan.FromSeconds(3));
            progress.Report(100);
            return chapters;
        }

        private async Task<IEnumerable<Chapter>> DownloadAndParseChapters(string manga, CancellationToken cancellationToken)
        {
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            var title = selector.Select(input, "//meta[@property='og:title']").Attributes["content"];
            var chaps = selector.SelectMany(input, "//div[contains(@class,'detail_list')]/ul//a")
                .Select(n =>
                {
                    return new Chapter(n.InnerHtml.Trim(), $"http:{n.Attributes["href"]}") { Manga = title };
                });
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            // find all pages in a chapter
            var pages = await retry.DoAsync(() =>
            {
                return DownloadAndParsePages(chapter, cancellationToken);
            }, TimeSpan.FromSeconds(3));

            // find all images in pages
            int current = 0;
            var images = new List<string>();
            foreach (var page in pages)
            {
                string image = await retry.DoAsync(() =>
                {
                    return DownloadAndParseImage(page, cancellationToken);
                }, TimeSpan.FromSeconds(3));
                images.Add(image);
                var f = (float)++current / pages.Count();
                int i = Convert.ToInt32(f * 100);
                progress.Report(i);
            }

            return images;
        }

        private async Task<string> DownloadAndParseImage(string page, CancellationToken cancellationToken)
        {
            var pageHtml = await downloader.DownloadStringAsync(page, cancellationToken);
            var image = selector
            .Select(pageHtml, "//section[contains(@class,'read_img')]/a/img[@id='image']").Attributes["src"];
            return image;
        }

        private async Task<IEnumerable<string>> DownloadAndParsePages(Chapter chapter, CancellationToken cancellationToken)
        {
            string input = await downloader.DownloadStringAsync(chapter.Url, cancellationToken);
            var pages = selector
                .SelectMany(input, "//section[contains(@class,'readpage_top')]//select[contains(@class,'wid60')]/option[not(text()='Featured')]")
                .Select(n =>
                {
                    return new Uri(new Uri(chapter.Url), n.Attributes["value"]).AbsoluteUri;
                });
            return pages;
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaHere), "http://www.mangahere.cc", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangahere.cc");
        }
    }
}
