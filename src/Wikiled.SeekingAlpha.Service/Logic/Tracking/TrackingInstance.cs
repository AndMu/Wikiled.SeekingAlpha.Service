using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.MachineLearning.Mathematics.Tracking;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.Sentiment.Api.Service;

namespace Wikiled.SeekingAlpha.Service.Logic.Tracking
{
    public class TrackingInstance : ITrackingInstance
    {
        private readonly ISentimentAnalysis sentiment;

        private readonly Func<ITracker> trackerFactory;

        private readonly ConcurrentDictionary<string, Lazy<ITracker>> articleTrackers = new ConcurrentDictionary<string, Lazy<ITracker>>(StringComparer.OrdinalIgnoreCase);

        private readonly ILogger<TrackingInstance> logger;

        private readonly IArticlesPersistency persistency;

        public TrackingInstance(ILogger<TrackingInstance> logger, ISentimentAnalysis sentiment, Func<ITracker> trackerFactory, IArticlesPersistency persistency)
        {
            this.sentiment = sentiment ?? throw new ArgumentNullException(nameof(sentiment));
            this.trackerFactory = trackerFactory ?? throw new ArgumentNullException(nameof(trackerFactory));
            this.persistency = persistency ?? throw new ArgumentNullException(nameof(persistency));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Save(Article article)
        {
            try
            {
                logger.LogDebug("Saving: {0} {1}", article.Definition.Feed.Category, article.Definition.Id);
                var saveTask = Task.Run(() => persistency.Save(article));   
                var tracker = Resolve(article.Definition.Feed.Category);
                Dictionary<string, (DateTime Date, string Text)> texts = new Dictionary<string, (DateTime Date, string Text)>();
                if (!tracker.IsTracked(article.Definition.Id))
                {
                    logger.LogDebug("Tracking: {0}", article.Definition.Id);
                    var date = article.Definition.Date ?? DateTime.UtcNow;
                    texts[article.Definition.Id] = (date, article.ArticleText.Text);
                }

                logger.LogDebug("Total comments: {0}", article.Comments.Length);
                foreach (var comment in article.Comments)
                {
                    if (!tracker.IsTracked(comment.Id))
                    {
                        logger.LogDebug("Tracking: {0}", comment.Id);
                        texts[comment.Id] = (comment.Date, comment.Text);
                    }
                }

                if (texts.Count > 0)
                {
                    logger.LogDebug("Checking sentiment: {0}", texts.Count);
                    var request = texts.Select(item => (item.Key, item.Value.Text)).ToArray();
                    var sentimentValue = await sentiment.Measure(request, CancellationToken.None).ToArray();
                    foreach (var tuple in sentimentValue)
                    {
                        var result = texts[tuple.Item1];
                        tracker.AddRating(new RatingRecord(tuple.Item1, result.Date, tuple.Item2));
                    }
                }

                logger.LogDebug("[{0}] Total: {1} with sentiment: {2} Average: {3}",
                                article.Definition.Feed.Category,
                                tracker.Count(false),
                                tracker.Count(),
                                tracker.CalculateAverageRating());
                await saveTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed processing");
            }
        }

        public ITracker Resolve(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var result = articleTrackers.GetOrAdd(key, new Lazy<ITracker>(() => trackerFactory()));
            return result.Value;
        }
    }
}
