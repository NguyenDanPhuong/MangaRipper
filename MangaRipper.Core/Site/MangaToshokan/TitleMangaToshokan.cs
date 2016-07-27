using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    public class TitleMangaToshokan : TitleBase
    {
        public TitleMangaToshokan(Uri address) : base(address) { }

        protected override List<IChapter> ParseChapterObjects(string html)
        {
            var list = new List<IChapter>();
            Regex reg = new Regex("<td width='40%' align='left' class='ccell'><a href='(?<Value>[^']+)' title=\"[^\"]+\">(?<Text>[^<]+)</a><span>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                string name = match.Groups["Text"].Value;
                IChapter chapter = new ChapterMangaToshokan(name, value);
                list.Add(chapter);
            }

            return list;
        }

        protected override List<Uri> ParseChapterAddresses(string html)
        {
            var list = new List<Uri>();
            Regex reg = new Regex(@"<a href='(?<Value>http://www.mangatoshokan.com/series/[^/]+/\d+)'>\d+</a>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                if (list.Where(r=>r.AbsoluteUri == match.Groups["Value"].Value).Count() == 0)
                {
                    var value = new Uri(Address, match.Groups["Value"].Value);
                    list.Add(value);
                }
            }

            return list;
        }
    }
}
