using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaRipper.Core.Logging;
using MangaRipper.Core.Models;
using MangaRipper.Core.Plugins;

namespace MangaRipper.Plugin.ReadOPM
{
    /// <summary>
    /// Support find chapters, images from readopm
    /// </summary>
    public class ReadOPM : IPlugin
    {
        private static ILogger<ReadOPM> logger;
        private readonly IHttpDownloader downloader;

        public ReadOPM(ILogger<ReadOPM> myLogger, IHttpDownloader downloader)
        {
            logger = myLogger;
            this.downloader = downloader;
        }
        public async Task<IReadOnlyCollection<Chapter>> GetChapters(string manga, IProgress<string> progress,
            CancellationToken cancellationToken)
        {
            string input = await downloader.GetStringAsync(manga, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            var chaps = doc.DocumentNode
                .SelectNodes("//ul[contains(@class, 'chapters-list')]/li/a")
                .Select(n =>
                {
                    string url = n.Attributes["href"].Value;
                    return new Chapter(null, url);
                }).ToList();

            var chap_numbers = doc.DocumentNode
                .SelectNodes("//ul[contains(@class, 'chapters-list')]/li/a/span[contains(@class, 'chapter__no')]")
                .Select(n => n.InnerText)
                .ToList();

            chaps.ForEach(c => c.Name = "One Punch Man " + chap_numbers[chaps.IndexOf(c)]);
            return chaps;
        }

        public async Task<IReadOnlyCollection<string>> GetImages(string chapterUrl, IProgress<string> progress,
            CancellationToken cancellationToken)
        {
            progress.Report("Detecting...");
            string input = await downloader.GetStringAsync(chapterUrl, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            var images = doc.DocumentNode.SelectNodes("//div[contains(@class,'img_container')]/img")
                .Select(n => n.Attributes["src"].Value)
                .Where(src =>
                {
                    return !string.IsNullOrWhiteSpace(src)
                    && Uri.TryCreate(src, UriKind.Absolute, out Uri validatedUri)
                    && !string.IsNullOrWhiteSpace(Path.GetFileName(validatedUri.LocalPath));
                });

            return images.ToList();
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(ReadOPM), "https://ww3.readopm.com/", "English");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("ww3.readopm.com");
        }
    }
}
