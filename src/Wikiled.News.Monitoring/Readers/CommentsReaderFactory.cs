using System;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers
{
    public class CommentsReaderFactory : ICommentsReaderFactory
    {
        private readonly ILoggerFactory loggerFactory;

        private readonly IHtmlReader reader;

        public CommentsReaderFactory(ILoggerFactory loggerFactory, IHtmlReader reader)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public ICommentsReader Create(ArticleDefinition article, bool anonymous)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            throw new NotImplementedException();
            //return new CommentsReader(
            //    loggerFactory, 
            //    article, 
            //    anonymous ? new AnonymousAdjuster() : (IAdjuster)new RegisteredAdjuster(), 
            //    reader);
        }
    }
}
