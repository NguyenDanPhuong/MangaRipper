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
    public class MangaFox : IMangaService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        void IMangaService.Configuration(IEnumerable<KeyValuePair<string, object>> settings)
        {
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaFox), "http://mangafox.me", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("mangafox.me");
        }

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            Logger.Info($@"> FindChapters(): {manga}");
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

            var pages = (await FindPagesInChapter(chapter.Url)).ToList();
            var transformedPages = TransformPagesUrl(chapter.Url, pages).ToArray();
            
            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(
                transformedPages,
                new Progress<int>(count =>
                {
                    var f = (float) count / transformedPages.Count();
                    var i = Convert.ToInt32(f * 100);
                    progress.Report(i);
                }),
                cancellationToken);

            var images = parser.Parse("<img src=\"(?<Value>[^\"]+)\"[ ]+width=", pageData, "Value");

            progress.Report(100);
            return images;
        }

        private async Task<IEnumerable<string>> FindPagesInChapter(string chapterUrl)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();
            var input = await downloader.DownloadStringAsync(chapterUrl);
            return parser.Parse(@"<option value=""(?<Value>[^""]+)"" (|selected=""selected"")>\d+</option>", input, "Value");
        }

        private IEnumerable<string> TransformPagesUrl(string chapterUrl, IEnumerable<string> pages)
        {
            return pages.Select(p =>
            {
                var value = new Uri(new Uri(chapterUrl), (p + ".html")).AbsoluteUri;
                return value;
            });
        }
    }
}
