using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using MangaRipper.Core.Interfaces;
using MangaRipper.Plugin.MangaStream;
using MangaRipper.Plugin.KissManga;
using MangaRipper.Plugin.MangaHere;
using MangaRipper.Plugin.MangaFox;
using MangaRipper.Plugin.MangaReader;
using MangaRipper.Plugin.NHentai;
using Moq;
using MangaRipper.Core;
using MangaRipper.Core.FilenameDetectors;
using Xunit;

namespace MangaRipper.Test
{
    public class Plugin
    {
        CancellationTokenSource source;
        ILogger logger;
        Downloader downloader;

        public Plugin()
        {
            source = new CancellationTokenSource();
            logger = new Mock<ILogger>().Object;
            downloader = new Downloader(new FilenameDetector(new GoogleProxyFilenameDetector()));
        }

        [Fact]
        public async Task MangaReader_Test()
        {
            string url = "https://www.mangareader.net/naruto";
            var service = new MangaReader(logger, downloader, new HtmlAtilityPackAdapter());
            Assert.True(service.Of(url));
            // Test service can find chapters
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            // Test chapters are in correct order.
            var chapter = chapters.Last();
            Assert.Equal("Naruto", chapter.Manga);
            Assert.Equal("Naruto 1", chapter.DisplayName);
            Assert.Equal("https://www.mangareader.net/naruto/1", chapter.Url);
            // Test there're no duplicated chapters.
            var anyDuplicated = chapters.GroupBy(x => x.Url).Any(g => g.Count() > 1);
            Assert.False(anyDuplicated, "There're duplicated chapters.");
            // Test service can find images.
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(53, images.Count());
            Assert.Equal("https://i10.mangareader.net/naruto/1/naruto-1564773.jpg", images.ToArray()[0]);
            Assert.Equal("https://i4.mangareader.net/naruto/1/naruto-1564774.jpg", images.ToArray()[1]);
            Assert.Equal("https://i1.mangareader.net/naruto/1/naruto-1564825.jpg", images.ToArray()[52]);

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }

