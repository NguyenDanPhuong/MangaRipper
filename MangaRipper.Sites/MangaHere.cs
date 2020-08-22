using HtmlAgilityPack;
using MangaRipper.Core.Logging;
using MangaRipper.Core.Models;
using MangaRipper.Core.Plugins;
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
    public class MangaHere : IPlugin
    {
        private static ILogger<MangaHere> logger;
        private readonly IHttpDownloader downloader;
        private readonly IRetry retry;
        private readonly RemoteWebDriver driver;
        private readonly WebDriverWait Wait;

        public MangaHere(ILogger<MangaHere> myLogger, IHttpDownloader downloader, IRetry retry, RemoteWebDriver driver)
        {
            logger = myLogger;
            this.downloader = downloader;
            this.retry = retry;
            this.driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }
        public async Task<IReadOnlyList<Chapter>> GetChapters(string manga, IProgress<string> progress, CancellationToken cancellationToken)
        {
            var chapters = await retry.DoAsync(() =>
            {
                return GetChaptersImpl(manga, cancellationToken);
            }, TimeSpan.FromSeconds(3));
            return chapters;
        }

        private async Task<IReadOnlyList<Chapter>> GetChaptersImpl(string mangaUrl, CancellationToken cancellationToken)
        {
           
            string input = await downloader.GetStringAsync(mangaUrl, cancellationToken);
            var x = new HtmlDocument();
            x.LoadHtml(input);
            var title = x.DocumentNode.SelectSingleNode("//span[@class='detail-info-right-title-font']").InnerText;
            var hrefs = x.DocumentNode.SelectNodes( "//ul[@class='detail-main-list']/li/a").Select(a => a.Attributes["href"]).Select(a => a.Value).ToList();
            var texts = x.DocumentNode.SelectNodes( "//ul[@class='detail-main-list']/li/a/div/p[@class='title3']").Select(p => p.InnerText).ToList();

            var chaps = new List<Chapter>();
            for (int i = 0; i < hrefs.Count(); i++)
            {
                var chap = new Chapter($"{title} {texts[i]}", $"https://www.mangahere.cc{hrefs[i]}");
                chaps.Add(chap);
            }

            return chaps;
        }

        public async Task<IReadOnlyList<string>> GetImages(string chapterUrl, IProgress<string> progress, CancellationToken cancellationToken)
        {
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
                cancellationToken.ThrowIfCancellationRequested();
                var currentDatapage = nextButton.GetAttribute("data-page");
                nextButton.Click();
                Wait.Until(driver =>
                {
                    var src2 = img.GetAttribute("src");
                    if (!src2.EndsWith("loading.gif"))
                    {
                        imgList.Add(src2);
                        return true;
                    }
                    return false;
                });
                progress.Report("Detecting: " + imgList.Count);
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
            return await Task.FromResult(imgList);
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
