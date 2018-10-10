using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Polly;

namespace Wikiled.News.Monitoring.Readers
{
    public class HtmlReader : IHtmlReader
    {
        private readonly Policy policy;

        public HtmlReader()
        {
            policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4)
                });
        }

        public async Task<HtmlDocument> ReadDocument(Uri url)
        {
            HttpClient client = new HttpClient();
            var page = await policy.ExecuteAsync(ct => client.GetStringAsync(url), CancellationToken.None)
                .ConfigureAwait(false);
            var html = new HtmlDocument();
            html.LoadHtml(page);
            return html;
        }
    }
}
