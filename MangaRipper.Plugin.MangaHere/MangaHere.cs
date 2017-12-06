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
    public class MangaHere : MangaService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new ParserHelper();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            var chaps = parser.ParseGroup("<a class=\"color_0077\" href=\"(?<Value>//[^\"]+)\"[^<]+>(?<Name>[^<]+)</a>", input, "Name", "Value");
            progress.Report(100);

            chaps = chaps.Select(chap => {
                var newUri = CheckAndInsertMissingScheme(chap.Url);
                return new Chapter(chap.OriginalName, newUri);
            });

            return chaps;
        }
        
        public override async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new Downloader();
            var parser = new ParserHelper();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url, cancellationToken);
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

        public override SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(MangaHere), "http://www.mangahere.co", "English");
        }

        public override bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("www.mangahere.co");
        }

        /// <summary>
        /// Checks if the URI is missing the HTTP or HTTPS scheme.
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <param name="preferredScheme">The scheme to insert if it is missing one.</param>
        /// <returns></returns>
        public string CheckAndInsertMissingScheme(string uri, string preferredScheme = "http")
        {
            var missingSchemePattern = "^(?!http[s]:)(?=//)";

            if (System.Text.RegularExpressions.Regex.IsMatch(uri, missingSchemePattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                // Insert the missing colon if the preferred scheme doesn't end with one.
                if (!preferredScheme.EndsWith(":"))
                {
                    preferredScheme = string.Concat(preferredScheme, ":");
                }

                // Return the uri with the preferred scheme prefixed.
                return uri.Insert(0, preferredScheme);
            }
            else
            {
                // Return the unchanged value.
                return uri;
            }

        }

    }
}
