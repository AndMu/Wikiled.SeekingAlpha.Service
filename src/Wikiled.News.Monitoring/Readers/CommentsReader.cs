using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers
{
    public class CommentsReader : ICommentsReader
    {
        private readonly ILogger<CommentsReader> logger;

        private int pageSize = 20;

        private readonly ArticleDefinition article;

        private bool isInit;

        private readonly IHtmlReader reader;

        public CommentsReader(ILoggerFactory loggerFactory, ArticleDefinition article, IHtmlReader reader)
        {
            this.article = article ?? throw new ArgumentNullException(nameof(article));
            logger = loggerFactory?.CreateLogger<CommentsReader>() ?? throw new ArgumentNullException(nameof(logger));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        private HtmlDocument firstPage;

        public int Total { get; private set; }

        public async Task Init()
        {
            throw new NotImplementedException();
            //firstPage = await reader.ReadDocument(GetUri(0));
            //var doc = firstPage.DocumentNode;
            //var commentsDefinition = doc.QuerySelector("div#comments-list");
            //Total = int.Parse(commentsDefinition.Attributes.First(a => a.Name == "data-count").Value);
            //isInit = true;
        }

        public IObservable<CommentData> ReadAllComments()
        {
            if (!isInit)
            {
                throw new InvalidOperationException("Reader is not initialized");
            }

            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            throw new NotImplementedException();

            //return Observable.Create<CommentData>(
            //    async observer =>
            //    {
            //        int totalPages = Total / pageSize;
            //        if (Total % pageSize > 0)
            //        {
            //            totalPages++;
            //        }

            //        try
            //        {
            //            var tasks = new List<Task<HtmlDocument>>();
            //            tasks.Add(Task.FromResult(firstPage));
            //            for (int i = 1; i < totalPages; i++)
            //            {
            //                tasks.Add(reader.ReadDocument(GetUri(i)));
            //            }

            //            foreach (var task in tasks)
            //            {
            //                var result = await task;
            //                foreach (var commentData in ParsePage(result))
            //                {
            //                    observer.OnNext(commentData);
            //                }
            //            }

            //            observer.OnCompleted();
            //        }
            //        catch (Exception ex)
            //        {
            //            observer.OnError(ex);
            //        }
            //    });
        }


        private IEnumerable<CommentData> ParsePage(HtmlDocument html)
        {
            var doc = html.DocumentNode;
            var comments = doc.QuerySelectorAll("div.comment-post");
            throw new NotImplementedException();
            foreach (var htmlNode in comments)
            {
                //CommentData record = new CommentData();
                //var author = htmlNode.QuerySelector("div.comment-author");
                //if (author == null)
                //{
                //    logger.LogDebug("Author not found: {0}", htmlNode.InnerText.Trim());
                //    continue;
                //}

                //record.Id = htmlNode.Attributes["data-post-id"].Value;
                //record.Author = author.InnerText.Trim();
                //var comment = htmlNode.QuerySelector("div.comment-content-inner");
                //if (comment == null)
                //{
                //    logger.LogDebug("Comment not found for {0}[{1}]", record.Id, record.Author);
                //    continue;
                //}

                //record.Text = comment.InnerText.Trim();
                //record.UpVote = int.Parse(htmlNode.QuerySelector("div.comment-votes-up").InnerText.Trim());
                //record.DownVote = int.Parse(htmlNode.QuerySelector("div.comment-votes-down").InnerText.Trim());
                //var dateIp = htmlNode.QuerySelector("div.comment-date").InnerText.Trim();
                //var ip = dateIp.IndexOf("IP:");
                //if (ip == -1)
                //{
                //    record.Date = DateTime.Parse(dateIp.Trim());
                //}
                //else
                //{
                //    record.Date = DateTime.Parse(dateIp.Substring(0, ip).Trim());
                //    record.Address = IPAddress.Parse(dateIp.Substring(ip + 3).Trim());
                //}

                //yield return record;
            }
        }

        //private string GetUri(int page)
        //{
        //    var builder = new UriBuilder(article.Url);
        //    var query = HttpUtility.ParseQueryString(builder.Query);
        //    query["com"] = "1";
        //    adjuster.AddParametes(query);
        //    query["no"] = (page * pageSize).ToString();
        //    builder.Query = query.ToString();
        //    return builder.ToString();
        //}
    }
}
