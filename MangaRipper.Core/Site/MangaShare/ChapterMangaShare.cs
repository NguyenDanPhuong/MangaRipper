using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    [Serializable]
    public class ChapterMangaShare : ChapterBase
    {
        public ChapterMangaShare(string name, Uri address) : base(name, address) { }

        protected override List<Uri> ParsePageAddresses(string html)
        {
            var list = new List<Uri>();
            list.Add(Address);
            Regex reg = new Regex(@"<select name=""pagejump"" class=""page"" onchange=""javascript:window.location='(?<Value>[^']+)'\+this\.value\+'\.html';"">",
                RegexOptions.IgnoreCase);

            Match m = reg.Match(html);
            if (m.Success)
            {
                string value = m.Groups["Value"].Value;

                reg = new Regex(@"<option value=""(?<FileName>\d+)"">Page \d+</option>",
                RegexOptions.IgnoreCase);

                MatchCollection matches = reg.Matches(html);

                foreach (Match match in matches)
                {
                    string link = value + match.Groups["FileName"].Value + ".html";
                    var url = new Uri(Address, link);
                    list.Add(url);
                }
            }
            return list;
        }

        protected override List<Uri> ParseImageAddresses(string html)
        {
            var list = new List<Uri>();
            Regex reg = new Regex(@"<img src=""(?<Value>[^""]+)"" border=""0"" alt=""[^""]+"" />\n",
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
