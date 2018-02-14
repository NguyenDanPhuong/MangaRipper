namespace MangaRipper.Core.Models
{
    public class Chapter
    {
        public string Manga { get; set; }
        public string Name { get; set; }
        public int Prefix { get; set; }
        public string DisplayName => Prefix > 0 ? $"[{Prefix:000}] {Name}" : Name;
        public string Url { get; set; }
        public string Language { get; set; }
        public Chapter(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
