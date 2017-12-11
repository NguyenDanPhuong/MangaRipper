using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
using MangaRipper.Core;

namespace MangaRipper.Plugin.MangaStream
{
    /// <summary>
    /// Support find chapters, images from MangaStream
    /// </summary>
    public class MangaStream : IMangaService
    {
        private static IMyLogger logger;
        public MangaStream(IMyLogger myLogger)
        {
            logger = myLogger;
        }
        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress,
            CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new ParserHelper();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            string regEx = "<td><a href=\"(?<Value>[^\"]+)\">(?<Name>[^<]+)</a>";
            var chaps = parser.ParseGroup(regEx, input, "Name", "Value");
            chaps = chaps.Select(c => new Chapter(c.OriginalName, $"https://readms.net{c.Url}"));
            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress,
            CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new ParserHelper();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url, cancellationToken);
            string regExPages =
                "<li><a href=\"(?<Value>/r[^\"]+)\">[^<]+</a>";
            var pages = parser.Parse(regExPages, input, "Value")
                .Select(p => $"https://readms.net{p}");

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(pages, new Progress<int>((count) =>
            {
                var f = (float)count / pages.Count();
                int i = Convert.ToInt32(f * 100);
                progress.Report(i);
            }), cancellationToken);
            var images = parser.Parse("<img id=\"manga-page\" src=\"(?<Value>[^\"]+)\"", pageData,
                "Value");
            return images.Select(i => $"https:{i}");
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaStream), "http://readms.net/manga", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("readms.net");
        }
    }
}
