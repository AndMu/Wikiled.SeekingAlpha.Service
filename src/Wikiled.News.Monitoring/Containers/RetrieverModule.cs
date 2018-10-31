using System;
using Autofac;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Containers
{
    public class RetrieverModule : Module
    {
        private readonly RetrieveConfguration configuration;

        public RetrieverModule(RetrieveConfguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(configuration);
            builder.RegisterType<SimpleDataRetriever>().As<IDataRetriever>();
            builder.RegisterType<IPHandler>().As<IIPHandler>().SingleInstance();
            builder.RegisterType<ConcurrentManager>().As<IConcurentManager>().SingleInstance();
        }
    }
}
