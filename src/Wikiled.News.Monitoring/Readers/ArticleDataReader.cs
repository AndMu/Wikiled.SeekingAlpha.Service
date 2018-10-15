using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers.SeekingAlpha
{
    public class ArticleDataReader : IArticleDataReader
    {
        private readonly ILogger<ArticleDataReader> logger;

        private readonly Func<ICommentsReader> commentsReaderFactory;

        private readonly IArticleTextReader articleTextReader;

        public ArticleDataReader(ILoggerFactory loggerFactory, Func<ICommentsReader> commentsReaderFactory, IArticleTextReader articleTextReader)
        {
            logger = loggerFactory?.CreateLogger<ArticleDataReader>() ?? throw new ArgumentNullException(nameof(logger));
            this.commentsReaderFactory = commentsReaderFactory ?? throw new ArgumentNullException(nameof(commentsReaderFactory));
            this.articleTextReader = articleTextReader ?? throw new ArgumentNullException(nameof(articleTextReader));
        }

        public async Task<bool> RequiresRefreshing(Article article)
        {
            throw new NotImplementedException();
        }

        public async Task<Article> Read(ArticleDefinition definition)
        {
            logger.LogDebug("Reading article: {0}[{1}]", definition.Title, definition.Id);
            var comments = ReadComments(definition);
            var readArticle = articleTextReader.ReadArticle(definition);
            return new Article(definition, await comments, await readArticle, DateTime.UtcNow);
        }

        private async Task<CommentData[]> ReadComments(ArticleDefinition definition)
        {
            //var commentsReader = commentsReaderFactory.Create(definition);
            //var result = await commentsReader.ReadAllComments().ToArray();
            throw new NotImplementedException();
            //if (commentsReader.Total != result.Length)
            //{
            //    logger.LogWarning("Mistmatch in comments count. Expected {0} but received {1}", commentsReader.Total, result.Length);
            //}

            //return new CommentsData(commentsReader.Total, result);;
        }
    }
}
