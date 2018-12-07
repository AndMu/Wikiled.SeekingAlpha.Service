using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Extensions;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Containers.Alpha;
using Wikiled.News.Monitoring.Monitoring;
using Wikiled.SeekingAlpha.Service.Config;
using Wikiled.SeekingAlpha.Service.Logic.Tracking;
using Wikiled.Sentiment.Tracking.Service;
using Wikiled.Server.Core.Helpers;

namespace Wikiled.SeekingAlpha.Service
{
    public class Startup : BaseStartup
    {
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        private readonly ILogger<Startup> logger;

        private MonitorConfig config;

        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment env)
            : base(loggerFactory, env)
        {
            logger = loggerFactory.CreateLogger<Startup>();
        }

        public override void Configure(IApplicationBuilder app,
                                       IHostingEnvironment env,
                                       IApplicationLifetime applicationLifetime)
        {
            base.Configure(app, env, applicationLifetime);
            applicationLifetime.ApplicationStopping.Register(OnShutdown, disposable);
        }


        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            config = services.RegisterConfiguration<MonitorConfig>(Configuration.GetSection("Monitor"));
            config.Location.EnsureDirectoryExistence();
            return base.ConfigureServices(services);
        }

        private void OnShutdown(object toDispose)
        {
            ((IDisposable)toDispose).Dispose();
        }

        protected override void ConfigureSpecific(ContainerBuilder builder)
        {
            builder.RegisterType<TrackingInstance>()
                .SingleInstance()
                .AutoActivate()
                .OnActivating(item =>
                {
                    logger.LogInformation("Starting monitoring");
                    var initial = item.Context.Resolve<IArticlesMonitor>()
                        .Start()
                        .Select(item.Instance.Save)
                        .Merge()
                        .Subscribe();
                    disposable.Add(initial);

                    var monitorArticles = item.Context.Resolve<IArticlesMonitor>()
                        .Monitor()
                        .Select(item.Instance.Save)
                        .Merge()
                        .Subscribe();
                    disposable.Add(monitorArticles);
                });
            builder.RegisterModule<MainModule>();
            builder.RegisterModule(new AlphaModule(config.Location, config.Stocks));
            builder.RegisterModule(new RetrieverModule(config.Service));
        }

        protected override string GetPersistencyLocation()
        {
            return config.Location;
        }
    }
}