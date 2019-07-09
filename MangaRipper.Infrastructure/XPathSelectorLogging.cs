using System.Collections.Generic;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Plugins;

namespace MangaRipper.Infrastructure
{
    public class XPathSelectorLogging : IXPathSelector
    {
        private readonly IXPathSelector decoratee;
        private readonly ILogger logger;

        public XPathSelectorLogging(IXPathSelector decoratee, ILogger logger)
        {
            this.decoratee = decoratee;
            this.logger = logger;
        }

        public MyHtmlNode Select(string html, string xpath)
        {
            try
            {
                return decoratee.Select(html, xpath);
            }
            catch
            {
                logger.Info($"Xpath: {xpath}");
                logger.Fatal(html);
                throw;
            }
        }

        public IEnumerable<MyHtmlNode> SelectMany(string html, string xpath)
        {
            try
            {
                return decoratee.SelectMany(html, xpath);
            }
            catch
            {
                logger.Info($"Xpath: {xpath}");
                logger.Fatal(html);
                throw;
            }
        }
    }
}
