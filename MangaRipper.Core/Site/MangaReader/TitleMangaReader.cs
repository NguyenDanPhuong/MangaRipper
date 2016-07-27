using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    public class TitleMangaReader : TitleBase
    {
        public TitleMangaReader(Uri address) : base(address) { }

        protected override List<IChapter> ParseChapterObjects(string html)
        {
            var list = new List<IChapter>();
            Regex reg = new Regex("<a href=\"(?<Value>[^\"]+)\">(?<Text>[^<]+)</a> :",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                string name = match.Groups["Text"].Value;

                var recentItem = list.Where(c => c.Address == value).FirstOrDefault();

                if (recentItem != null)
                {
                    list.Remove(recentItem);
                }

                IChapter chapter = new ChapterMangaReader(name, value);
                list.Add(chapter);
            }

            list.Reverse();

            return list;
        }
    }
}
