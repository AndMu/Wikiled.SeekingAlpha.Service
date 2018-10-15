using Autofac;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Readers.SeekingAlpha;

namespace Wikiled.News.Monitoring.Containers.Alpha
{
    public class AlphaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AlphaCommentsReader>().As<ICommentsReader>();
        }
    }
}
