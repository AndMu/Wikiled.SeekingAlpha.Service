using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Utilities.Config;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Readers.SeekingAlpha
{
    public class AlphaSessionReader : ISessionReader
    {
        private readonly ILogger<AlphaSessionReader> logger;

        private readonly IApplicationConfiguration configuration;

        private readonly ITrackedRetrieval reader;

        private readonly ILoggerFactory loggerFactory;

        private bool initialized;

        private readonly SemaphoreSlim calls = new SemaphoreSlim(1);

        private readonly RetrieveConfguration httpConfiguration;

        public AlphaSessionReader(ILoggerFactory loggerFactory, IApplicationConfiguration configuration, ITrackedRetrieval reader, RetrieveConfguration httpConfiguration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.httpConfiguration = httpConfiguration ?? throw new ArgumentNullException(nameof(httpConfiguration));
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger<AlphaSessionReader>();
        }

        public Task<CommentData[]> ReadComments(ArticleDefinition article)
        {
            return Caller(async () => await new AlphaCommentsReader(loggerFactory, article, reader).ReadAllComments().ToArray());
        }

        public Task<ArticleText> ReadArticle(ArticleDefinition article)
        {
            return Caller(() => new AlphaArticleTextReader(loggerFactory, reader).ReadArticle(article));
        }

        private async Task<T> Caller<T>(Func<Task<T>> logic)
        {
            if (!initialized)
            {
                throw new InvalidOperationException();
            }

            try
            {
                await calls.WaitAsync().ConfigureAwait(false);
                logger.LogDebug("Wait until calling...");
                await Task.Delay(httpConfiguration.CallDelay).ConfigureAwait(false);
                var result = await logic().ConfigureAwait(false);
                logger.LogDebug("Cooldown after calling...");
                await Task.Delay(httpConfiguration.CallDelay).ConfigureAwait(false);
                return result;
            }
            finally
            {
                calls.Release();
            }
        }

        public async Task Init()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            string email = configuration.GetEnvironmentVariable("ALPHA_EMAIL");
            email = System.Web.HttpUtility.UrlEncode(email);
            string pass = configuration.GetEnvironmentVariable("ALPHA_PASS");
            pass = System.Web.HttpUtility.UrlEncode(pass);
            string loginData = $"id=headtabs_login&activity=footer_login&function=FooterBar.Login&user%5Bemail%5D={email}&user%5Bpassword%5D={pass}";
            await reader.Authenticate(new Uri("https://seekingalpha.com/authentication/login"), loginData, Constants.Ajax);
        }
    }
}