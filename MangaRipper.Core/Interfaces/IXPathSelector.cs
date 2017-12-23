using System.Collections.Generic;

namespace MangaRipper.Core.Interfaces
{
    public interface IXPathSelector
    {
        IEnumerable<MyHtmlNode> SelectMany(string html, string xpath);
        MyHtmlNode Select(string html, string xpath);
    }
}
