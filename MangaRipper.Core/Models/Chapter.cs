using MangaRipper.Core.Extensions;

namespace MangaRipper.Core.Models
{
    public class Chapter
    {
        public string Name { get; private set; }
        /// <summary>
        /// Chapter name which safe for using as folder name.
        /// </summary>
        public string NomalizeName => Name.RemoveFileNameInvalidChar();

        public string OriginalName;

        public string Url { get; private set; }

        public Chapter(string name, string url, string originalName = null)
        {
            Name = name;
            Url = url;
            OriginalName = originalName ?? name;
        }

        public void AddPrefix(int prefix, bool addPrefix)
        {
            Name = (addPrefix) ? $"[{prefix:000}] - {OriginalName}" : OriginalName;
        }
    }
}
