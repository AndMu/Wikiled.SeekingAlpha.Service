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
                Dictionary<string, (DateTime Date, string Text)> texts = new Dictionary<string, (DateTime Date, string Text)>();
                if (!tracker.IsTracked(article.Definition.Id))
                {
                    var date = article.Definition.Date ?? DateTime.UtcNow;
                    texts[article.Definition.Id] = (date, article.ArticleText.Text);
                }

                foreach (var comment in article.Comments)
                {
                    if (!tracker.IsTracked(comment.Id))
                    {
                        texts[article.Definition.Id] = (comment.Date, comment.Text);
                    }
                }

                if (texts.Count > 0)
                {
                    var request = texts.Select(item => (item.Key, item.Value.Text)).ToArray();
                    var sentimentValue = await sentiment.Measure(request, CancellationToken.None).ToArray();
                    foreach (var tuple in sentimentValue)
                    {
                        var result = texts[tuple.Item1];
                        tracker.AddRating(new RatingRecord(tuple.Item1, result.Date, tuple.Item2));
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
