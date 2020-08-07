using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Logging;
using MangaRipper.Core.Plugins;
using MangaRipper.Plugin.MangaFox;
using MangaRipper.Tools.ChromeDriver;
using Moq;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace MangaRipper.Test.Plugins
{
    /// <summary>
    /// Test with unlicensed manga. Appveyor CI is US based and cannot access licensed manga in the US. 
    /// If we test with a licensed manga, this test will failed.
    /// </summary>
    public class FanfoxTests : IDisposable
    {
        readonly ChromeDriver ChromeDriver;
        readonly Mock<ILogger> logger = new Mock<ILogger>();
        readonly IHttpDownloader downloader;
        readonly MangaFox service;
        private readonly CancellationTokenSource source;

        public FanfoxTests()
        {
            var updater = new ChromeDriverUpdater(".\\");
            updater.ExecuteAsync().Wait();

            source = new CancellationTokenSource();

            var options = new ChromeOptions();
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--start-maximized");
            options.AddArgument("--headless");
            ChromeDriver = new ChromeDriver(options);


            downloader = new HttpDownloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new MangaFox(logger.Object, downloader, new Retry(), ChromeDriver);
        }

        public void Dispose()
        {
            ChromeDriver.Close();
            ChromeDriver.Dispose();
        }

#if DEBUG
        [Fact]
#endif
        public async void FindChapters()
        {
            string url = "https://fanfox.net/manga/tian_jiang_xian_shu_nan/";
            Assert.True(service.Of(url));
            var chapters = await service.GetChapters(url, new Progress<string>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.Equal("Tian Jiang Xian Shu Nan Ch.001", chapter.Name);
            Assert.Equal("https://fanfox.net/manga/tian_jiang_xian_shu_nan/c001/1.html", chapter.Url);
        }

#if DEBUG
        [Fact]
#endif
        public async void FindImages()
        {
            var images = await service.GetImages("https://fanfox.net/manga/tian_jiang_xian_shu_nan/c001/1.html", new Progress<string>(), source.Token);
            Assert.Equal(16, images.Count());
            Assert.StartsWith("https://zjcdn.mangafox.me/store/manga/19803/001.0/compressed/q001.jpg", images.ToArray()[0]);
            Assert.StartsWith("https://zjcdn.mangafox.me/store/manga/19803/001.0/compressed/q002.jpg", images.ToArray()[1]);
            Assert.StartsWith("https://zjcdn.mangafox.me/store/manga/19803/001.0/compressed/q003.jpg", images.ToArray()[2]);

            string imageString = await downloader.GetStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }
    }
}
