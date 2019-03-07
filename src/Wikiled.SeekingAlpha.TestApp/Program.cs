using System;
using System.Net;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Wikiled.Common.Extensions;
using Wikiled.Common.Utilities.Modules;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Monitoring;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.News.Monitoring.Retriever;
using Wikiled.SeekingAlpha.Api.Containers;

namespace Wikiled.SeekingAlpha.TestApp
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
            var builder = new ContainerBuilder();
            builder.RegisterModule<MainModule>();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule(new AlphaModule("Articles", "AAPL", "AMD", "GOOG", "AAPL"));
            builder.RegisterModule(
                new RetrieverModule(new RetrieveConfiguration
                {
                    LongRetryDelay = 60 * 20,
                    CallDelay = 30000,
                    LongRetryCodes = new[] { HttpStatusCode.Forbidden, },
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

            var container = builder.Build();
            var loggerFactory = container.Resolve<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });

            var monitor = container.Resolve<IArticlesMonitor>();
            "Articles".EnsureDirectoryExistence();
            var persistency = container.Resolve<IArticlesPersistency>();
            monitor.Start().Subscribe(item => persistency.Save(item));
            monitor.Monitor().Subscribe(item => persistency.Save(item));
            Console.ReadLine();
        }
    }
}
