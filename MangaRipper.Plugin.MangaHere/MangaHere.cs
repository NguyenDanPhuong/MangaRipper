using MangaRipper.Core;
using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
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
    public class MangaHere : IMangaService
    {
        private static ILogger logger;
        private readonly Downloader downloader;
        private readonly ParserHelper parser;

        public MangaHere(ILogger myLogger, Downloader downloader, ParserHelper parser)
        {
            logger = myLogger;
            this.downloader = downloader;
            this.parser = parser;
        }
        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            var chaps = parser.ParseGroup("<a class=\"color_0077\" href=\"(?<Value>[^\"]+)\" >\r\n              (?<Name>[^<]+)            </a>", input, "Name", "Value");
            chaps = chaps.Select(c =>
            {
                return new Chapter(c.OriginalName, $"http:{c.Url}");
            });
            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url, cancellationToken);
            var pages = parser.Parse(@"<option value=""(?<Value>[^""]+)"" (|selected=""selected"")>.+</option>", input, "Value");

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
            return new SiteInformation(nameof(MangaHere), "http://www.mangahere.cc", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangahere.cc");
        }
    }
}
