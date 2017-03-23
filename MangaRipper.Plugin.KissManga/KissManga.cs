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

        // _0x331e[20] = a5e8e2e9c2721be0a84ad660c472c1f3 - IV in lo.js
        private string _iv = "a5e8e2e9c2721be0a84ad660c472c1f3";
        private string _keyFirstPart = "_0xa5a2";
        private string _keySecondPart = "_0x2c7e";

        public override void Configuration(IEnumerable<KeyValuePair<string, object>> settings)
        {
            var settingCollection = settings.ToArray();
            if (settingCollection.Any(i => i.Key.Equals("iv")))
            {
                var iv = settingCollection.First(i => i.Key.Equals("iv")).Value;
                _logger.Info($@"Current IV: {_iv}. New IV: {iv}");
                _iv = iv as string;
            }
            if (settingCollection.Any(i => i.Key.Equals("keyFirstPart")))
            {
                var keyFirstPart = settingCollection.First(i => i.Key.Equals("keyFirstPart")).Value;
                _logger.Info($@"Current keyFirstPart: {_keyFirstPart}. New keyFirstPart: {keyFirstPart}");
                _keyFirstPart = keyFirstPart as string;
            }
            if (settingCollection.Any(i => i.Key.Equals("keySecondPart")))
            {
                var keySecondPart = settingCollection.First(i => i.Key.Equals("keySecondPart")).Value;
                _logger.Info($@"Current keySecondPart: {_keySecondPart}. New keySecondPart: {keySecondPart}");
                _keySecondPart = keySecondPart as string;
            }
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

            // find all pages in a chapter
            string input = await downloader.DownloadStringAsync(chapter.Url);

            // lstImages.push(wrapKA("NDOeJU8ZBXxMjFTpgRw1b0ZnYzl8vpuyRsizNH1yBQfAMe3C8nblEjlasXI0t2dr75XMONW6A/37Hb6xc5jHl2j2hbXOAC+x8G1nnW5FgEQxc+w8GyjbI0CjGHviBEht"));
            var encryptPages = parser.Parse("lstImages.push\\(wrapKA\\(\"(?<Value>.[^\"]*)\"\\)\\)", input, "Value");

            // var _0xa5a2 = ["\\x37\\x32\\x6E\\x6E\\x61\\x73\\x64\\x61\\x73\\x64\\x39\\x61\\x73\\x64\\x6E\\x31\\x32\\x33"];
            var firstHex = parser.Parse("var " + _keyFirstPart + " = \\[\"(?<Value>.[^\"]*)\"\\];", input, "Value").FirstOrDefault();

            // var _0x2c7e = ["\\x6E\\x61\\x73\\x64\\x62\\x61\\x73\\x64\\x36\\x31\\x32\\x62\\x61\\x73\\x64"];
            var secondHex = parser.Parse("var " + _keySecondPart + " = \\[\"(?<Value>.[^\"]*)\"\\];", input, "Value").FirstOrDefault();

            if (string.IsNullOrEmpty(firstHex) || string.IsNullOrEmpty(secondHex))
            {
                _logger.Error("Cannot find necessary values on page.");
                throw new MangaRipperException("Cannot decrypt links in KissManga. Necessary values are not exist.");
            }

            var pages = DecryptTheURLs(encryptPages, firstHex, secondHex);

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

        #region Images Decryption
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pages">all encrypted url's</param>
        /// <param name="firstHex">First part of the key</param>
        /// <param name="secondHex">Second part of the key</param>
        /// <returns>Decrypted pages</returns>
        private IEnumerable<string> DecryptTheURLs(IEnumerable<string> pages, string firstHex, string secondHex)
        {
            List<string> newPages = new List<string>();

            var hexEncode1 = UrlDecryption.FromHexToString(firstHex);
            var hexEncode2 = UrlDecryption.FromHexToString(secondHex);
            
            var key = UrlDecryption.ReturnShaKeyBytes(hexEncode1 + hexEncode2);

            foreach (var page in pages)
            {
                newPages.Add(UrlDecryption.DecryptStringAES(page, _iv, key));
            }

            return newPages;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="iv"></param>
        /// <param name="keybytes"></param>
        /// <returns></returns>
        //private string DecryptStringAES(string cipherText, string iv, byte[] keybytes)
        //{
        //    var ivbytes = Enumerable.Range(0, iv.Length)
        //             .Where(x => x % 2 == 0)
        //             .Select(x => Convert.ToByte(iv.Substring(x, 2), 16))
        //             .ToArray();

        //    var encrypted = Convert.FromBase64String(cipherText);
        //    var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, ivbytes);
        //    return string.Format(decriptedFromJavascript);
        //}

        
        //private string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        //{
        //    // Check arguments.  
        //    if (cipherText == null || cipherText.Length <= 0)
        //    {
        //        _logger.Error("Text for decryption was not provided");
        //        throw new ArgumentNullException("cipherText");
        //    }
        //    if (key == null || key.Length <= 0)
        //    {
        //        _logger.Error("Key for decryption was not provided");
        //        throw new ArgumentNullException("key");
        //    }
        //    if (iv == null || iv.Length <= 0)
        //    {
        //        _logger.Error("IV for decryption was not provided");
        //        throw new ArgumentNullException("key");
        //    }

        //    // Declare the string used to hold  
        //    // the decrypted text.  
        //    string plaintext = null;

        //    // Create an RijndaelManaged object  
        //    // with the specified key and IV.  
        //    using (var rijAlg = new RijndaelManaged())
        //    {
        //        //Settings
        //        rijAlg.Mode = CipherMode.CBC;
        //        rijAlg.Padding = PaddingMode.PKCS7;
        //        rijAlg.FeedbackSize = 128;

        //        rijAlg.Key = key;
        //        rijAlg.IV = iv;

        //        // Create a decrytor to perform the stream transform.  
        //        var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

        //        try
        //        {
        //            // Create the streams used for decryption.  
        //            using (var msDecrypt = new MemoryStream(cipherText))
        //            {
        //                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //                {

        //                    using (var srDecrypt = new StreamReader(csDecrypt))
        //                    {
        //                        // Read the decrypted bytes from the decrypting stream  
        //                        // and place them in a string.  
        //                        plaintext = srDecrypt.ReadToEnd();

        //                    }
        //                }
        //            }
        //        }
        //        catch
        //        {
        //            _logger.Error("Decryption Failed");
        //            throw new MangaRipperException("Cannot decrypt links in KissManga");
        //        }
        //    }

        //    return plaintext;
        //}

        //private byte[] ReturnShaKeyBytes(string cipherText)
        //{
        //    byte[] bytes = Encoding.UTF8.GetBytes(cipherText);
        //    SHA256Managed hashstring = new SHA256Managed();
        //    return hashstring.ComputeHash(bytes);
        //}

        ///// <summary>
        ///// Change keys from Hex to String
        ///// </summary>
        ///// <param name="hex">Text as "\\x2E\\x0E"</param>
        ///// <returns></returns>
        //private string FromHexToString(string hex)
        //{
        //    // because first element will be empty
        //    var hexEncode = hex.Replace(@"\", string.Empty).Substring(1).Split('x');

        //    var sb = new StringBuilder();

        //    foreach (string liter in hexEncode)
        //    {
        //        int value = Convert.ToInt32(liter, 16);
        //        sb.Append(char.ConvertFromUtf32(value));
        //    }

        //    return sb.ToString();
        //}

        #endregion
    }
}
