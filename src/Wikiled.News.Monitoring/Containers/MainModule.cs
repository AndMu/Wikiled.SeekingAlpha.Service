using Autofac;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Containers
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoggerFactory>().As<ILoggerFactory>()
                   .SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();
            builder.RegisterType<TrackedRetrieval>().As<ITrackedRetrieval>();
            builder.RegisterType<ArticleDataReader>().As<IArticleDataReader>();
            builder.Register(c => ConcurentManager.CreateDefault()).As<IConcurentManager>().SingleInstance();
        }
    }
}
