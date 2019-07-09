using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Plugins;
using MangaRipper.Plugin.MangaStream;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MangaRipper.Test.Plugins
{
    public class MangaStreamTests
    {
        CancellationTokenSource source;
        readonly ILogger logger;
        HttpDownloader downloader;
        private readonly MangaStream service;

        public MangaStreamTests()
        {
            source = new CancellationTokenSource();
            logger = new Mock<ILogger>().Object;
            downloader = new HttpDownloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new MangaStream(logger, downloader, new XpathSelector());
        }

        [Fact]
        public async Task FindChapters()
        {
            string url = "https://readms.net/manga/dragon_ball_super";
            Assert.True(service.Of(url));
            var chapters = await service.GetChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.Equal("Dragon Ball Super 001 - The God of Destruction's Prophetic Dream", chapter.Name);
            Assert.Equal("https://readms.net/r/dragon_ball_super/001/2831/1", chapter.Url);
           
        }

        [Fact]
        public async Task FindImages()
        {
            var images = await service.GetImages("https://readms.net/r/dragon_ball_super/001/2831/1", new Progress<int>(), source.Token);
            Assert.Equal(17, images.Count());
            Assert.StartsWith("https://img.mangastream.com/cdn/manga/107/2831/001.jpg", images.ToArray()[0]);
            Assert.StartsWith("https://img.mangastream.com/cdn/manga/107/2831/001a.jpg", images.ToArray()[1]);
            Assert.StartsWith("https://img.mangastream.com/cdn/manga/107/2831/002.png", images.ToArray()[2]);

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }
    }
}
