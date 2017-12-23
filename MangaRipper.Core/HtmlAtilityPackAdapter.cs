using HtmlAgilityPack;
using MangaRipper.Core.CustomException;
using MangaRipper.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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
