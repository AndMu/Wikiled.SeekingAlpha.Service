using System.Net;
using Autofac;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Modules;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Containers.Alpha;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Tests.Helpers
{
    public class NetworkHelper
    {
        public NetworkHelper(AlphaModule module = null)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule<MainModule>();
            if (module != null)
            {
                builder.RegisterModule(module);
            }

            builder.RegisterModule(
                new RetrieverModule(
                    new RetrieveConfguration
                {
                    LongRetryDelay = 1000,
                    CallDelay = 1000,
                    LongRetryCodes = new[] { HttpStatusCode.Forbidden },
                    RetryCodes = new[]
                    {
                        HttpStatusCode.Forbidden,
                        HttpStatusCode.RequestTimeout, // 408
                        HttpStatusCode.InternalServerError, // 500
                        HttpStatusCode.BadGateway, // 502
                        HttpStatusCode.ServiceUnavailable, // 503
                        HttpStatusCode.GatewayTimeout // 504
                    },
                    MaxConcurrent = 1
                }));

            builder.RegisterModule(new AlphaModule("Data", "AMD"));
            builder.RegisterModule(new LoggingModule(new LoggerFactory()));
            Container = builder.Build();
            Retrieval = Container.Resolve<ITrackedRetrieval>();
        }

        public  IContainer Container { get; }

        public ITrackedRetrieval Retrieval { get; }

    }
}
