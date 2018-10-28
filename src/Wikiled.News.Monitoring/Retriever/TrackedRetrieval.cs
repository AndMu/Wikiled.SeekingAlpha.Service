using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Wikiled.News.Monitoring.Retriever
{
    public class TrackedRetrieval : ITrackedRetrieval
    {
        private readonly IConcurentManager manager;

        private CookieCollection collection;

        private readonly ILoggerFactory loggerFactory;

        private readonly Policy policy;

        public TrackedRetrieval(ILoggerFactory loggerFactory, IConcurentManager manager)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            // Handle both exceptions and return values in one policy
            HttpStatusCode[] httpStatusCodesWorthRetrying = {
                                                                HttpStatusCode.RequestTimeout, // 408
                                                                HttpStatusCode.InternalServerError, // 500
                                                                HttpStatusCode.BadGateway, // 502
                                                                HttpStatusCode.ServiceUnavailable, // 503
                                                                HttpStatusCode.GatewayTimeout // 504
                                                            };
            policy = Policy
                     .Handle<WebException>(r => httpStatusCodesWorthRetrying.Contains(((HttpWebResponse)r.Response).StatusCode))
                     .WaitAndRetryAsync(new[]
                                        {
                                            TimeSpan.FromSeconds(1),
                                            TimeSpan.FromSeconds(2),
                                            TimeSpan.FromSeconds(4)
                                        });
        }

        public async Task Authenticate(Uri uri, string data, Action<HttpWebRequest> modify = null)
        {
            using (SimpleDataRetriever retriever = new SimpleDataRetriever(loggerFactory, manager, uri))
            {
                retriever.Modifier = modify;
                retriever.AllCookies = new CookieCollection();
                retriever.AllowGlobalRedirection = true;
                await policy.ExecuteAsync(() => retriever.PostData(data)).ConfigureAwait(false);
                collection = retriever.AllCookies;
            }
        }

        public async Task<string> Read(Uri uri, Action<HttpWebRequest> modify = null)
        {
            using (SimpleDataRetriever retriever = new SimpleDataRetriever(loggerFactory, manager, uri))
            {
                retriever.Modifier = modify;
                retriever.AllowGlobalRedirection = true;
                retriever.AllCookies = collection;
                await policy.ExecuteAsync(() => retriever.ReceiveData()).ConfigureAwait(false);
                return retriever.Data;
            }
        }

        public async Task ReadFile(Uri uri, Stream stream)
        {
            using (SimpleDataRetriever retriever = new SimpleDataRetriever(loggerFactory, manager, uri))
            {
                retriever.AllCookies = collection;
                retriever.AllowGlobalRedirection = true;
                await policy.ExecuteAsync(() => retriever.ReceiveData(stream)).ConfigureAwait(false);
                collection = retriever.AllCookies;
            }
        }
    }
}
