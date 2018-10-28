using Autofac;
using Wikiled.Common.Utilities.Config;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Readers.SeekingAlpha;

namespace Wikiled.News.Monitoring.Containers.Alpha
{
    public class AlphaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationConfiguration>().As<IApplicationConfiguration>();
            builder.RegisterType<AlphaSessionReader>().As<ISessionReader>();
            builder.RegisterType<AlphaCommentsReader>().As<ICommentsReader>();
            builder.RegisterType<AlphaArticleTextReader>().As<IArticleTextReader>();
        }
    }
}
