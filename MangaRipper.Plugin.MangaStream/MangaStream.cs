using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
using NLog;

namespace MangaRipper.Plugin.MangaStream
{
    /// <summary>
    /// Support find chapters, images from MangaStream
    /// </summary>
    public class MangaStream : MangaService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress,
            CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            string regEx = "<td><a href=\"(?<Value>http://readms.net/r/[^\"]+)\">(?<Name>[^<]+)</a>";
            var chaps = parser.ParseGroup(regEx, input, "Name", "Value");
            progress.Report(100);
            return chaps;
        }

        public override async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress,
            CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url);
            string regExPages =
                "<li><a href=\"(?<Value>http://readms.com/r/[^\"]+)\">[^<]+</a>";
            var pages = parser.Parse(regExPages, input, "Value");

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(pages, new Progress<int>((count) =>
            {
                var f = (float) count / pages.Count();
                int i = Convert.ToInt32(f * 100);
                progress.Report(i);
            }), cancellationToken);
            var images = parser.Parse("<img id=\"manga-page\" src=\"(?<Value>[^\"]+)\"/>", pageData,
                "Value");

            return images;
        }

        public override SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaStream), "http://mangastream.com/manga", "English");
        }

        public override bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("mangastream.com");
        }
    }
}
