using Autofac;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Net;
using System.Reflection;
using Wikiled.Common.Extensions;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Containers.Alpha;
using Wikiled.News.Monitoring.Monitoring;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.TestApp
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule<MainModule>();
            builder.RegisterModule(new AlphaModule("AAPL", "AMD", "GOOG", "AAPL"));
            builder.RegisterModule(
                new RetrieverModule(new RetrieveConfguration
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


            IContainer container = builder.Build();
            ILoggerFactory loggerFactory = container.Resolve<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });

            IArticlesMonitor monitor = container.Resolve<IArticlesMonitor>();
            "Articles".EnsureDirectoryExistence();
            ArticlesPersistency persistency = new ArticlesPersistency(loggerFactory.CreateLogger<ArticlesPersistency>(), "Articles");
            monitor.Start().Subscribe(item => persistency.Save(item));
            monitor.Monitor().Subscribe(item => persistency.Save(item));
            Console.ReadLine();
        }
    }
}
