using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Config;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.SeekingAlpha.Api.Readers
{
    public class AlphaAuthentication : IAuthentication
    {
        private readonly IApplicationConfiguration configuration;

        private readonly ITrackedRetrieval reader;

        private readonly ILogger<AlphaAuthentication> logger;

        public AlphaAuthentication(ILogger<AlphaAuthentication> logger, IApplicationConfiguration configuration, ITrackedRetrieval reader)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Authenticate(CancellationToken token)
        {
            logger.LogInformation("Authenticating");
            var email = configuration.GetEnvironmentVariable("ALPHA_EMAIL");
            email = System.Web.HttpUtility.UrlEncode(email);
            var pass = configuration.GetEnvironmentVariable("ALPHA_PASS");
            pass = System.Web.HttpUtility.UrlEncode(pass);
            var loginData = $"id=headtabs_login&activity=footer_login&function=FooterBar.Login&user%5Bemail%5D={email}&user%5Bpassword%5D={pass}";
            reader.ResetCookies();
            await reader.Post(new Uri("https://seekingalpha.com/authentication/login"), loginData, token, Constants.Ajax).ConfigureAwait(false);
            return true;
        }
    }
}