using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.MachineLearning.Mathematics.Tracking;
using Wikiled.News.Monitoring.Data;
using Wikiled.Sentiment.Api.Service;

namespace Wikiled.SeekingAlpha.Service.Logic.Tracking
{
    public class TrackingInstance : ITrackingInstance
    {
        private readonly ISentimentAnalysis sentiment;

        private readonly Func<ITracker> trackerFactory;

        private readonly ConcurrentDictionary<string, Lazy<ITracker>> articleTrackers = new ConcurrentDictionary<string, Lazy<ITracker>>(StringComparer.OrdinalIgnoreCase);

        private readonly ILogger<TrackingInstance> logger;

        public TrackingInstance(ILogger<TrackingInstance> logger, ISentimentAnalysis sentiment, Func<ITracker> trackerFactory)
        {
            this.sentiment = sentiment ?? throw new ArgumentNullException(nameof(sentiment));
            this.trackerFactory = trackerFactory;
            this.logger = logger;
        }

        public async Task Save(Article article)
        {
            try
            {
                var tracker = Resolve(article.Definition.Topic);
                if (!tracker.IsTracked(article.Definition.Id))
                {
                    var sentimentValue = await sentiment.Measure(article.ArticleText.Text).ConfigureAwait(false);
                    var date = article.Definition.Date ?? DateTime.UtcNow;
                    tracker.AddRating(new RatingRecord(article.Definition.Id, date, sentimentValue));
                }

                foreach (var comment in article.Comments)
                {
                    if (!tracker.IsTracked(comment.Id))
                    {
                        var sentimentValue = await sentiment.Measure(comment.Text).ConfigureAwait(false);
                        tracker.AddRating(new RatingRecord(comment.Id, comment.Date, sentimentValue));
                    }
                }
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
