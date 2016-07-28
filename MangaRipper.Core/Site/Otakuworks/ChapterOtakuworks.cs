using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    [Serializable]
    public class ChapterOtakuworks : ChapterBase
    {
        public ChapterOtakuworks(string name, Uri address) : base(name, address) { }

        protected override List<Uri> ParseImageAddresses(string html)
        {
            var list = new List<Uri>();
            Regex reg = new Regex(@"(?<Value>http://static\.otakuworks\.net/viewer/[^""]+)",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                list.Add(value);
            }

            return list.Distinct().ToList();
        }

        protected override List<Uri> ParsePageAddresses(string html)
        {
            var list = new List<Uri>();
            Regex reg = new Regex(@"create_jsnumsel2\('fpage1',(?<Min>[^,]+),(?<Max>[^,]+)",
                RegexOptions.IgnoreCase);
            Match match = reg.Match(html);

            if (match.Success)
            {
                int min = Convert.ToInt32(match.Groups["Min"].Value);
                int max = Convert.ToInt32(match.Groups["Max"].Value);
                for (int i = min; i <= max; i++)
                {
                    var value = new Uri(Address, Address.AbsoluteUri + i);
                    list.Add(value);
                }
            }

            return list.Distinct().ToList();
        }
    }
}
