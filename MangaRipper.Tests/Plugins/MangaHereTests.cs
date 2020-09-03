using MangaRipper.ChromeDriver;
using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Logging;
using MangaRipper.Core.Plugins;
using MangaRipper.Plugin.MangaHere;
using Moq;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace MangaRipper.Test.Plugins
{

    public class MangaHereTests : IDisposable
    {
        readonly OpenQA.Selenium.Chrome.ChromeDriver ChromeDriver;
        readonly Mock<ILogger<MangaHere>> logger = new Mock<ILogger<MangaHere>>();
        readonly IHttpDownloader downloader;
        readonly MangaHere service;
        private readonly CancellationTokenSource source;

        public MangaHereTests()
        {
            var updater = new ChromeDriverUpdater(".\\");
            updater.ExecuteAsync().Wait();
            source = new CancellationTokenSource();

            var options = new ChromeOptions();
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--start-maximized");
            options.AddArgument("--headless");
            ChromeDriver = new OpenQA.Selenium.Chrome.ChromeDriver(options);


            downloader = new HttpDownloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new MangaHere(logger.Object, downloader, new Retry(), ChromeDriver);
        }

        public void Dispose()
        {
            ChromeDriver.Quit();
        }

#if DEBUG
        [Fact]
#endif
        public async void FindChapters()
        {
            string url = "https://www.mangahere.cc/manga/deathtopia/";
            Assert.True(service.Of(url));
            var chapters = await service.GetChapters(url, new Progress<string>(), source.Token);
            Assert.Equal(66, chapters.Count());
            var chapter = chapters.Last();
            Assert.Equal("Deathtopia Ch.001 - Those People", chapter.Name);
            Assert.Equal("https://www.mangahere.cc/manga/deathtopia/c001/1.html", chapter.Url);
        }


#if DEBUG
        [Fact]
#endif
        public async void FindImages()
        {
            var images = await service.GetImages("https://www.mangahere.cc/manga/deathtopia/c001/1.html", new Progress<string>(), source.Token);
            Assert.Equal(60, images.Count());
            Assert.StartsWith("https://zjcdn.mangahere.org/store/manga/14771/001.0/compressed/uimg001.jpg", images.ToArray()[0]);
            Assert.StartsWith("https://zjcdn.mangahere.org/store/manga/14771/001.0/compressed/uimg002.jpg", images.ToArray()[1]);
            Assert.StartsWith("https://zjcdn.mangahere.org/store/manga/14771/001.0/compressed/uimg059.jpg", images.ToArray()[58]);
            string imageString = await downloader.GetStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }
    }
}
