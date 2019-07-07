using MangaRipper.Core;
using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Interfaces;
using MangaRipper.Plugin.MangaReader;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MangaRipper.Test.Plugins
{
    public class MangaReaderTests
    {
        CancellationTokenSource source;
        readonly ILogger logger;
        Downloader downloader;
        private readonly MangaReader service;

        public MangaReaderTests()
        {
            source = new CancellationTokenSource();
            logger = new Mock<ILogger>().Object;
            downloader = new Downloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new MangaReader(logger, downloader, new HtmlAtilityPackAdapter());
        }

        [Fact]
        public async Task FindChapters()
        {
            string url = "https://www.mangareader.net/naruto";
            Assert.True(service.Of(url));
            // Test service can find chapters
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            // Test chapters are in correct order.
            var chapter = chapters.Last();
            Assert.Equal("Naruto", chapter.Manga);
            Assert.Equal("Naruto 1", chapter.Name);
            Assert.Equal("https://www.mangareader.net/naruto/1", chapter.Url);
            // Test there're no duplicated chapters.
            var anyDuplicated = chapters.GroupBy(x => x.Url).Any(g => g.Count() > 1);
            Assert.False(anyDuplicated, "There're duplicated chapters.");
        }

        [Fact]
        public async Task FindImages()
        {
            var images = await service.FindImages("https://www.mangareader.net/naruto/1", new Progress<int>(), source.Token);
            Assert.Equal(53, images.Count());
            Assert.Equal("https://i10.mangareader.net/naruto/1/naruto-1564773.jpg", images.ToArray()[0]);
            Assert.Equal("https://i4.mangareader.net/naruto/1/naruto-1564774.jpg", images.ToArray()[1]);
            Assert.Equal("https://i1.mangareader.net/naruto/1/naruto-1564825.jpg", images.ToArray()[52]);

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }
    }
}
