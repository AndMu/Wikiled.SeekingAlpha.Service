﻿using Autofac;
using System.Net;
using Wikiled.Common.Utilities.Modules;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Retriever;
using Wikiled.SeekingAlpha.Api.Containers;

namespace Wikiled.SeekingAlpha.Api.Tests.Helpers
{
    public class NetworkHelper
    {
        public NetworkHelper(AlphaModule module = null)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MainModule>();
            if (module != null)
            {
                builder.RegisterModule(module);
            }

            builder.RegisterModule(
                new RetrieverModule(
                    new RetrieveConfiguration
                    {
                        LongRetryDelay = 1000,
                        CallDelay = 100,
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
            builder.RegisterModule(new LoggingModule());
            Container = builder.Build();
            Retrieval = Container.Resolve<ITrackedRetrieval>();
        }

        public IContainer Container { get; }

        public ITrackedRetrieval Retrieval { get; }
    }
}