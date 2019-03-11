using System;
using System.Threading;
using Autofac;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Auth;
using Wikiled.Common.Utilities.Config;
using Wikiled.News.Monitoring.Feeds;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.News.Monitoring.Readers;
using Wikiled.SeekingAlpha.Api.Readers;

namespace Wikiled.SeekingAlpha.Api.Containers
{
    public class AlphaModule : Module
    {
        private readonly string[] stocks;

        private readonly string saveLocation;

        public AlphaModule(string saveLocation, params string[] stocks)
        {
            if (stocks == null)
            {
                throw new ArgumentNullException(nameof(stocks));
            }

            if (stocks.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(stocks));
            }

            this.saveLocation = saveLocation ?? throw new ArgumentNullException(nameof(saveLocation));
            this.stocks = stocks;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationConfiguration>().As<IApplicationConfiguration>();
            builder.RegisterType<AlphaAuthentication>().As<IAuthentication>();
            builder.RegisterType<AlphaCommentsReader>().As<ICommentsReader>();
            builder.RegisterType<AlphaArticleTextReader>().As<IArticleTextReader>();
            builder.RegisterType<AlphaDefinitionTransformer>().As<IDefinitionTransformer>();
            builder.Register(ctx => new ArticlesPersistency(ctx.Resolve<ILogger<ArticlesPersistency>>(), saveLocation)).As<IArticlesPersistency>();

            foreach (var stock in stocks)
            {
                var feed = new FeedName();
                feed.Url = $"https://seekingalpha.com/api/sa/combined/{stock}.xml";
                feed.Category = stock;
                builder.RegisterInstance(feed);
            }
        }
    }
}
