using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    public class TitleOtakuworks : TitleBase
    {
        public TitleOtakuworks(Uri address) : base(address) { }

        protected override List<IChapter> ParseChapterObjects(string html)
        {
            var list = new List<IChapter>();
            Regex reg = new Regex(@"<a href=""(?<Value>/view/[^""]+)"">(?<Text>[^\<]+)</a>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            string title = ParseTitleName(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value + "/read/");
                string name = string.Format("{0} {1}", title, match.Groups["Text"].Value);
                IChapter chapter = new ChapterOtakuworks(name, value);
                list.Add(chapter);
            }

            return list;
        }

        private string ParseTitleName(string html)
        {
            string name = "";
            Regex reg = new Regex(@"<title>(?<Text>[^(]+)\s\(Manga\)",
                RegexOptions.IgnoreCase);

            Match match = reg.Match(html);

            if (match.Success)
            {
                name = match.Groups["Text"].Value;
            }

            return name;
        }
    }
}
