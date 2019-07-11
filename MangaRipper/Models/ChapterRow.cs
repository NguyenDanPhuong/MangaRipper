using MangaRipper.Core.Models;

namespace MangaRipper.Models
{
    public class ChapterRow
    {
        public ChapterRow()
        {

        }
        public ChapterRow(Chapter chapter)
        {
            Name = chapter.Name;
            Url = chapter.Url;
        }
        public int Prefix { get; set; }
        public string DisplayName => Prefix > 0 ? $"[{Prefix:000}] {Name}" : Name;
        public string Name { get; private set; }
        public string Url { get; private set; }
    }
}
