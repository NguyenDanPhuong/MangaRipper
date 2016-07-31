using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    class MangaReaderImpl : IManga
    {
        public async Task<IList<Chapter>> FindChapters(string manga)
        {
            var downloader = new Downloader();
            var parser = new Parser();

            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a href=\"(?<Value>[^\"]+)\">(?<Name>[^<]+)</a> :", input, "Name", "Value");

            // transform pages link
            chaps = chaps.Select(c => {
                return new Chapter(c.Name, new Uri(new Uri(manga), c.Link).AbsoluteUri);
            }).ToList();

            return chaps;
        }

        public async Task<IList<string>> FindImanges(Chapter chapter)
        {
            var downloader = new Downloader();
            var parser = new Parser();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Link);
            var pages = parser.Parse(@"<option value=""(?<Value>[^""]+)""(| selected=""selected"")>\d+</option>", input, "Value");

            // transform pages link
            pages = pages.Select(p => {
                var value = new Uri(new Uri(chapter.Link), p).AbsoluteUri;
                return value;
            }).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(pages);
            var images = parser.Parse(@"<img id=""img"" width=""\d+"" height=""\d+"" src=""(?<Value>[^""]+)""", pageData, "Value");

            return images;
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation("MangaReader", "http://www.mangareader.net", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangareader.net");
        }
    }
}
