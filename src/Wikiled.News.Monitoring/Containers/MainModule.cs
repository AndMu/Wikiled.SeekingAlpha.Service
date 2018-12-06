using System.Reactive.Concurrency;
using Autofac;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Feeds;
using Wikiled.News.Monitoring.Monitoring;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Containers
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TrackedRetrieval>().As<ITrackedRetrieval>();
            builder.RegisterType<ArticleDataReader>().As<IArticleDataReader>();
            builder.RegisterType<ArticlesMonitor>().As<IArticlesMonitor>();
            builder.RegisterType<FeedsHandler>().As<IFeedsHandler>();

            builder.RegisterType<ArticlesPersistency>().As<IArticlesPersistency>();
            
        }
    }
}
