using System;
using Autofac;
using System.Net;
using Wikiled.Common.Utilities.Modules;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Retriever;
using Wikiled.SeekingAlpha.Api.Containers;

namespace Wikiled.SeekingAlpha.Api.Tests.Helpers
{
    public class NetworkHelper : IDisposable
    {
        public NetworkHelper()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<MainNewsModule>();
            builder.RegisterModule(
                new NewsRetrieverModule(
                    new RetrieveConfiguration
                    {
                        LongDelay = 60000,
                        ShortDelay = 1000,
                        CallDelay = 10000,
                        LongRetryCodes = new[] {HttpStatusCode.Forbidden},
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
            Container = builder.Build();
            Retrieval = Container.Resolve<ITrackedRetrieval>();
        }

        public IContainer Container { get; }

        public ITrackedRetrieval Retrieval { get; }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}
