using MangaRipper.Core;
using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Plugin.MangaReader;
using MangaRipper.Plugin.MangaStream;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MangaRipper.Test.Plugins
{
    public class MangaStreamTests
    {
        CancellationTokenSource source;
        readonly ILogger logger;
        Downloader downloader;
        private readonly MangaStream service;

        public MangaStreamTests()
        {
            source = new CancellationTokenSource();
            logger = new Mock<ILogger>().Object;
            downloader = new Downloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new MangaStream(logger, downloader, new HtmlAtilityPackAdapter());
        }

        [Fact]
        public async Task FindChapters()
        {
            string url = "https://readms.net/manga/dragon_ball_super";
            Assert.True(service.Of(url));
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.Equal("Dragon Ball Super", chapter.Manga);
            Assert.Equal("001 - The God of Destruction's Prophetic Dream", chapter.DisplayName);
            Assert.Equal("https://readms.net/r/dragon_ball_super/001/2831/1", chapter.Url);
           
        }

        [Fact]
        public async Task FindImages()
        {
            var chapter = new Chapter("001 - The God of Destruction's Prophetic Dream", "https://readms.net/r/dragon_ball_super/001/2831/1")
            {
                Manga = "Dragon Ball Super"
            };
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(17, images.Count());
            Assert.StartsWith("https://img.mangastream.com/cdn/manga/107/2831/001.jpg", images.ToArray()[0]);
            Assert.StartsWith("https://img.mangastream.com/cdn/manga/107/2831/001a.jpg", images.ToArray()[1]);
            Assert.StartsWith("https://img.mangastream.com/cdn/manga/107/2831/002.png", images.ToArray()[2]);

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }
    }
}
