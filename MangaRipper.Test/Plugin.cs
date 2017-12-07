using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using MangaRipper.Core.Services;
using MangaRipper.Core.Providers;
using System.Collections.Generic;

namespace MangaRipper.Test
{
    [TestClass]
    public class Plugin
    {
        CancellationTokenSource _source;

        [TestInitialize]
        public void Initialize()
        {
            Framework.Init(Environment.CurrentDirectory, Path.Combine(Environment.CurrentDirectory, "MangaRipper.Configuration.json"));
            _source = new CancellationTokenSource();
        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        public async Task MangaReader_Test()
        {
            string url = "http://www.mangareader.net/naruto";
            var service = Framework.GetService(url);
            // Test service can find chapters
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            // Test chapters are in correct order.
            var chapter = chapters.Last();
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

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaFox_Test()
        {
            // Test with unlicensed manga. Appveyor CI is US based and cannot access licensed manga in the US. 
            // If we test with a licensed manga, this test will failed.
            string url = "http://mangafox.la/manga/tian_jiang_xian_shu_nan/";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Tian Jiang Xian Shu Nan 1", chapter.Name);
            Assert.AreEqual("http://mangafox.la/manga/tian_jiang_xian_shu_nan/c001/1.html", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(15, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("https://lmfcdn.secure.footprint.net/store/manga/19803/001.0/compressed/q001.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("https://lmfcdn.secure.footprint.net/store/manga/19803/001.0/compressed/q002.jpg"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("https://lmfcdn.secure.footprint.net/store/manga/19803/001.0/compressed/q003.jpg"));

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaHere_Test()
        {
            string url = "http://www.mangahere.co/manga/the_god_of_high_school/";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("The God Of High School 1", chapter.Name);
            Assert.AreEqual("http://www.mangahere.co/manga/the_god_of_high_school/c001/", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(55, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.01.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.02.jpg"));
            Assert.IsTrue(images.ToArray()[54].StartsWith("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.55.jpg"));

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        #region Batoto

        [TestMethod]
        public async Task Batoto_Test()
        {
            string url = "http://bato.to/comic/_/comics/21st-century-boys-r1591";
            var service = Framework.GetService(url);

            service.Configuration(new[] {
                new KeyValuePair<string, object>("Username", "gufrohepra"),
                new KeyValuePair<string, object>("Password", "123"),
                new KeyValuePair<string, object>("Languages", "")
            });

            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Vol.01 Ch.01 Read Online", chapter.Name);
            Assert.AreEqual("https://bato.to/reader#900d11d96d1466f2", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(31, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("http://img.bato.to/comics/2014/10/08/2/read54357eb5e1ca9/img000001.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("http://img.bato.to/comics/2014/10/08/2/read54357eb5e1ca9/img000002.jpg"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("http://img.bato.to/comics/2014/10/08/2/read54357eb5e1ca9/img000003.jpg"));

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }
        
        [TestMethod]
        public async Task Batoto_AllLanguages_Test()
        {
            string url = "https://bato.to/comic/_/comics/bitter-virgin-r110";
            var service = Framework.GetService(url);

            service.Configuration(new[] {
                new KeyValuePair<string, object>("Username", "gufrohepra"),
                new KeyValuePair<string, object>("Password", "123"),
                new KeyValuePair<string, object>("Languages", "")
            });

            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.AreEqual(chapters.Count(), 82);
        }

        [TestMethod]
        public async Task Batoto_EnglishLanguages_Test()
        {
            string url = "https://bato.to/comic/_/comics/bitter-virgin-r110";
            var service = Framework.GetService(url);

            service.Configuration(new[] {
                new KeyValuePair<string, object>("Username", "gufrohepra"),
                new KeyValuePair<string, object>("Password", "123"),
                new KeyValuePair<string, object>("Languages", "English")
            });

            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.AreEqual(chapters.Count(), 32);
        }

        [TestMethod]
        public async Task Batoto_GermanEnglishLanguages_Test()
        {
            string url = "https://bato.to/comic/_/comics/bitter-virgin-r110";
            var service = Framework.GetService(url);

            service.Configuration(new[] {
                new KeyValuePair<string, object>("Username", "gufrohepra"),
                new KeyValuePair<string, object>("Password", "123"),
                new KeyValuePair<string, object>("Languages", "German, English")
            });

            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.AreEqual(chapters.Count(), 64);
        }

        #endregion

        [TestMethod]
        public async Task MangaStream_Test()
        {
            string url = "http://mangastream.com/manga/dragon_ball_super";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("001 - The God of Destruction's Prophetic Dream", chapter.Name);
            Assert.AreEqual("http://readms.net/r/dragon_ball_super/001/2831/1", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(17, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("http://img.readms.net/cdn/manga/107/2831/001.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("http://img.readms.net/cdn/manga/107/2831/001a.jpg"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("http://img.readms.net/cdn/manga/107/2831/002.png"));

            var downloader = new Downloader();
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
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), _source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Onepunch-Man _vol.001 ch.001", chapter.Name);
            Assert.AreEqual("http://kissmanga.com/Manga/Onepunch-Man/vol-001-ch-001?id=313725", chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(), _source.Token);
            Assert.AreEqual(19, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http%3a%2f%2f2.p.mpcdn.net%2f50%2f531513%2f1.jpg&imgmax=30000"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http%3a%2f%2f2.p.mpcdn.net%2f50%2f531513%2f2.jpg&imgmax=30000"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http%3a%2f%2f2.p.mpcdn.net%2f50%2f531513%2f3.jpg&imgmax=30000"));

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0], _source.Token);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }
    }
}
