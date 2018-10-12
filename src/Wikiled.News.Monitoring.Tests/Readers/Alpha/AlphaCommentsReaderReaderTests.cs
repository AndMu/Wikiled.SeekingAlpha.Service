using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Common.Utilities.Config;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers.SeekingAlpha;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Tests.Readers.Alpha
{
    [TestFixture]
    public class AlphaCommentsReaderReaderTests
    {
        [TestCase]
        public async Task ReadComments()
        {
            ArticleDefinition article = new ArticleDefinition();
            // https://seekingalpha.com/article/4210510-apple-price-matters
            article.Url = new Uri("https://seekingalpha.com/account/ajax_get_comments?id=4211146&type=Article&commentType=");
            var reader = new AlphaCommentsReader(new NullLoggerFactory(),
                                                 article,
                                                 new TrackedRetrieval(new NullLoggerFactory(),
                                                                      ConcurentManager.CreateDefault()),
                                                 new ApplicationConfiguration());
            var comments = await reader.ReadAllComments().ToArray();
            var distinct = comments.Select(item => item.Id).Distinct();
        }
    }
}
