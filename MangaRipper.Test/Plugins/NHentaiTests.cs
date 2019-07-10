using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using MangaRipper.Plugin.NHentai;
using Moq;
using MangaRipper.Core.FilenameDetectors;
using Xunit;
using MangaRipper.Core.Plugins;
using MangaRipper.Core.Logging;

namespace MangaRipper.Test.Plugins
{
    public class NHentaiTests
    {
        CancellationTokenSource source;
        readonly ILogger logger;
        HttpDownloader downloader;
        private readonly NHentai service;

        public NHentaiTests()
        {
            source = new CancellationTokenSource();
            logger = new Mock<ILogger>().Object;
            downloader = new HttpDownloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new NHentai(logger, downloader, new XPathSelector(), new Retry());
        }


        [Fact]
        public async Task FindChapters()
        {
            string url = "https://nhentai.net/g/247893/";
            Assert.True(service.Of(url));
            var chapters = await service.GetChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            // Well, it's one chapter per url
            var chapter = chapters.Last();
            Assert.Equal("[Korotsuke] Koopa Hime | Bowsette (New Super Mario Bros. U Deluxe) [English] {darknight}", chapter.Name);
            Assert.Equal("https://nhentai.net/g/247893/", chapter.Url);
        }

        [Fact]
        public async Task FindImages()
        {
            var images = await service.GetImages("https://nhentai.net/g/247893/", new Progress<int>(), source.Token);
            Assert.Equal(5, images.Count());
            Assert.StartsWith("https://i.nhentai.net/galleries/1291586/1.jpg", images.ToArray()[0]);
            Assert.StartsWith("https://i.nhentai.net/galleries/1291586/2.jpg", images.ToArray()[1]);
            Assert.StartsWith("https://i.nhentai.net/galleries/1291586/3.jpg", images.ToArray()[2]);

            string imageString = await downloader.GetStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }
    }
}
