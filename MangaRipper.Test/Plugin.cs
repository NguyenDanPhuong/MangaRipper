using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using MangaRipper.Core.Services;
using System.Collections.Generic;
using MangaRipper.Plugin.MangaStream;
using MangaRipper.Plugin.KissManga;
using MangaRipper.Plugin.MangaHere;
using MangaRipper.Plugin.MangaFox;
using MangaRipper.Plugin.MangaReader;
using MangaRipper.Plugin.Batoto;
using Moq;
using MangaRipper.Core;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;

namespace MangaRipper.Test
{
    [TestClass]
    public class Plugin
    {
        CancellationTokenSource _source;
        ILogger _logger;
        Downloader downloader;

        [TestInitialize]
        public void Initialize()
        {
            _source = new CancellationTokenSource();
            _logger = new Mock<ILogger>().Object;
            downloader = new Downloader(_logger);
        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        public async Task MangaReader_Test()
        {
            string url = "http://www.mangareader.net/naruto";
            var service = new MangaReader(_logger, new Downloader(_logger), new HtmlAtilityPackAdapter());
            // Test service can find chapters
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            // Test chapters are in correct order.
            var chapter = chapters.Last();
            Assert.AreEqual("Naruto", chapter.Manga);
            Assert.AreEqual("Naruto 1", chapter.Name);
            Assert.AreEqual("http://www.mangareader.net/naruto/1", chapter.Url);
            // Test there're no duplicated chapters.
            var anyDuplicated = chapters.GroupBy(x => x.Url).Any(g => g.Count() > 1);
            Assert.IsFalse(anyDuplicated, "There're duplicated chapters.");
            // Test service can find images.
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(53, images.Count());
            Assert.AreEqual("http://i10.mangareader.net/naruto/1/naruto-1564773.jpg", images.ToArray()[0]);
            Assert.AreEqual("http://i4.mangareader.net/naruto/1/naruto-1564774.jpg", images.ToArray()[1]);
            Assert.AreEqual("http://i1.mangareader.net/naruto/1/naruto-1564825.jpg", images.ToArray()[52]);

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaFox_Test()
        {
            // Test with unlicensed manga. Appveyor CI is US based and cannot access licensed manga in the US. 
            // If we test with a licensed manga, this test will failed.
            string url = "http://mangafox.la/manga/tian_jiang_xian_shu_nan/";
            var service = new MangaFox(_logger, new Downloader(_logger), new HtmlAtilityPackAdapter());
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Tian Jiang Xian Shu Nan Manga", chapter.Manga);
            Assert.AreEqual("Tian Jiang Xian Shu Nan 1", chapter.Name);
            Assert.AreEqual("http://mangafox.la/manga/tian_jiang_xian_shu_nan/c001/1.html", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(15, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("https://lmfcdn.secure.footprint.net/store/manga/19803/001.0/compressed/q001.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("https://lmfcdn.secure.footprint.net/store/manga/19803/001.0/compressed/q002.jpg"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("https://lmfcdn.secure.footprint.net/store/manga/19803/001.0/compressed/q003.jpg"));

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaHere_Test()
        {
            string url = "http://www.mangahere.cc/manga/the_god_of_high_school/";
            var service = new MangaHere(_logger, new Downloader(_logger), new HtmlAtilityPackAdapter());
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("The God Of High School", chapter.Manga);
            Assert.AreEqual("The God Of High School 1", chapter.Name);
            Assert.AreEqual("http://www.mangahere.cc/manga/the_god_of_high_school/c001/", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(56, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("https://mhcdn.secure.footprint.net/store/manga/9275/001.0/compressed/m001.01.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("https://mhcdn.secure.footprint.net/store/manga/9275/001.0/compressed/m001.02.jpg"));
            Assert.IsTrue(images.ToArray()[54].StartsWith("https://mhcdn.secure.footprint.net/store/manga/9275/001.0/compressed/m001.55.jpg"));

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        #region Batoto

        [TestMethod]
        public async Task Batoto_Test()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.FindConfigByPrefix(It.IsAny<string>())).Returns(new[] {
                new KeyValuePair<string, object>("Plugin.Batoto.Username", "gufrohepra"),
                new KeyValuePair<string, object>("Plugin.Batoto.Password", "123"),
                new KeyValuePair<string, object>("Plugin.Batoto.Languages", "") });

            string url = "http://bato.to/comic/_/comics/21st-century-boys-r1591";
            var service = new Batoto(configMock.Object, _logger, new Downloader(_logger), new HtmlAtilityPackAdapter());

            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("21st Century Boys", chapter.Manga);
            Assert.AreEqual("Vol.01 Ch.01 Read Online", chapter.Name);
            Assert.AreEqual("https://bato.to/reader#900d11d96d1466f2", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(31, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("http://img.bato.to/comics/2014/10/08/2/read54357eb5e1ca9/img000001.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("http://img.bato.to/comics/2014/10/08/2/read54357eb5e1ca9/img000002.jpg"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("http://img.bato.to/comics/2014/10/08/2/read54357eb5e1ca9/img000003.jpg"));

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task Batoto_AllLanguages_Test()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.FindConfigByPrefix(It.IsAny<string>())).Returns(new[] {
                new KeyValuePair<string, object>("Plugin.Batoto.Username", "gufrohepra"),
                new KeyValuePair<string, object>("Plugin.Batoto.Password", "123"),
                new KeyValuePair<string, object>("Plugin.Batoto.Languages", "") });

            string url = "https://bato.to/comic/_/comics/bitter-virgin-r110";
            var service = new Batoto(configMock.Object, _logger, new Downloader(_logger), new HtmlAtilityPackAdapter());

            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.AreEqual(chapters.Count(), 82);
        }

        [TestMethod]
        public async Task Batoto_EnglishLanguages_Test()
        {

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.FindConfigByPrefix(It.IsAny<string>())).Returns(new[] {
                new KeyValuePair<string, object>("Plugin.Batoto.Username", "gufrohepra"),
                new KeyValuePair<string, object>("Plugin.Batoto.Password", "123"),
                new KeyValuePair<string, object>("Plugin.Batoto.Languages", "English") });
            string url = "https://bato.to/comic/_/comics/bitter-virgin-r110";
            var service = new Batoto(configMock.Object, _logger, new Downloader(_logger), new HtmlAtilityPackAdapter());

            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.AreEqual(32, chapters.Count());
        }

        [TestMethod]
        public async Task Batoto_GermanEnglishLanguages_Test()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.FindConfigByPrefix(It.IsAny<string>())).Returns(new[] {
                new KeyValuePair<string, object>("Plugin.Batoto.Username", "gufrohepra"),
                new KeyValuePair<string, object>("Plugin.Batoto.Password", "123"),
                new KeyValuePair<string, object>("Plugin.Batoto.Languages", "German, English") });
            string url = "https://bato.to/comic/_/comics/bitter-virgin-r110";
            var service = new Batoto(configMock.Object, _logger, new Downloader(_logger), new HtmlAtilityPackAdapter());

            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.AreEqual(32 + 32, chapters.Count());
        }

        #endregion

        [TestMethod]
        public async Task MangaStream_Test()
        {
            string url = "https://readms.net/manga/dragon_ball_super";
            var service = new MangaStream(_logger, new Downloader(_logger), new HtmlAtilityPackAdapter());
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Dragon Ball Super", chapter.Manga);
            Assert.AreEqual("001 - The God of Destruction's Prophetic Dream", chapter.Name);
            Assert.AreEqual("https://readms.net/r/dragon_ball_super/001/2831/1", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(17, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("https://img.mangastream.com/cdn/manga/107/2831/001.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("https://img.mangastream.com/cdn/manga/107/2831/001a.jpg"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("https://img.mangastream.com/cdn/manga/107/2831/002.png"));

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
#if !DEBUG
        [Ignore]
#endif
        public async Task KissManga_Test()
        {
            string url = "http://kissmanga.com/Manga/Onepunch-Man";
            var service = new KissManga(_logger, new Downloader(_logger), new HtmlAtilityPackAdapter(), new JurassicScriptEngine());
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Onepunch-Man", chapter.Manga);
            Assert.AreEqual("Onepunch-Man 001", chapter.Name);
            Assert.AreEqual("http://kissmanga.com/Manga/Onepunch-Man/Punch-001?id=369844", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(28, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http%3a%2f%2f2.bp.blogspot.com%2f-daAIY2sJQcE%2fV8rt280634I%2fAAAAAAAA404%2fLd1A6XZGrvcKioYmulO4MG8RcbPJf8zagCHM%2fs16000%2f0001-001.png&imgmax=30000"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http%3a%2f%2f2.bp.blogspot.com%2f-cx66pnwxYF4%2fV8rt3BUIFuI%2fAAAAAAAA408%2fC9nPR0AhT-oiTLiUzrKoo_K4JpGhv8OHACHM%2fs16000%2f0001-002.png&imgmax=30000"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http%3a%2f%2f2.bp.blogspot.com%2f-EfldQUNYKe8%2fV8rt3cmh-nI%2fAAAAAAAA41A%2f_O27IwHy_FkjCy8epn_zhccCy-6KRyCTwCHM%2fs16000%2f0001-003.png&imgmax=30000"));

            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }
    }
}
