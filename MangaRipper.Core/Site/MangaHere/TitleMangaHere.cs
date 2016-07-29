using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    class TitleMangaHere:TitleBase
    {
        public TitleMangaHere(string address) : base(address) { }

        protected override List<IChapter> ParseChapterObjects(string html)
        {
            var list = new List<IChapter>();
            Regex reg = new Regex("<a class=\"color_0077\" href=\"(?<Value>[^\"]+)\"[^<]+>(?<Text>[^<]+)</a>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(new Uri(Address), match.Groups["Value"].Value);
                string name = match.Groups["Text"].Value.Trim();
                IChapter chapter = new ChapterMangaHere(name, value);
                list.Add(chapter);
            }

            return list;
        }
    }
}
