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

namespace MangaRipper.Plugin.MangaFox
{
    /// <summary>
    /// Support find chapters, images from MangaFox
    /// </summary>
    public class MangaFox : IPlugin
    {
        private readonly ILogger<MangaFox> Logger;
        private readonly IHttpDownloader downloader;
        private readonly IRetry retry;
        private readonly RemoteWebDriver webDriver;

        public WebDriverWait Wait { get; }

        public MangaFox(ILogger<MangaFox> myLogger, IHttpDownloader downloader, IRetry retry, RemoteWebDriver webDriver)
        {
            Logger = myLogger;
            this.downloader = downloader;
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

        public async Task<IReadOnlyList<Chapter>> GetChapters(string manga, IProgress<string> progress, CancellationToken cancellationToken)
        {
            Logger.Info($@"> FindChapters(): {manga}");

            IReadOnlyList<Chapter> chaps = await retry.DoAsync(() =>
            {
                return GetChaptersImpl(manga, cancellationToken);
            }, TimeSpan.FromSeconds(3));

            var result = chaps.Select(chap =>
            {
                chap.Url = $"https://fanfox.net{chap.Url}";
                return chap;
            });

            return result.ToList();
        }

        private async Task<IReadOnlyList<Chapter>> GetChaptersImpl(string manga, CancellationToken cancellationToken)
        {
            string input = await downloader.GetStringAsync(manga, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            var title = doc.DocumentNode.SelectSingleNode( "//span[@class='detail-info-right-title-font']").InnerText;
            var hrefs = doc.DocumentNode.SelectNodes( "//ul[@class='detail-main-list']/li/a").Select(a => a.Attributes["href"]).Select(h => h.Value).ToList();
            var texts = doc.DocumentNode.SelectNodes( "//ul[@class='detail-main-list']/li/a/div/p[@class='title3']").Select(p => p.InnerText).ToList();

            var chaps = new List<Chapter>();
            for (int i = 0; i < hrefs.Count(); i++)
            {
                var chap = new Chapter($"{title} {texts[i]}", hrefs[i]);
                chaps.Add(chap);
            }

            return chaps;
        }

        public async Task<IReadOnlyList<string>> GetImages(string chapterUrl, IProgress<string> progress, CancellationToken cancellationToken)
        {
            webDriver.Url = chapterUrl;
            webDriver.Manage().Cookies.AddCookie(new Cookie("noshowdanmaku", "1", "fanfox.net", "/", null));
            webDriver.Manage().Cookies.AddCookie(new Cookie("isAdult", "1", "fanfox.net", "/", null));
            webDriver.Navigate().Refresh();
            var img = webDriver.FindElementByXPath("//img[@class='reader-main-img']");
            var imgList = new List<string>();
            var src = img.GetAttribute("src");
            imgList.Add(src);

            var nextButton = webDriver.FindElementByXPath("//a[@data-page][text()='>']");
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
            return await Task.FromResult(imgList);
        }
    }
}
