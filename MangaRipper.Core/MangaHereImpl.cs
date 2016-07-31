using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    class MangaHereImpl : IManga
    {
        public async Task<IList<Chapter>> FindChapters(string manga, CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new Parser();

            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a class=\"color_0077\" href=\"(?<Value>http://[^\"]+)\"[^<]+>(?<Text>[^<]+)</a>", input, "Name", "Value");
            return chaps;
        }

        public async Task<IList<string>> FindImanges(Chapter chapter, CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new Parser();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Link);
            var pages = parser.Parse(@"<option value=""(?<Value>[^""]+)"" (|selected=""selected"")>\d+</option>", input, "Value");

            // transform pages link
            pages = pages.Select(p => {
                var value = new Uri(new Uri(chapter.Link), p).AbsoluteUri;
                return value;
            }).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(pages, cancellationToken);
            var images = parser.Parse("<img src=\"(?<Value>[^\"]+)\"[ ]+onerror", pageData, "Value");

            return images;
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation("MangaHere", "http://www.mangahere.co/", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangahere.co");
        }
    }
}
