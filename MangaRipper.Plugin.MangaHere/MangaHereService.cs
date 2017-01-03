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

namespace MangaRipper.Plugin.MangaHere
{

    /// <summary>
    /// Support find chapters and images from MangaHere
    /// </summary>
    public class MangaHereService : IMangaService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a class=\"color_0077\" href=\"(?<Value>http://[^\"]+)\"[^<]+>(?<Name>[^<]+)</a>", input, "Name", "Value");
            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url);
            var pages = parser.Parse(@"<option value=""(?<Value>[^""]+)"" (|selected=""selected"")>\d+</option>", input, "Value");

            // transform pages link
            pages = pages.Select(p =>
            {
                var value = new Uri(new Uri(chapter.Url), p).AbsoluteUri;
                return value;
            }).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(pages, new Progress<int>((count) =>
            {
                var f = (float)count / pages.Count();
                int i = Convert.ToInt32(f * 100);
                progress.Report(i);
            }), cancellationToken);
            var images = parser.Parse("<img src=\"(?<Value>[^\"]+)\" onload=", pageData, "Value");

            return images;
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation("MangaHere", "http://www.mangahere.co", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangahere.co");
        }
    }
}
