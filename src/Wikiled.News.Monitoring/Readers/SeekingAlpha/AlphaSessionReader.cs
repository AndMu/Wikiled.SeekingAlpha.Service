using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Utilities.Config;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Readers.SeekingAlpha
{
    public class AlphaSessionReader : ISessionReader
    {
        private readonly IApplicationConfiguration configuration;

        private readonly ITrackedRetrieval reader;

        private readonly ILoggerFactory loggerFactory;

        private bool initialized;

        private readonly SemaphoreSlim calls = new SemaphoreSlim(1);

        public AlphaSessionReader(ILoggerFactory loggerFactory, IApplicationConfiguration configuration, ITrackedRetrieval reader)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

        }

        public ICommentsReader ReadComments(ArticleDefinition article)
        {
            return new AlphaCommentsReader(loggerFactory, article, reader);
        }

        public async Task<ArticleText> ReadArticle(ArticleDefinition article)
        {
            try
            {
                await calls.WaitAsync().ConfigureAwait(false);
                await Init().ConfigureAwait(false);
                var result = await new AlphaArticleTextReader(loggerFactory, reader).ReadArticle(article).ConfigureAwait(false);
                return result;
            }
            finally
            {
                calls.Release();
            }
        }

        private async Task Init()
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