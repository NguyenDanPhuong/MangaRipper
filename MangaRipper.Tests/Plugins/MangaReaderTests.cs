using MangaRipper.ChromeDriver;
using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Logging;
using MangaRipper.Core.Plugins;
using MangaRipper.Plugin.MangaReader;
using Moq;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MangaRipper.Test.Plugins
{
    public class MangaReaderTests:IDisposable
    {
        readonly CancellationTokenSource source;
        readonly ILogger<MangaReader> logger;

        public OpenQA.Selenium.Chrome. ChromeDriver ChromeDriver { get; }

        readonly IHttpDownloader downloader;
        private readonly MangaReader service;

        public MangaReaderTests()
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
            source = new CancellationTokenSource();
            logger = new Mock<ILogger<MangaReader>>().Object;
            downloader = new HttpDownloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new MangaReader(logger, downloader, ChromeDriver);
        }

        [Fact]
        public async Task FindChapters()
        {
            string url = "https://www.mangareader.net/naruto";
            Assert.True(service.Of(url));
            // Test service can find chapters
            var chapters = await service.GetChapters(url, new Progress<string>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            // Test chapters are in correct order.
            var chapter = chapters.Last();
            Assert.Equal("Naruto 1", chapter.Name);
            Assert.Equal("https://www.mangareader.net/naruto/1", chapter.Url);
            // Test there're no duplicated chapters.
            var anyDuplicated = chapters.GroupBy(x => x.Url).Any(g => g.Count() > 1);
            Assert.False(anyDuplicated, "There're duplicated chapters.");
        }

        [Fact]
        public async Task FindImages()
        {
            var images = await service.GetImages("https://www.mangareader.net/naruto/1", new Progress<string>(), source.Token);
            Assert.Equal(53, images.Count());
            Assert.Equal("https://i9.imggur.net/naruto/1/naruto-1564773.jpg", images.ToArray()[0]);
            Assert.Equal("https://i7.imggur.net/naruto/1/naruto-1564774.jpg", images.ToArray()[1]);
            Assert.Equal("https://i1.imggur.net/naruto/1/naruto-1564825.jpg", images.ToArray()[52]);

            string imageString = await downloader.GetStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }

        public void Dispose()
        {
            ChromeDriver.Quit();
        }
    }
}
