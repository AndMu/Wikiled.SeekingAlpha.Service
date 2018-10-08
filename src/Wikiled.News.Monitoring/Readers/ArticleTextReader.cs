using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers
{
    public class ArticleTextReader : IArticleTextReader
    {
        private readonly ILogger<ArticleTextReader> logger;

        private readonly IHtmlReader reader;

        public ArticleTextReader(ILoggerFactory loggerFactory, IHtmlReader reader)
        {
            logger = loggerFactory?.CreateLogger<ArticleTextReader>() ?? throw new ArgumentNullException(nameof(logger));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public async Task<ArticleText> ReadArticle(ArticleDefinition definition)
        {
            logger.LogDebug("Reading article text: {0}", definition.Id);
            var page = await reader.ReadDocument(definition.Url.ToString());
            var doc = page.DocumentNode;
            var article = doc.QuerySelector("div.article-body");
            var description = article.QuerySelector("div[itemprop='description']");
            var articleInner = doc.QuerySelector("div[itemprop='articleBody']");
            StringBuilder builder = new StringBuilder();
            var pargraphs = articleInner.QuerySelectorAll("p");
            foreach (var pargraph in pargraphs)
            {
                foreach (var textNode in pargraph.ChildNodes.Where(item => item is HtmlTextNode || string.Compare(item.Name, "a", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    builder.Append(textNode.InnerText.Trim());
                    builder.Append(' ');
                }
            }

            return new ArticleText
            {
                Description = description.InnerText?.Trim(),
                Text = builder.ToString()
            };
        }
    }
}
