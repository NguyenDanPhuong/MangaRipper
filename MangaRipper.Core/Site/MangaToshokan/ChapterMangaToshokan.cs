using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    [Serializable]
    public class ChapterMangaToshokan : ChapterBase
    {
        public ChapterMangaToshokan(string name, Uri address) : base(name, address) { }

        protected override List<Uri> ParseImageAddresses(string html)
        {
            var list = new List<Uri>();
            Regex reg = new Regex("<img src=\"(?<Value>[^\"]+)\" alt=\"[^\"]+\" id=\"readerPage\"",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                list.Add(value);
            }
           
            return list;
        }

        protected override List<Uri> ParsePageAddresses(string html)
        {
            var list = new List<Uri>();
            Regex reg = new Regex(@"<option value=""(?<Value>/read/[^/]+/[^/]+/[^/]+/[^""]+)""(?:| selected=""selected"")>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                list.Add(value);
            }
           
            return list;
        }
    }
}
