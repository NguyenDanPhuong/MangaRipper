using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
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
        private readonly IDownloader downloader;
        private readonly IXPathSelector selector;
        private readonly IRetry retry;
        private readonly RemoteWebDriver webDriver;

        public WebDriverWait Wait { get; }

        public MangaFox(ILogger myLogger, IDownloader downloader, IXPathSelector selector, IRetry retry, RemoteWebDriver webDriver)
        {
            Logger = myLogger;
            this.downloader = downloader;
            this.selector = selector;
            this.retry = retry;
            this.webDriver = webDriver;
            Wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));

        }
        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaFox), "https://fanfox.net", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("fanfox.net");
        }

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            Logger.Info($@"> FindChapters(): {manga}");
            progress.Report(0);

            // find all chapters in a manga
            IEnumerable<Chapter> chaps = await retry.DoAsync(() =>
            {
                return DownloadAndParseChapters(manga, cancellationToken);
            }, TimeSpan.FromSeconds(3));
            progress.Report(100);

            // Insert missing URI schemes in each chapter's URI.
            // Provisional solution, the current implementation may not be the best way to go about it.
            chaps = chaps.Select(chap =>
            {
                chap.Url = $"https://fanfox.net{chap.Url}";
                return chap;
            });

            return chaps;
        }

        private async Task<IEnumerable<Chapter>> DownloadAndParseChapters(string manga, CancellationToken cancellationToken)
        {
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            var title = selector.Select(input, "//span[@class='detail-info-right-title-font']").InnerHtml;
            var hrefs = selector.SelectMany(input, "//ul[@class='detail-main-list']/li/a").Select(a => a.Attributes["href"]).ToList();
            var texts = selector.SelectMany(input, "//ul[@class='detail-main-list']/li/a/div/p[@class='title3']").Select(p => p.InnerHtml).ToList();

            var chaps = new List<Chapter>();
            for (int i = 0; i < hrefs.Count(); i++)
            {
                var chap = new Chapter(texts[i], hrefs[i]) { Manga = title };
                chaps.Add(chap);
            }

            return chaps;
        }

        public async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);

            webDriver.Url = "https://fanfox.net/manga/tian_jiang_xian_shu_nan/c001/1.html";
            var img = webDriver.FindElementByXPath("//img[@class='reader-main-img']");
            var imgList = new List<string>();
            var src = img.GetAttribute("src");
            imgList.Add(src);

            var nextButton = webDriver.FindElementByXPath("//a[@data-page][text()='>']");
            do
            {
                var currentDatapage = nextButton.GetAttribute("data-page");
                nextButton.Click();
                var src2 = img.GetAttribute("src");
                imgList.Add(src2);

                Wait.Until(driver =>
                {
                    try
                    {
                        var currentNext = webDriver.FindElementByXPath("//a[@data-page][text()='>']");
                        if (currentNext.GetAttribute("data-page") != currentDatapage)
                        {
                            nextButton = currentNext;
                            return true;
                        }
                        return false;
                    }
                    catch (NoSuchElementException)
                    {
                        nextButton = null;
                        return true;
                    }
                });

            }
            while (nextButton != null && nextButton.Displayed);
            progress.Report(100);
            return imgList;
        }

        private async Task<string> DownloadAndParseImage(string page, CancellationToken cancellationToken)
        {
            var pageHtml = await downloader.DownloadStringAsync(page, cancellationToken);
            var image = selector
            .Select(pageHtml, "//img[@id='image']").Attributes["src"];
            return image;
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
