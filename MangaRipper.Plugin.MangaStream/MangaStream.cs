using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string MangaProviderUrl = "readms.net";
        private string RequestedUrl;

        public override async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress,
            CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new ParserHelper();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            string regEx = "<td><a href=\"(?<Value>[^\"]+)\">(?<Name>[^<]+)</a>";
            var chaps = parser.ParseGroup(regEx, input, "Name", "Value");
            chaps = chaps.Select(c => new Chapter(c.OriginalName, $"https://{RequestedUrl}{c.Url}"));
            progress.Report(100);
            return chaps;
        }

        public override async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress,
            CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new ParserHelper();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url, cancellationToken);
            string regExPages =
                "<li><a href=\"(?<Value>/r[^\"]+)\">[^<]+</a>";
            var pages = parser.Parse(regExPages, input, "Value")
                .Select(p => $"https://{RequestedUrl}{p}");

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(pages, new Progress<int>((count) =>
            {
                var f = (float) count / pages.Count();
                int i = Convert.ToInt32(f * 100);
                progress.Report(i);
            }), cancellationToken);
            var images = parser.Parse("<img id=\"manga-page\" src=\"(?<Value>[^\"]+)\"", pageData,
                "Value");
            return images.Select(i => $"https:{i}");
        }

        #region Init methods

        public override SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaStream), "https://readms.net/manga", "English");
        }

        public override bool Of(string link)
        {
            return Of(link, MangaProviderUrl);
        }

        public override bool Of(string link, string providerUrl)
        {
            var uri = new Uri(link);
            RequestedUrl = CheckWithAlternative(uri, providerUrl);
            logger.Info($@"The following Url was chosen {RequestedUrl}");
            return (RequestedUrl != null);
        }

        #endregion
    }
}
