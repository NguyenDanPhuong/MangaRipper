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

        static ScriptEngine Engine;

        public KissManga()
        {
            _decryptor = new KissMangaTextDecryption(_iv, _chko);
        }

        public void InitializeJurassicEngine()
        {
            if (Engine == null)
            {
                Engine = new ScriptEngine();
            }

            /// Consider using a one-and-done approach with the engine.
            ///  Make it static and allow it to be reused.
            ///  
            /// Download the latest files from KissManga, to do so, the HTTP client will
            /// also have to be static as to avoid the five second wait per request.
            /// Investigate methods to achieve the aforementioned.
            /// 

            /// Execute the following JavaScript files as the browser would do.
            /// Engine.Execute(Properties.Resources.KissManga_CryptoJs);
            /// Engine.Execute(Properties.Resources.KissManga_lo);

            // Series
            // http://kissmanga.com/Manga/Koe-no-Katachi/

            // Chapters
            // http://kissmanga.com/Manga/Koe-no-Katachi/vol-000-ch-000?id=323664
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

            InitializeJurassicEngine();

            string pattern = "<script\\s+(type=[\"']text/javascript[\"'])?\\s+(src=[\"']/Scripts/{0}[\"'])>";

            if (Regex.IsMatch(input, string.Format(pattern, "ca.js"), RegexOptions.IgnoreCase | RegexOptions.Compiled) &&
                Regex.IsMatch(input, string.Format(pattern, "lo.js"), RegexOptions.IgnoreCase | RegexOptions.Compiled))
            {
                string funcUri = "http://kissmanga.com/Scripts/lo.js";                
                string decryptFunc = await downloader.DownloadStringAsync(funcUri, cancellationToken);

                // Execute CryptoJS from saved resources to reduce HTTP requests.
                Engine.Execute(Properties.Resources.CryptoJs);

                // Execute the decryption function to allow it to be called later.
                Engine.Execute(decryptFunc);

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
                    Engine.Execute(keys);
                }
                
                var encryptPages = parser.Parse("lstImages.push\\(wrapKA\\(\"(?<Value>.[^\"]*)\"\\)\\)", input, "Value");

                var pages = encryptPages.Select(e => {
                    string value = string.Empty;

                    try
                    {
                        value = Engine.CallGlobalFunction<string>("wrapKA", e);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(e);
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


            //var pages = encryptPages.Select(e => _decryptor.DecryptFromBase64(e));

            //// transform pages link
            //pages = pages.Select(p =>
            //{
            //    var value = new Uri(new Uri(chapter.Url), p).AbsoluteUri;
            //    return value;
            //}).ToList();
            //return pages;
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
