using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Wikiled.Common.Utilities.Config;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Readers.SeekingAlpha
{
    public class AlphaCommentsReader : ICommentsReader
    {
        private readonly ILogger<AlphaCommentsReader> logger;

        private readonly ArticleDefinition article;

        private readonly ITrackedRetrieval reader;

        private readonly IApplicationConfiguration configuration;

        public AlphaCommentsReader(ILoggerFactory loggerFactory,
                                   ArticleDefinition article,
                                   ITrackedRetrieval reader,
                                   IApplicationConfiguration configuration)
        {
            this.article = article ?? throw new ArgumentNullException(nameof(article));
            logger = loggerFactory?.CreateLogger<AlphaCommentsReader>() ??
                throw new ArgumentNullException(nameof(logger));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
                        var email = configuration.GetEnvironmentVariable("ALPHA_EMAIL");
                        email = System.Web.HttpUtility.UrlEncode(email);
                        var pass = configuration.GetEnvironmentVariable("ALPHA_PASS");
                        pass = System.Web.HttpUtility.UrlEncode(pass);
                        string loginData = $"id=headtabs_login&activity=footer_login&function=FooterBar.Login&user%5Bemail%5D={email}&user%5Bpassword%5D={pass}";
                        await reader.Authenticate(new Uri("https://seekingalpha.com/authentication/login"), loginData);
                        HtmlDocument page = await reader.Read(article.Url).ConfigureAwait(false);
                        foreach (CommentData commentData in ParsePage(page))
                        {
                            observer.OnNext(commentData);
                        }

                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });
        }


        private IEnumerable<CommentData> ParsePage(HtmlDocument html)
        {
            HtmlNode doc = html.DocumentNode;
            IEnumerable<HtmlNode> comments = doc.QuerySelectorAll("section#comments");
            throw new NotImplementedException();
            foreach (HtmlNode htmlNode in comments)
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
