using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Feeds
{
    public class FeedsHandler : IFeedsHandler
    {
        private readonly FeedName[] feeds;

        public FeedsHandler(FeedName[] feeds)
        {
            this.feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));
        }

        public IObservable<ArticleDefinition> GetArticles()
        {
            return Observable.Create<ArticleDefinition>(
                async observer =>
                {
                    List<(FeedName Feed, Task<Feed> Task)> tasks = new List<(FeedName Feed, Task<Feed> Task)>();
                    foreach (var feed in feeds)
                    {
                        var task = FeedReader.ReadAsync(feed.Url);
                        tasks.Add((feed, task));
                    }

                    foreach (var task in tasks)
                    {
                        var result = await task.Task;
                        foreach (var item in result.Items)
                        {
                            ArticleDefinition article = new ArticleDefinition();
                            article.Url = new Uri(item.Link);
                            article.Id = item.Id;
                            article.Date = item.PublishingDate;
                            article.Title = item.Title;
                            article.Feed = task.Feed;
                            article.Element = item.SpecificItem.Element;
                            observer.OnNext(article);
                        }
                    }

                    observer.OnCompleted();
                });
        }
    }
}
