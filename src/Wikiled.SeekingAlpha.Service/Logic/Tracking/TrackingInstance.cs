using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.Sentiment.Api.Service;
using Wikiled.Sentiment.Tracking.Logic;

namespace Wikiled.SeekingAlpha.Service.Logic.Tracking
{
    public class TrackingInstance : IArticlesPersistency
    {
        private readonly ISentimentAnalysis sentiment;

        private readonly ILogger<TrackingInstance> logger;

        private readonly IArticlesPersistency persistency;

        private readonly ITrackingManager trackingManager;

        public TrackingInstance(ILogger<TrackingInstance> logger, ISentimentAnalysis sentiment, IArticlesPersistency persistency, ITrackingManager trackingManager)
        {
            this.sentiment = sentiment ?? throw new ArgumentNullException(nameof(sentiment));
            this.persistency = persistency ?? throw new ArgumentNullException(nameof(persistency));
            this.trackingManager = trackingManager;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Save(Article article)
        {
            try
            {
                logger.LogDebug("Saving: {0} {1}", article.Definition.Feed.Category, article.Definition.Id);
                var saveTask = Task.Run(() => persistency.Save(article));   
                var trackerArticle = trackingManager.Resolve(article.Definition.Feed.Category, "Article");
                var trackerComments = trackingManager.Resolve(article.Definition.Feed.Category, "Comment");
                Dictionary<string, (string Text, Action<double?> Rating)> texts = new Dictionary<string, (string Text, Action<double?> Rating)>();
                if (!trackerArticle.IsTracked(article.Definition.Id))
                {
                    logger.LogDebug("Tracking: {0}", article.Definition.Id);
                    var date = article.Definition.Date ?? DateTime.UtcNow;
                    texts[article.Definition.Id] = (article.ArticleText.Text, rating => trackerArticle.AddRating(new RatingRecord(article.Definition.Id, date, rating)));
                }

                logger.LogDebug("Total comments: {0}", article.Comments.Length);
                foreach (var comment in article.Comments)
                {
                    if (!trackerComments.IsTracked(comment.Id))
                    {
                        logger.LogDebug("Tracking: {0}", comment.Id);
                        texts[comment.Id] = (comment.Text, rating => trackerComments.AddRating(new RatingRecord(comment.Id, comment.Date, rating)));
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
                        result.Rating(tuple.Item2);
                    }
                }

                logger.LogDebug(
                    "Articles [{0}] Total: {1} with sentiment: {2} Average: {3}",
                    article.Definition.Feed.Category,
                    trackerArticle.Count(false),
                    trackerArticle.Count(),
                    trackerArticle.CalculateAverageRating());

                logger.LogDebug(
                    "Comments [{0}] Total: {1} with sentiment: {2} Average: {3}",
                    article.Definition.Feed.Category,
                    trackerArticle.Count(false),
                    trackerArticle.Count(),
                    trackerArticle.CalculateAverageRating());
                await saveTask.ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed processing");
            }

            return false;
        }
    }
}
