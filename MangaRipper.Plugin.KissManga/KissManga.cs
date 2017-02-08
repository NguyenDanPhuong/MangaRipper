using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Plugin.KissManga
{
    /// <summary>
    /// Support find chapters and images from KissManga
    /// </summary>
    public class KissManga : IMangaService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<td>\n<a href=\"(?=/Manga/)(?<Value>.[^\"]*)\" title=\"(?<Name>.[^\"]*)\"", input, "Name", "Value");
            chaps = chaps.Select(c => NameResolver(c.Name, c.Url, new Uri(manga)));
            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url);
            var pages = parser.Parse("lstImages.push\\(\"(?<Value>.[^\"]*)\"\\)", input, "Value");

            // transform pages link
            pages = pages.Select(p =>
            {
                var value = new Uri(new Uri(chapter.Url), p).AbsoluteUri;
                return value;
            }).ToList();
            
            return pages;
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation("KissManga", "http://kissmanga.com/", "English");
        }

        public bool Of(string link)
        {
            return new Uri(link).Host.Equals("kissmanga.com");
        }
        
        private Chapter NameResolver(string name, string value, Uri adress)
        {
            var urle = new Uri(adress, value);
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = System.Net.WebUtility.HtmlDecode(name);
                name = Regex.Replace(name, "^Read\\s+|\\s+online$|:", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                name = Regex.Replace(name, "\\s+Read\\s+Online$", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            return new Chapter(name, urle.AbsoluteUri);
        }

        void IMangaService.Configuration(IEnumerable<KeyValuePair<string, object>> settings)
        {
        }
    }
}
