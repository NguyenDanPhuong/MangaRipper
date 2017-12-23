using MangaRipper.Core.Extensions;

namespace MangaRipper.Core.Models
{
    public class Chapter
    {
        public string OriginalName { get; }
        public int Prefix { get; set; }
        public string Name => Prefix > 0 ? $"[{Prefix:000}] {OriginalName.RemoveFileNameInvalidChar()}" : OriginalName.RemoveFileNameInvalidChar();
        public string Url { get;}
        public string Language { get; set; }
        public Chapter(string originalName, string url)
        {
            OriginalName = originalName;
            Url = url;
        }
    }
}
