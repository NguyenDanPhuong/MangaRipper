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

namespace MangaRipper.Plugin.MangaReader
{
    /// <summary>
    /// Support find chapters and images from MangaReader
    /// </summary>
    public class MangaReader : IPlugin
    {
        private static ILogger<MangaReader> logger;
        private readonly IHttpDownloader downloader;
        private readonly RemoteWebDriver driver;

        private readonly WebDriverWait Wait;

        public MangaReader(ILogger<MangaReader> myLogger, IHttpDownloader downloader, RemoteWebDriver driver)
        {
            logger = myLogger;
            this.downloader = downloader;
            this.driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        }
        public async Task<IEnumerable<Chapter>> GetChapters(string manga, IProgress<string> progress, CancellationToken cancellationToken)
        {
            string input = await downloader.GetStringAsync(manga, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            var chaps = doc.DocumentNode
                .SelectNodes("//i[@class='d16 d45']/following-sibling::a")
                .Select(n =>
                {
                    string url = n.Attributes["href"].Value;
                    var resultUrl = new Uri(new Uri(manga), url).AbsoluteUri;
                    return new Chapter(n.InnerText, resultUrl);
                });
            chaps = chaps.Reverse().GroupBy(x => x.Url).Select(g => g.First()).ToList();
            return chaps;
        }

        public async Task<IEnumerable<string>> GetImages(string chapterUrl, IProgress<string> progress, CancellationToken cancellationToken)
        {
            // find all pages in a chapter
            driver.Url = chapterUrl;
            var scrollMode = driver.FindElementByXPath("//div[@id='swsc']");
            scrollMode.Click();
            Wait.Until(driver => {

                var x = driver.FindElements(By.XPath("//img[@data-id]"));

                var isLoading = x.Any(p => p.GetAttribute("src").StartsWith("data:image/svg+xml"));
                if (isLoading)
                {
                    return false;
                }

               
                return true;
            });

            var pages = driver.FindElements(By.XPath("//img[@data-id]"));
            var srts= pages.Select(n => n.GetAttribute("src"))
               .Select(p =>
               {
                   var value = new Uri(new Uri(chapterUrl), p).AbsoluteUri;
                   return value;
               }).ToList();

            progress.Report("Detecting: " + pages.Count);

            return await Task.FromResult(srts.AsEnumerable());
        }


        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaReader), "https://www.mangareader.net", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangareader.net");
        }
    }
}
