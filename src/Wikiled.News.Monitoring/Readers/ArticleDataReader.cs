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

        private readonly ICommentsReaderFactory commentsReaderFactory;

        private readonly IArticleTextReader articleTextReader;

        public ArticleDataReader(ILoggerFactory loggerFactory, ICommentsReaderFactory commentsReaderFactory, IArticleTextReader articleTextReader)
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
            var anonymous = ReadComments(definition, true);
            var registered = ReadComments(definition, false);
            var readArticle = articleTextReader.ReadArticle(definition);
            return new Article(definition, await anonymous, await registered, await readArticle, DateTime.UtcNow);
        }

        private async Task<CommentsData> ReadComments(ArticleDefinition definition, bool anonymous)
        {
            var commentsReader = commentsReaderFactory.Create(definition, anonymous);
            await commentsReader.Init();
            var result = await commentsReader.ReadAllComments().ToArray();
            if (commentsReader.Total != result.Length)
            {
                logger.LogWarning("Mistmatch in comments count. Expected {0} but received {1}", commentsReader.Total, result.Length);
            }

            return new CommentsData(commentsReader.Total, result);;
        }
    }
}
