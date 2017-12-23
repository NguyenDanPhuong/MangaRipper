using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public interface IXPathSelector
    {
        IEnumerable<MyHtmlNode> SelectMany(string html, string xpath);
        MyHtmlNode Select(string html, string xpath);
    }
}
