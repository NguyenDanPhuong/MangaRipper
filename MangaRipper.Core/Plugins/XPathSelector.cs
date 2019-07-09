using HtmlAgilityPack;
using MangaRipper.Core.CustomException;
using MangaRipper.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace MangaRipper.Core.Plugins
{
    public class XpathSelector : IXPathSelector
    {
        public IEnumerable<MyHtmlNode> SelectMany(string html, string xpath)
        {
            var source = new HtmlDocument();
            source.LoadHtml(html);
            return source.DocumentNode
                .SelectNodes(xpath)
                .Select(n =>
                {
                    if (n == null)
                    {
                        throw new MangaRipperException("Cannot find html node by inputed xpath.");
                    }
                    return new MyHtmlNode(n);
                });
        }

        public MyHtmlNode Select(string html, string xpath)
        {
            var source = new HtmlDocument();
            source.LoadHtml(html);
            var node = source.DocumentNode.SelectSingleNode(xpath);
            if(node == null)
            {
                throw new MangaRipperException("Cannot find html node by inputed xpath.");
            }
            return new MyHtmlNode(node);
        }
    }
}
