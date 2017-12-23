using HtmlAgilityPack;
using Jurassic;
using MangaRipper.Core.CustomException;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class MyHtmlNode
    {
        internal MyHtmlNode(HtmlNode node)
        {
            Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in node.Attributes)
            {
                Attributes.Add(item.Name, item.Value);
            }
            InnerHtml = node.InnerHtml;
        }

        public Dictionary<string, string> Attributes { get; set; }
        public string InnerHtml { get; set; }
    }
    public class HtmlAtilityPackAdapter : IXPathSelector
    {
        private HtmlDocument source;

        public HtmlAtilityPackAdapter()
        {
            source = new HtmlDocument();
        }

        public IEnumerable<MyHtmlNode> SelectMany(string html, string xpath)
        {
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
