using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.News.Monitoring.Feeds;

namespace Wikiled.News.Monitoring.Tests.Feeds
{
    [TestFixture]
    public class FeedHandlerTests
    {
        private FeedsHandler instance;

        [SetUp]
        public void SetUp()
        {
            instance = CreateInstance();
        }

        [Test]
        public async Task GetArticles()
        {
            var result = await instance.GetArticles().ToArray();
            Assert.Greater(result.Length, 0);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new FeedsHandler(null));
        }

        private FeedsHandler CreateInstance()
        {
            FeedName feed = new FeedName();
            feed.Url = "https://seekingalpha.com/api/sa/combined/AAPL.xml";
            feed.Category = "AAPL";
            return new FeedsHandler(new[] { feed });
        }
    }
}
