using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    [Serializable]
    public class ChapterMangaShare : ChapterBase
    {
        public ChapterMangaShare(string name, string address) : base(name, address) { }

        protected override List<string> ParsePageAddresses(string html)
        {
            var list = new List<string>();
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
                    var url = new Uri(new Uri(Address), link).AbsoluteUri;
                    list.Add(url);
                }
            }
            return list;
        }

        protected override List<string> ParseImageAddresses(string html)
        {
            var list = new List<string>();
            Regex reg = new Regex(@"<img src=""(?<Value>[^""]+)"" border=""0"" alt=""[^""]+"" />\n",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(new Uri(Address), match.Groups["Value"].Value).AbsoluteUri;
                list.Add(value);
            }

            return list;
        }
    }
}
