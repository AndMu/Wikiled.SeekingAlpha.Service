using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        private readonly ArticleDefinition article;

        private bool initialized;

        public AlphaSessionReader(ILoggerFactory loggerFactory, IApplicationConfiguration configuration, ITrackedRetrieval reader, ArticleDefinition article)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.article = article ?? throw new ArgumentNullException(nameof(article));
        }

        public ICommentsReader ReadComments()
        {
            return new AlphaCommentsReader(loggerFactory, article, reader);
        }

        public async Task<ArticleText> ReadArticle()
        {
            await Init().ConfigureAwait(false);
            return await new AlphaArticleTextReader(loggerFactory, reader).ReadArticle(article).ConfigureAwait(false);
        }

        private async Task Init()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            var email = configuration.GetEnvironmentVariable("ALPHA_EMAIL");
            email = System.Web.HttpUtility.UrlEncode(email);
            var pass = configuration.GetEnvironmentVariable("ALPHA_PASS");
            pass = System.Web.HttpUtility.UrlEncode(pass);
            string loginData = $"id=headtabs_login&activity=footer_login&function=FooterBar.Login&user%5Bemail%5D={email}&user%5Bpassword%5D={pass}";
            await reader.Authenticate(new Uri("https://seekingalpha.com/authentication/login"), loginData, Constants.Ajax);
        }
    }
}
