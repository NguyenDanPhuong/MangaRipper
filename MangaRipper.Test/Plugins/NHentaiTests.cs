using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using MangaRipper.Core.Interfaces;
using MangaRipper.Plugin.MangaStream;
using MangaRipper.Plugin.MangaHere;
using MangaRipper.Plugin.MangaReader;
using MangaRipper.Plugin.NHentai;
using Moq;
using MangaRipper.Core;
using MangaRipper.Core.FilenameDetectors;
using Xunit;
using MangaRipper.Core.Models;

namespace MangaRipper.Test.Plugins
{
    public class NHentaiTests
    {
        CancellationTokenSource source;
        readonly ILogger logger;
        Downloader downloader;
        private readonly NHentai service;

        public NHentaiTests()
        {
            source = new CancellationTokenSource();
            logger = new Mock<ILogger>().Object;
            downloader = new Downloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
            service = new NHentai(logger, downloader, new HtmlAtilityPackAdapter(), new Retry());
        }


        [Fact]
        public async Task FindChapters()
        {
            string url = "https://nhentai.net/g/247893/";
            Assert.True(service.Of(url));
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            // Well, it's one chapter per url
            var chapter = chapters.Last();
            Assert.Equal("[Korotsuke] Koopa Hime | Bowsette (New Super Mario Bros. U Deluxe) [English] {darknight}", chapter.Manga);
            Assert.Equal("[Korotsuke] Koopa Hime | Bowsette (New Super Mario Bros. U Deluxe) [English] {darknight}", chapter.DisplayName);
            Assert.Equal("https://nhentai.net/g/247893/", chapter.Url);
        }

        [Fact]
        public async Task FindImages()
        {
            var chapter = new Chapter("[Korotsuke] Koopa Hime | Bowsette (New Super Mario Bros. U Deluxe) [English] {darknight}", "https://nhentai.net/g/247893/")
            {
                Manga = "[Korotsuke] Koopa Hime | Bowsette (New Super Mario Bros. U Deluxe) [English] {darknight}"
            };
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(5, images.Count());
            Assert.StartsWith("https://i.nhentai.net/galleries/1291586/1.jpg", images.ToArray()[0]);
            Assert.StartsWith("https://i.nhentai.net/galleries/1291586/2.jpg", images.ToArray()[1]);
            Assert.StartsWith("https://i.nhentai.net/galleries/1291586/3.jpg", images.ToArray()[2]);

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }
    }
}
