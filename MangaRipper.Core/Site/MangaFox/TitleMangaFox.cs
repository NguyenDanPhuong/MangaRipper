using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    public class TitleMangaFox : TitleBase
    {
        public TitleMangaFox(Uri address) : base(address) { }

        protected override List<IChapter> ParseChapterObjects(string html)
        {
            var list = new List<IChapter>();
            Regex reg = new Regex("<a href=\"(?<Value>[^\"]+)\" title=\"(|[^\"]+)\" class=\"tips\">(?<Text>[^<]+)</a>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                string name = match.Groups["Text"].Value;
                IChapter chapter = new ChapterMangaFox(name, value);
                list.Add(chapter);
            }

            return list;
        }
    }
}
