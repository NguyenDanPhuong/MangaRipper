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
using Jurassic;

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

        private ScriptEngine _engine = null;

        public KissManga()
        {
            _decryptor = new KissMangaTextDecryption(_iv, _chko);
            InitializeJurassicEngine();
        }

        public void InitializeJurassicEngine()
        {
            _engine = new ScriptEngine();            
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
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);
            var chaps = parser.ParseGroup("<td>\\s+<a\\s+href=\"(?=/Manga/)(?<Value>.[^\"]*)\"\\s+title=\"(?<Name>.[^\"]*)\"", input, "Name", "Value");
            chaps = chaps.Select(c => NameResolver(c.Name, c.Url, new Uri(manga)));
            progress.Report(100);
            return chaps;
        }

        public override async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var downloader = new DownloadService();
            var parser = new ParserHelper();
            string input = await downloader.DownloadStringAsync(chapter.Url, cancellationToken);

            /// Could be secured against changes by capturing the script's path as it exists in the live document instead of assuming the location.
            string pattern = "<script\\s+(type=[\"']text/javascript[\"'])?\\s+(src=[\"']/Scripts/{0}[\"'])>";
            string concatedPattern = string.Concat(string.Format(pattern, "ca.js"), "|", string.Format(pattern, "lo.js"));

            if (Regex.IsMatch(input, concatedPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled))
            {
                string funcUri = "http://kissmanga.com/Scripts/lo.js";
                string decryptFunc = await downloader.DownloadStringAsync(funcUri, cancellationToken);

                /// Execute CryptoJS from saved resources to reduce HTTP requests.
                _engine.Execute(Properties.Resources.CryptoJs);

                /// Execute the decryption function to allow it to be called later.
                _engine.Execute(decryptFunc);


                var keysPattern = "<script type=\"text/javascript\">[\\s]*(?<Value>.*)(?!</script>)";
                var regex = new Regex(keysPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var keys = string.Empty;

                foreach (Match match in regex.Matches(input))
                {
                    if (match.Value.Contains("CryptoJS"))
                    {
                        keys = match.Groups["Value"].Value;
                        break;
                    }
                }
                
                if (string.IsNullOrWhiteSpace(keys))
                {
                    throw new ArgumentException("Cannot decrypt image URIs.");
                }
                else
                {
                    _engine.Execute(keys);
                }

                /// As with the script locations, to avoid unnecessary breaking the application, the function name could be captured and invoked 
                /// in the event it changes.
                var encryptPages = parser.Parse("lstImages.push\\(wrapKA\\(\"(?<Value>.[^\"]*)\"\\)\\)", input, "Value");

                var pages = encryptPages.Select(e =>
                {
                    string value = string.Empty;

                    try
                    {
                        value = _engine.CallGlobalFunction<string>("wrapKA", e);
                    }
                    catch (Exception ex)
                    {
                        _logger.Fatal(ex);
                        throw;
                    }

                    return value;
                });


                pages = pages.Select(p =>
                {
                    var value = new Uri(new Uri(chapter.Url), p).AbsoluteUri;
                    return value;
                }).ToList();

                return pages;
            }

            return null;

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
