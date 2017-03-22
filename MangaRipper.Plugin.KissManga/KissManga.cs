using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace MangaRipper.Plugin.KissManga
{
    /// <summary>
    /// Support find chapters and images from KissManga
    /// </summary>
    public class KissManga : MangaService, IDisposable
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private IWebDriver WebDriver;
        private WebDriverWait Wait;

        public KissManga()
        {
            var serviceJs = PhantomJSDriverService.CreateDefaultService();
            serviceJs.HideCommandPromptWindow = true;
            WebDriver = new PhantomJSDriver(serviceJs);
            WebDriver.Navigate().GoToUrl("http://kissmanga.com/");
            Wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(15));
            Wait.Until(ExpectedConditions.TitleContains("KissManga"));
        }

        public override async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<td>\\s+<a\\s+href=\"(?=/Manga/)(?<Value>.[^\"]*)\"\\s+title=\"(?<Name>.[^\"]*)\"", input, "Name", "Value");
            chaps = chaps.Select(c => NameResolver(c.Name, c.Url, new Uri(manga)));
            progress.Report(100);
            return chaps;
        }

        public override async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            WebDriver.Navigate().GoToUrl(chapter.Url);
            Wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//select[@id='selectReadType']")));
            progress.Report(50);
            var selectTag = WebDriver.FindElement(By.XPath("//select[@id='selectReadType']"));
            var s = new SelectElement(selectTag);
            s.SelectByText("All pages");
            Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@id='divImage']")));
            var images = WebDriver.FindElements(By.XPath("//p/img[@onload][@src]"));
            progress.Report(100);
            return images.Select(i => i.GetAttribute("src"));
        }

        public override SiteInformation GetInformation()
        {
            return new SiteInformation("KissManga", "http://kissmanga.com/", "English");
        }

        public override bool Of(string link)
        {
            return new Uri(link).Host.Equals("kissmanga.com");
        }

        private Chapter NameResolver(string name, string value, Uri adress)
        {
            var urle = new Uri(adress, value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = System.Net.WebUtility.HtmlDecode(name);
                name = Regex.Replace(name, "^Read\\s+|\\s+online$|:", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                name = Regex.Replace(name, "\\s+Read\\s+Online$", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            return new Chapter(name, urle.AbsoluteUri);
        }

        public void Dispose()
        {
            WebDriver.Quit();
        }
    }
}
