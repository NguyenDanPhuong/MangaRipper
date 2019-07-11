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
        private readonly ILogger Logger;
        private readonly IHttpDownloader downloader;
        private readonly IXPathSelector selector;
        private readonly IRetry retry;
        private readonly RemoteWebDriver webDriver;

        public WebDriverWait Wait { get; }

        public MangaFox(ILogger myLogger, IHttpDownloader downloader, IXPathSelector selector, IRetry retry, RemoteWebDriver webDriver)
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

        public async Task<IEnumerable<Chapter>> GetChapters(string manga, IProgress<string> progress, CancellationToken cancellationToken)
        {
            Logger.Info($@"> FindChapters(): {manga}");

            IEnumerable<Chapter> chaps = await retry.DoAsync(() =>
            {
                return GetChaptersImpl(manga, cancellationToken);
            }, TimeSpan.FromSeconds(3));

            chaps = chaps.Select(chap =>
            {
                chap.Url = $"https://fanfox.net{chap.Url}";
                return chap;
            });

            return chaps;
        }

        private async Task<IEnumerable<Chapter>> GetChaptersImpl(string manga, CancellationToken cancellationToken)
        {
            string input = await downloader.GetStringAsync(manga, cancellationToken);
            var title = selector.Select(input, "//span[@class='detail-info-right-title-font']").InnerText;
            var hrefs = selector.SelectMany(input, "//ul[@class='detail-main-list']/li/a").Select(a => a.Attributes["href"]).ToList();
            var texts = selector.SelectMany(input, "//ul[@class='detail-main-list']/li/a/div/p[@class='title3']").Select(p => p.InnerText).ToList();

            var chaps = new List<Chapter>();
            for (int i = 0; i < hrefs.Count(); i++)
            {
                var chap = new Chapter($"{title} {texts[i]}", hrefs[i]);
                chaps.Add(chap);
            }

            return chaps;
        }

        public async Task<IEnumerable<string>> GetImages(string chapterUrl, IProgress<string> progress, CancellationToken cancellationToken)
        {
            webDriver.Url = chapterUrl;
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
                var src2 = img.GetAttribute("src");
                imgList.Add(src2);
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
