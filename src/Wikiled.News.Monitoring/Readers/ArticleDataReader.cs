using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers
{
    public class ArticleDataReader : IArticleDataReader
    {
        private readonly ILogger<ArticleDataReader> logger;

        private readonly Func<ArticleDefinition, ISessionReader> sessionReader;

        private IDefinitionTransformer transformer;

        public ArticleDataReader(ILoggerFactory loggerFactory, Func<ArticleDefinition, ISessionReader> sessionReader, IDefinitionTransformer transformer)
        {
            this.sessionReader = sessionReader ?? throw new ArgumentNullException(nameof(sessionReader));
            this.transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
            logger = loggerFactory?.CreateLogger<ArticleDataReader>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Article> Read(ArticleDefinition definition)
        {
            logger.LogDebug("Reading article: {0}[{1}]", definition.Title, definition.Id);
            definition = transformer.Transform(definition);
            var comments = ReadComments(definition);
            var readArticle = sessionReader(definition).ReadArticle();
            return new Article(definition, await comments.ConfigureAwait(false), await readArticle.ConfigureAwait(false), DateTime.UtcNow);
        }

        private async Task<CommentData[]> ReadComments(ArticleDefinition definition)
        {
            var commentsReader = sessionReader(definition).ReadComments();
            var result = await commentsReader.ReadAllComments().ToArray();
            return result;
        }
    }
}
