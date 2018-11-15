using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reflection;
using Wikiled.Common.Extensions;
using Wikiled.Common.Net.Client;
using Wikiled.MachineLearning.Mathematics.Tracking;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Containers.Alpha;
using Wikiled.News.Monitoring.Monitoring;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.SeekingAlpha.Service.Config;
using Wikiled.SeekingAlpha.Service.Logic.Tracking;
using Wikiled.Sentiment.Api.Request;
using Wikiled.Sentiment.Api.Service;
using Wikiled.Server.Core.Errors;
using Wikiled.Server.Core.Helpers;
using Wikiled.Server.Core.Middleware;

namespace Wikiled.SeekingAlpha.Service
{
    public class Startup
    {
        private readonly ILogger<Startup> logger;

        private readonly CompositeDisposable disposable = new CompositeDisposable();

        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Env = env;
            logger = loggerFactory.CreateLogger<Startup>();
            Configuration.ChangeNlog();
            logger.LogInformation($"Starting: {Assembly.GetExecutingAssembly().GetName().Version}");
        }

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }
            
            //app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseExceptionHandlingMiddleware();
            app.UseHttpStatusCodeExceptionMiddleware();
            app.UseRequestLogging();
            app.UseMvc();
            applicationLifetime.ApplicationStopping.Register(OnShutdown, disposable);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Needed to add this section, and....
            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        "CorsPolicy",
                        itemBuider => itemBuider.AllowAnyOrigin()
                                                .AllowAnyMethod()
                                                .AllowAnyHeader()
                                                .AllowCredentials());
                });

            // Add framework services.
            services.AddMvc(options => { });

            // needed to load configuration from appsettings.json
            services.AddOptions();

            MonitorConfig config = services.RegisterConfiguration<MonitorConfig>(Configuration.GetSection("Monitor"));
            SentimentConfig sentimentConfig = services.RegisterConfiguration<SentimentConfig>(Configuration.GetSection("sentiment"));

            // Create the container builder.
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule<MainModule>();
            builder.RegisterModule(new AlphaModule(config.Location, config.Stocks));
            builder.RegisterModule(new RetrieverModule(config.Service));;
            builder.RegisterType<SentimentAnalysis>().As<ISentimentAnalysis>();
            
            builder.Populate(services);
            SetupServices(builder, sentimentConfig);
            SetupTracking(builder);

            IContainer appContainer = builder.Build();
            IArticlesMonitor monitor = appContainer.Resolve<IArticlesMonitor>();
            config.Location.EnsureDirectoryExistence();
            IArticlesPersistency persistency = appContainer.Resolve<ITrackingInstance>();
            IDisposable start = monitor.Start().Subscribe(item => persistency.Save(item));
            IDisposable stop = monitor.Monitor().Subscribe(item => persistency.Save(item));
            disposable.Add(start);
            disposable.Add(stop);

            logger.LogInformation("Ready!");
            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(appContainer);
        }

        private void SetupTracking(ContainerBuilder builder)
        {
            builder.RegisterType<Tracker>().As<ITracker>();
            builder.RegisterType<TrackingInstance>().As<ITrackingInstance>();
            builder.RegisterInstance(new TrackingConfiguration(TimeSpan.FromHours(1), TimeSpan.FromDays(1)));
        }

        private void SetupServices(ContainerBuilder builder, SentimentConfig sentiment)
        {
            logger.LogInformation("Setting up services...");
            builder.Register(context => new StreamApiClientFactory(context.Resolve<ILoggerFactory>(),
                                                                   new HttpClient
                                                                   {
                                                                       Timeout = TimeSpan.FromMinutes(10)
                                                                   },
                                                                   new Uri(sentiment.Url)))
                .As<IStreamApiClientFactory>();
            var request = new WorkRequest();
            request.CleanText = true;
            request.Domain = sentiment.Domain;
            builder.RegisterInstance(request);
            builder.RegisterType<SentimentAnalysis>().As<ISentimentAnalysis>();
            logger.LogInformation("Register sentiment: {0} {1}", sentiment.Url, sentiment.Domain);
        }

        private void OnShutdown(object toDispose)
        {
            ((IDisposable)toDispose).Dispose();
        }
    }
}
