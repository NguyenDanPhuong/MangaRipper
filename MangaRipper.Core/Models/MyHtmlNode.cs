using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace MangaRipper.Core.Models
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
            InnerText = node.InnerText;
        }

        public Dictionary<string, string> Attributes { get; set; }
        public string InnerText { get; set; }
    }
}
