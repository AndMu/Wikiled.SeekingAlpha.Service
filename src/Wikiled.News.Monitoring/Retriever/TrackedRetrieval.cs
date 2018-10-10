using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Wikiled.News.Monitoring.Retriever
{
    public class TrackedRetrieval : ITrackedRetrieval
    {
        private readonly IConcurentManager manager;

        private CookieCollection collection;

        private readonly ILoggerFactory loggerFactory;

        public TrackedRetrieval(ILoggerFactory loggerFactory, IConcurentManager manager)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public async Task Authenticate(Uri uri, string data)
        {
            using (var retriever = new SimpleDataRetriever(loggerFactory, manager, uri))
            {
                retriever.AllCookies = new CookieCollection();
                retriever.AllowGlobalRedirection = true;
                await retriever.PostData(data).ConfigureAwait(false);
                collection = retriever.AllCookies;
            }
        }

        public async Task<HtmlDocument> Read(Uri uri)
        {
            using (var retriever = new SimpleDataRetriever(loggerFactory, manager, uri))
            {
                retriever.AllowGlobalRedirection = true;
                await retriever.ReceiveData().ConfigureAwait(false);
                retriever.AllCookies = collection;
                var document = new HtmlDocument();
                document.LoadHtml(retriever.Data);
                return document;
            }
        }

        public async Task ReadFile(Uri uri, Stream stream)
        {
            using (var retriever = new SimpleDataRetriever(loggerFactory, manager, uri))
            {
                retriever.AllCookies = collection;
                retriever.AllowGlobalRedirection = true;
                await retriever.ReceiveData(stream).ConfigureAwait(false);
                collection = retriever.AllCookies;
            }
        }
    }
}
