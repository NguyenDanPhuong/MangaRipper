using MangaRipper.Core;
using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Plugin.MangaFox;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MangaRipper.Test
{
    /// <summary>
    /// Test with unlicensed manga. Appveyor CI is US based and cannot access licensed manga in the US. 
    /// If we test with a licensed manga, this test will failed.
    /// </summary>
    public class FanfoxTests : IDisposable
    {
        readonly ChromeDriver ChromeDriver;
        private readonly WebDriverWait Wait;
        readonly Mock<ILogger> logger = new Mock<ILogger>();
        readonly IDownloader downloader;
        readonly MangaFox service;
        private readonly CancellationTokenSource source;

        public FanfoxTests()
        {
            source = new CancellationTokenSource();

            var options = new ChromeOptions();
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--start-maximized");
            options.AddArgument("--headless");
            ChromeDriver = new ChromeDriver(options);
            Wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(10));


            downloader = new Downloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new MangaFox(logger.Object, downloader, new HtmlAtilityPackAdapter(), new Retry());
        }

        public void Dispose()
        {
            ChromeDriver.Close();
            ChromeDriver.Dispose();
        }

        [Fact]
        public async void FindChapters()
        {
            string url = "https://fanfox.net/manga/tian_jiang_xian_shu_nan/";
            Assert.True(service.Of(url));
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.Equal("Tian Jiang Xian Shu Nan", chapter.Manga);
            Assert.Equal("Ch.001", chapter.DisplayName);
            Assert.Equal("https://fanfox.net/manga/tian_jiang_xian_shu_nan/c001/1.html", chapter.Url);
        }


        [Fact]
        public async void FindImages()
        {
            var chapter = new Chapter("Ch.001", "https://fanfox.net/manga/tian_jiang_xian_shu_nan/c001/1.html")
            {
                Manga = "Tian Jiang Xian Shu Nan"
            };
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(15, images.Count());
            Assert.StartsWith("https://a.fanfox.net/store/manga/19803/001.0/compressed/q001.jpg", images.ToArray()[0]);
            Assert.StartsWith("https://a.fanfox.net/store/manga/19803/001.0/compressed/q002.jpg", images.ToArray()[1]);
            Assert.StartsWith("https://a.fanfox.net/store/manga/19803/001.0/compressed/q003.jpg", images.ToArray()[2]);

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }

        [Fact]
        public void Test()
        {
            ChromeDriver.Url = "https://fanfox.net/manga/tian_jiang_xian_shu_nan/c001/1.html";
            var img = ChromeDriver.FindElementByXPath("//img[@class='reader-main-img']");
            var imgList = new List<string>();
            var src = img.GetAttribute("src");
            imgList.Add(src);

            var nextButton = ChromeDriver.FindElementByXPath("//a[@data-page][text()='>']");
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
                        var currentNext = ChromeDriver.FindElementByXPath("//a[@data-page][text()='>']");
                        if (currentNext.GetAttribute("data-page") != currentDatapage)
                        {
                            nextButton = currentNext;
                            return true;
                        }
                        return false;
                    }
                    catch (NoSuchElementException ex)
                    {
                        nextButton = null;
                        return true;
                    }
                });

            }
            while (nextButton != null && nextButton.Displayed);

        }
    }
}
