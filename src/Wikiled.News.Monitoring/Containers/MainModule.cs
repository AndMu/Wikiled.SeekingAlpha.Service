using Autofac;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Containers
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HtmlReader>().As<IHtmlReader>();
            builder.RegisterType<TrackedRetrieval>().As<ITrackedRetrieval>();
            builder.Register(c => ConcurentManager.CreateDefault()).As<IConcurentManager>().SingleInstance();
        }
    }
}
