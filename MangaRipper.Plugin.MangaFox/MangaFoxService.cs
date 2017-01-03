using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Plugin.MangaFox
{
    /// <summary>
    /// Support find chapters, images from MangaFox
    /// </summary>
    public class MangaFoxService : IMangaService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        
        public SiteInformation GetInformation()
        {
            return new SiteInformation("MangaFox", "http://mangafox.me", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("mangafox.me");
        }

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new DownloadService();
            var parser = new ParserHelper();

            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a href=\"(?<Value>[^\"]+)\" title=\"(|[^\"]+)\" class=\"tips\">(?<Name>[^<]+)</a>", input, "Name", "Value");
            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new DownloadService();
            var parser = new ParserHelper();

            // find all pages in a chapter
            var input = await downloader.DownloadStringAsync(chapter.Url);
            var pages = parser.Parse(@"<option value=""(?<Value>[^""]+)"" (|selected=""selected"")>\d+</option>", input, "Value");
            // transform pages link
            pages = pages.Select(p =>
            {
                var value = new Uri(new Uri(chapter.Url), (p + ".html")).AbsoluteUri;
                return value;
            }).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(
                pages,
                new Progress<int>((count) =>
                {
                    var f = (float) count / pages.Count();
                    var i = Convert.ToInt32(f * 100);
                    progress.Report(i);
                }),
                cancellationToken);

            var images = parser.Parse("<img src=\"(?<Value>[^\"]+)\"[ ]+width=", pageData, "Value");

            progress.Report(100);
            return images;
        }
    }
}
