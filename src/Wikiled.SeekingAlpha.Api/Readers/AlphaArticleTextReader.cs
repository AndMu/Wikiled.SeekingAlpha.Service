using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.SeekingAlpha.Api.Readers
{
    public class AlphaArticleTextReader : IArticleTextReader
    {
        private readonly ILogger<AlphaArticleTextReader> logger;

        private readonly ITrackedRetrieval reader;

        public AlphaArticleTextReader(ILoggerFactory loggerFactory, ITrackedRetrieval reader)
        {
            logger = loggerFactory?.CreateLogger<AlphaArticleTextReader>() ?? throw new ArgumentNullException(nameof(logger));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public async Task<ArticleText> ReadArticle(ArticleDefinition definition, CancellationToken token)
        {
            logger.LogDebug("Reading article text: {0}", definition.Id);
            var page = (await reader.Read(definition.Url, token).ConfigureAwait(false)).GetDocument();
            var doc = page.DocumentNode;
            var article = doc.QuerySelector("article");
            var title = article.QuerySelector("h1[itemprop='headline']");
            var articleInner = definition.Topic == "News" ? doc.QuerySelector("div#mc-body") : doc.QuerySelector("div#a-body");
            var builder = new StringBuilder();
            var paragraphs = articleInner.QuerySelectorAll("p");
            foreach (var paragraph in paragraphs)
            {
                builder.Append(paragraph.InnerText.Trim());
                builder.Append(' ');
            }

            return new ArticleText
            {
                Title = title.InnerText?.Trim(),
                Text = builder.ToString()
            };
        }
    }
}
