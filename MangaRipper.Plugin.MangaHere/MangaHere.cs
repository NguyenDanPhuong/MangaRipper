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

namespace MangaRipper.Plugin.MangaHere
{

    /// <summary>
    /// Support find chapters and images from MangaHere
    /// </summary>
    public class MangaHere : IMangaService
    {
        private static ILogger logger;
        private readonly IDownloader downloader;
        private readonly IXPathSelector selector;
        private readonly IRetry retry;
        private readonly RemoteWebDriver driver;
        private WebDriverWait Wait;

        public MangaHere(ILogger myLogger, IDownloader downloader, IXPathSelector selector, IRetry retry, RemoteWebDriver driver)
        {
            logger = myLogger;
            this.downloader = downloader;
            this.selector = selector;
            this.retry = retry;
            this.driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
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
            var title = selector.Select(input, "//span[@class='detail-info-right-title-font']").InnerText;
            var hrefs = selector.SelectMany(input, "//ul[@class='detail-main-list']/li/a").Select(a => a.Attributes["href"]).ToList();
            var texts = selector.SelectMany(input, "//ul[@class='detail-main-list']/li/a/div/p[@class='title3']").Select(p => p.InnerText).ToList();

            var chaps = new List<Chapter>();
            for (int i = 0; i < hrefs.Count(); i++)
            {
                var chap = new Chapter($"{title} {texts[i]}", $"https://www.mangahere.cc{hrefs[i]}");
                chaps.Add(chap);
            }

            return chaps;
        }

        public async Task<IEnumerable<string>> FindImages(string chapterUrl, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            driver.Url = chapterUrl;
            driver.Manage().Cookies.AddCookie(new Cookie("noshowdanmaku", "1", "www.mangahere.cc", "/", null));
            driver.Manage().Cookies.AddCookie(new Cookie("isAdult", "1", "www.mangahere.cc", "/", null));
            driver.Navigate().Refresh();
            var img = driver.FindElementByXPath("//img[@class='reader-main-img']");
            var imgList = new List<string>();
            var src = img.GetAttribute("src");
            imgList.Add(src);

            var nextButton = driver.FindElementByXPath("//a[@data-page][text()='>']");
            do
            {
                var currentDatapage = nextButton.GetAttribute("data-page");
                nextButton.Click();
                var src2 = img.GetAttribute("src");
                imgList.Add(src2);

                Wait.Until(d =>
                {
                    try
                    {
                        var currentNext = driver.FindElementByXPath("//a[@data-page][text()='>']");
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
            return await Task.FromResult(imgList);
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
            return new SiteInformation(nameof(MangaHere), "https://www.mangahere.cc", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangahere.cc");
        }
    }
}
