using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Feeds;

namespace Wikiled.News.Monitoring.Monitoring
{
    public class ArticlesMonitor : IArticlesMonitor
    {
        private readonly IFeedsHandler handler;

        private readonly IScheduler scheduler;

        private readonly ILogger<ArticlesMonitor> logger;

        private Dictionary<string, Article> scanned = new Dictionary<string, Article>();

        public ArticlesMonitor(ILoggerFactory loggerFactory, IScheduler scheduler, IFeedsHandler handler)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            logger = loggerFactory.CreateLogger<ArticlesMonitor>();
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public IObservable<Article> Start()
        {
            logger.LogDebug("Start");
            var scanFeed = Observable.Interval(TimeSpan.FromHours(1), scheduler)
                .SelectMany(handler.GetArticles())
                .Where(item => !scanned.ContainsKey(item.Id))
                .Select(ArticleReceived)
                .Where(item => item != null);

            var updated = Observable.Interval(TimeSpan.FromHours(6), scheduler)
                .SelectMany(Updated().ToObservable(scheduler));

            return scanFeed.Merge(updated);
        }

        private IEnumerable<Article> Updated()
        {
            throw new NotImplementedException();
        }

        private Article ArticleReceived(ArticleDefinition article)
        {
            logger.LogDebug("ArticleReceived: {0}({1})", article.Topic, article.Id);
            //if (cache.TryGetValue(article.ToString(), out var cached))
            //{
            //    logger.LogDebug("Article already processed: {0}", article.Id);
            //    return null;
            //}

            throw new NotImplementedException();
        }
    }
}
