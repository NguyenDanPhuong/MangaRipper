using MangaRipper.Core;
using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Plugin.MangaFox;
using MangaRipper.Plugin.MangaHere;
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

namespace MangaRipper.Test.Plugins
{

    public class MangaHereTests : IDisposable
    {
        readonly ChromeDriver ChromeDriver;
        private readonly WebDriverWait Wait;
        readonly Mock<ILogger> logger = new Mock<ILogger>();
        readonly IDownloader downloader;
        readonly MangaHere service;
        private readonly CancellationTokenSource source;

        public MangaHereTests()
        {
            source = new CancellationTokenSource();

            var options = new ChromeOptions();
            //options.AddArgument("--window-size=1920,1080");
            //options.AddArgument("--start-maximized");
            //options.AddArgument("--headless");
            ChromeDriver = new ChromeDriver(options);
            Wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(10));


            downloader = new Downloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new MangaHere(logger.Object, downloader, new HtmlAtilityPackAdapter(), new Retry(), ChromeDriver);
        }

        public void Dispose()
        {
            ChromeDriver.Close();
            ChromeDriver.Dispose();
        }

        [Fact]
        public async void FindChapters()
        {
            string url = "https://www.mangahere.cc/manga/deathtopia/";
            Assert.True(service.Of(url));
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.Equal(66, chapters.Count());
            var chapter = chapters.Last();
            Assert.Equal("Deathtopia", chapter.Manga);
            Assert.Equal("Ch.001 - Those People", chapter.DisplayName);
            Assert.Equal("https://www.mangahere.cc/manga/deathtopia/c001/1.html", chapter.Url);
        }


        [Fact]
        public async void FindImages()
        {
            var chapter = new Chapter("Ch.001 - Those People", "https://www.mangahere.cc/manga/deathtopia/c001/1.html")
            {
                Manga = "Deathtopia"
            };
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(59, images.Count());
            Assert.StartsWith("https://l.mangatown.com/store/manga/14771/001.0/compressed/uimg001.jpg", images.ToArray()[0]);
            Assert.StartsWith("https://l.mangatown.com/store/manga/14771/001.0/compressed/uimg002.jpg", images.ToArray()[1]);
            Assert.StartsWith("https://l.mangatown.com/store/manga/14771/001.0/compressed/uimg059.jpg", images.ToArray()[58]);
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }
    }
}