        [Fact]
        public async Task MangaFox_Test()
        {
            // Test with unlicensed manga. Appveyor CI is US based and cannot access licensed manga in the US. 
            // If we test with a licensed manga, this test will failed.
            string url = "https://fanfox.net/manga/tian_jiang_xian_shu_nan/";
            var service = new MangaFox(logger, downloader, new HtmlAtilityPackAdapter(), new Retry());
            Assert.True(service.Of(url));
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.Equal("Tian Jiang Xian Shu Nan Manga", chapter.Manga);
            Assert.Equal("Tian Jiang Xian Shu Nan 1", chapter.DisplayName);
            Assert.Equal("https://fanfox.net/manga/tian_jiang_xian_shu_nan/c001/1.html", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(15, images.Count());
            Assert.True(images.ToArray()[0].StartsWith("https://a.fanfox.net/store/manga/19803/001.0/compressed/q001.jpg"));
            Assert.True(images.ToArray()[1].StartsWith("https://a.fanfox.net/store/manga/19803/001.0/compressed/q002.jpg"));
            Assert.True(images.ToArray()[2].StartsWith("https://a.fanfox.net/store/manga/19803/001.0/compressed/q003.jpg"));

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }

        [Fact]
        public async Task MangaHere_Test()
        {
            string url = "https://www.mangahere.cc/manga/deathtopia/";
            var service = new MangaHere(logger, downloader, new HtmlAtilityPackAdapter(), new Retry());
            Assert.True(service.Of(url));
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.Equal(66, chapters.Count());
            var chapter = chapters.Last();
            Assert.Equal("Deathtopia", chapter.Manga);
            Assert.Equal("Deathtopia 1", chapter.DisplayName);
            Assert.Equal("https://www.mangahere.cc/manga/deathtopia/c001/", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(59, images.Count());
            Assert.True(images.ToArray()[0].StartsWith("https://mangatown.secure.footprint.net/store/manga/14771/001.0/compressed/uimg001.jpg"));
            Assert.True(images.ToArray()[1].StartsWith("https://mangatown.secure.footprint.net/store/manga/14771/001.0/compressed/uimg002.jpg"));
            Assert.True(images.ToArray()[58].StartsWith("https://mangatown.secure.footprint.net/store/manga/14771/001.0/compressed/uimg059.jpg"));
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }

        [Fact]
        public async Task MangaStream_Test()
        {
            string url = "https://readms.net/manga/dragon_ball_super";
            var service = new MangaStream(logger, downloader, new HtmlAtilityPackAdapter());
            Assert.True(service.Of(url));
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.Equal("Dragon Ball Super", chapter.Manga);
            Assert.Equal("001 - The God of Destruction's Prophetic Dream", chapter.DisplayName);
            Assert.Equal("https://readms.net/r/dragon_ball_super/001/2831/1", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(17, images.Count());
            Assert.True(images.ToArray()[0].StartsWith("https://img.mangastream.com/cdn/manga/107/2831/001.jpg"));
            Assert.True(images.ToArray()[1].StartsWith("https://img.mangastream.com/cdn/manga/107/2831/001a.jpg"));
            Assert.True(images.ToArray()[2].StartsWith("https://img.mangastream.com/cdn/manga/107/2831/002.png"));

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }

        [Fact]
#if !DEBUG
        [Ignore]
#endif
        public async Task KissManga_Test()
        {
            string url = "https://kissmanga.com/Manga/Onepunch-Man";
            var service = new KissManga(logger, downloader, new HtmlAtilityPackAdapter(), new JurassicScriptEngine());
            Assert.True(service.Of(url));
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.Equal("Onepunch-Man", chapter.Manga);
            Assert.Equal("Onepunch-Man 001", chapter.DisplayName);
            Assert.Equal("https://kissmanga.com/Manga/Onepunch-Man/Punch-001?id=369844", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(28, images.Count());
            Assert.True(images.ToArray()[0].StartsWith("https://2.bp.blogspot.com/-daAIY2sJQcE/V8rt280634I/AAAAAAAA404/Ld1A6XZGrvcKioYmulO4MG8RcbPJf8zagCHM/s16000/0001-001.png"));
            Assert.True(images.ToArray()[1].StartsWith("https://2.bp.blogspot.com/-cx66pnwxYF4/V8rt3BUIFuI/AAAAAAAA408/C9nPR0AhT-oiTLiUzrKoo_K4JpGhv8OHACHM/s16000/0001-002.png"));
            Assert.True(images.ToArray()[2].StartsWith("https://2.bp.blogspot.com/-EfldQUNYKe8/V8rt3cmh-nI/AAAAAAAA41A/_O27IwHy_FkjCy8epn_zhccCy-6KRyCTwCHM/s16000/0001-003.png"));

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }

        [Fact]
        public async Task NHentai_Test()
        {
            string url = "https://nhentai.net/g/247893/";
            var service = new NHentai(logger, downloader, new HtmlAtilityPackAdapter(), new Retry());
            Assert.True(service.Of(url));
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.True(chapters.Any(), "Cannot find chapters.");
            // Well, it's one chapter per url
            var chapter = chapters.Last();
            Assert.Equal("[Korotsuke] Koopa Hime | Bowsette (New Super Mario Bros. U Deluxe) [English] {darknight}", chapter.Manga);
            Assert.Equal("[Korotsuke] Koopa Hime | Bowsette (New Super Mario Bros. U Deluxe) [English] {darknight}", chapter.DisplayName);
            Assert.Equal("https://nhentai.net/g/247893/", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), source.Token);
            Assert.Equal(5, images.Count());
            Assert.True(images.ToArray()[0].StartsWith("https://i.nhentai.net/galleries/1291586/1.jpg"));
            Assert.True(images.ToArray()[1].StartsWith("https://i.nhentai.net/galleries/1291586/2.jpg"));
            Assert.True(images.ToArray()[2].StartsWith("https://i.nhentai.net/galleries/1291586/3.jpg"));

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], source.Token);
            Assert.NotNull(imageString);
        }
    }
}
