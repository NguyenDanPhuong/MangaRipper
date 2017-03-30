using MangaRipper.Core.CustomException;
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
    public class KissManga : MangaService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private string _iv = "a5e8e2e9c2721be0a84ad660c472c1f3";
        private string _chko = "nsfd732nsdnds823nsdf";

        private KissMangaTextDecryption _decryptor;

        public KissManga()
        {
            _decryptor = new KissMangaTextDecryption(_iv, _chko);
        }

        public override void Configuration(IEnumerable<KeyValuePair<string, object>> settings)
        {
            var settingCollection = settings.ToArray();
            if (settingCollection.Any(i => i.Key.Equals("IV")))
            {
                var iv = settingCollection.First(i => i.Key.Equals("IV")).Value;
                _logger.Info($@"Current IV: {_iv}. New IV: {iv}");
                _iv = iv as string;
            }
            if (settingCollection.Any(i => i.Key.Equals("Chko")))
            {
                var chko = settingCollection.First(i => i.Key.Equals("Chko")).Value;
                _logger.Info($@"Current Chko: {_chko}. New Chko: {chko}");
                _chko = chko as string;
            }
            _decryptor = new KissMangaTextDecryption(_iv, _chko);
        }

        public override async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();
            progress.Report(0);
            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<td>\\s+<a\\s+href=\"(?=/Manga/)(?<Value>.[^\"]*)\"\\s+title=\"(?<Name>.[^\"]*)\"", input, "Name", "Value");
            chaps = chaps.Select(c => NameResolver(c.Name, c.Url, new Uri(manga)));
            progress.Report(100);
            return chaps;
        }

        public override async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();
            string input = await downloader.DownloadStringAsync(chapter.Url);
            var encryptPages = parser.Parse("lstImages.push\\(wrapKA\\(\"(?<Value>.[^\"]*)\"\\)\\)", input, "Value");
            var pages = encryptPages.Select(e => _decryptor.DecryptFromBase64(e));
            // transform pages link
            pages = pages.Select(p =>
            {
                var value = new Uri(new Uri(chapter.Url), p).AbsoluteUri;
                return value;
            }).ToList();
            return pages;
        }

        public override SiteInformation GetInformation()
        {
            return new SiteInformation("KissManga", "http://kissmanga.com/", "English");
        }

        public override bool Of(string link)
        {
            return new Uri(link).Host.Equals("kissmanga.com");
        }

        private Chapter NameResolver(string name, string value, Uri adress)
        {
            var urle = new Uri(adress, value);
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = System.Net.WebUtility.HtmlDecode(name);
                name = Regex.Replace(name, "^Read\\s+|\\s+online$|:", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                name = Regex.Replace(name, "\\s+Read\\s+Online$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            return new Chapter(name, urle.AbsoluteUri);
        }
    }
}
