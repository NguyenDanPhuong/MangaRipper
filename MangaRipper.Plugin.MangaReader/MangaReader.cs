using MangaRipper.Core.Logging;
using MangaRipper.Core.Models;
using MangaRipper.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Plugin.MangaReader
{
    /// <summary>
    /// Support find chapters and images from MangaReader
    /// </summary>
    public class MangaReader : IPlugin
    {
        private static ILogger logger;
        private readonly IHttpDownloader downloader;
        private readonly IXPathSelector selector;

        public MangaReader(ILogger myLogger, IHttpDownloader downloader, IXPathSelector selector)
        {
            logger = myLogger;
            this.downloader = downloader;
            this.selector = selector;
        }
        public async Task<IEnumerable<Chapter>> GetChapters(string manga, IProgress<string> progress, CancellationToken cancellationToken)
        {
            string input = await downloader.GetStringAsync(manga, cancellationToken);
            var title = selector.Select(input, "//h2[@class='aname']").InnerText;
            var chaps = selector
                .SelectMany(input, "//table[@id='listing']//a")
                .Select(n =>
                {
                    string url = n.Attributes["href"];
                    var resultUrl = new Uri(new Uri(manga), url).AbsoluteUri;
                    return new Chapter(n.InnerText, resultUrl);
                });
            chaps = chaps.Reverse().GroupBy(x => x.Url).Select(g => g.First()).ToList();
            return chaps;
        }

        public async Task<IEnumerable<string>> GetImages(string chapterUrl, IProgress<string> progress, CancellationToken cancellationToken)
        {
            // find all pages in a chapter
            string input = await downloader.GetStringAsync(chapterUrl, cancellationToken);
            var pages = selector.SelectMany(input, "//select[@id='pageMenu']/option")
                .Select(n => n.Attributes["value"]);

            // transform pages link
            pages = pages.Select(p =>
            {
                var value = new Uri(new Uri(chapterUrl), p).AbsoluteUri;
                return value;
            }).ToList();

            // find all images in pages
            var images = new List<string>();
            foreach (var page in pages)
            {
                var pageHtml = await downloader.GetStringAsync(page, cancellationToken);
                var image = selector
                .Select(pageHtml, "//img[@id='img']").Attributes["src"];
                images.Add(image);
                
                progress.Report("Detecting: " + images.Count);
            }

            return images;
        }


        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaReader), "https://www.mangareader.net", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangareader.net");
        }
    }
}
