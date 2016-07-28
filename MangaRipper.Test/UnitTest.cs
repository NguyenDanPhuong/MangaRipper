using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaRipper.Core;
using System.Threading.Tasks;

namespace MangaRipper.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task TestMangaReader()
        {
            string naruto = "http://www.mangareader.net/naruto";
            ITitle title = TitleFactory.CreateTitle(naruto);
            Assert.IsNotNull(title);

            Assert.IsNull(title.Chapters);

            await title.PopulateChapterAsync(new Core.Progress<int>(percent => {
                Console.WriteLine(percent);
            }));

            Assert.IsTrue(title.Chapters.Count > 0);

            IChapter chap = title.Chapters[0];
        }
    }
}
