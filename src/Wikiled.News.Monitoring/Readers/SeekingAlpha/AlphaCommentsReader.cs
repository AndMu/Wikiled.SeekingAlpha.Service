using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using Newtonsoft.Json;
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
                        string loginData =
                            $"id=headtabs_login&activity=footer_login&function=FooterBar.Login&user%5Bemail%5D={email}&user%5Bpassword%5D={pass}";
                        Action<HttpWebRequest> ajax = request =>
                        {
                            request.ContentType = "application/x-www-form-urlencoded";
                            request.Headers.Add("X-Prototype-Version", "1.7.1");
                            request.Headers.Add("Origin", "https://seekingalpha.com");
                            request.Referer = "https://seekingalpha.com/";
                            request.KeepAlive = true;
                            request.Accept = "text/javascript, text/html, application/xml, text/xml, */*";
                            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                        };

                        await reader.Authenticate(new Uri("https://seekingalpha.com/authentication/login"), loginData, ajax);
                        var data = await reader.Read(article.Url, ajax).ConfigureAwait(false);
                        var comments = JsonConvert.DeserializeObject<AlphaComments>(data);
                        //foreach (CommentData commentData in ParsePage(page))
                        //{
                        //    observer.OnNext(commentData);
                        //}

                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });
        }
    }
}
