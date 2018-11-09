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

        private readonly ConcurrentDictionary<string, ITracker> articleTrackers = new ConcurrentDictionary<string, ITracker>(StringComparer.OrdinalIgnoreCase);

        private readonly ILogger<TrackingInstance> logger;

        public TrackingInstance(ILogger<TrackingInstance> logger, ISentimentAnalysis sentiment, Func<ITracker> trackerFactory)
        {
            this.sentiment = sentiment ?? throw new ArgumentNullException(nameof(sentiment));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Save(Article article)
        {
            throw new NotImplementedException();
            try
            {
            //    article.Comments[0].
            //    var sentimentValue = await sentiment.Measure(tweet.Text).ConfigureAwait(false);
            //    var tweetItem = Tweet.GenerateTweetFromDTO(tweet);
            //    var saveTask = Task.Run(() => persistency?.Save(tweetItem, sentimentValue));
            //    foreach (var tracker in Trackers)
            //    {
            //        tracker.AddRating(tweet.Text, sentimentValue);
            //    }

            //    if (articleTrackers.TryGetValue(tweet.CreatedBy.Name, out var trackerUser))
            //    {
            //        trackerUser.AddRating(tweet.CreatedBy.Name, sentimentValue);
            //    }

            //    await saveTask.ConfigureAwait(false);
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

            articleTrackers.TryGetValue(key, out var value);
            return value;
        }
    }
}
