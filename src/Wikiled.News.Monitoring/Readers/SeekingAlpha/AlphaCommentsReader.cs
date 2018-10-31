using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Readers.SeekingAlpha
{
    public class AlphaCommentsReader : ICommentsReader
    {
        private readonly ILogger<AlphaCommentsReader> logger;

        private readonly ArticleDefinition article;

        private readonly ITrackedRetrieval reader;

        public AlphaCommentsReader(ILoggerFactory loggerFactory, ArticleDefinition article, ITrackedRetrieval reader)
        {
            this.article = article ?? throw new ArgumentNullException(nameof(article));
            logger = loggerFactory?.CreateLogger<AlphaCommentsReader>() ??
                throw new ArgumentNullException(nameof(logger));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public IObservable<CommentData> ReadAllComments()
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            return Observable.Create<CommentData>(
                async observer =>
                {
                    try
                    {
                        var uri = new Uri($"https://seekingalpha.com/account/ajax_get_comments?id={article.Id}&type=Article&commentType=");
                        var data = await reader.Read(uri, Constants.Ajax).ConfigureAwait(false);
                        var comments = JsonConvert.DeserializeObject<AlphaComments>(data);
                        int total = 0;
                        foreach (CommentData commentData in Read(comments.Comments))
                        {
                            total++;
                            var document = new HtmlDocument();
                            document.LoadHtml(commentData.Text);
                            commentData.Text = document.DocumentNode.InnerText;
                            observer.OnNext(commentData);
                        }

                        if (total != comments.Total)
                        {
                            logger.LogWarning("Total mistmatch. Expected: {0} and received: {1}", comments.Total, total);
                        }

                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });
        }

        private IEnumerable<CommentData> Read(Dictionary<string, AlphaComment> data)
        {
            if (data == null)
            {
                yield break;
            }

            foreach (var comment in data)
            {
                CommentData result = new CommentData();
                result.Author = comment.Value.UserNick;
                result.AuthorId = comment.Value?.UserId.ToString();
                result.IsSpecialAuthor = comment.Value.IsPremiumAuthor;
                result.Date = comment.Value.CreatedOn;
                result.Id = comment.Value?.Id.ToString();
                result.Text = comment.Value.Content;
                result.Vote = comment.Value.Likes;
                yield return result;
                foreach (var children in Read(comment.Value.Children))
                {
                    yield return children;
                }    
            }
        }
    }
}
