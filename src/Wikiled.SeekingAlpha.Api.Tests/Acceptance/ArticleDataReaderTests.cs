using Autofac;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;
using Wikiled.SeekingAlpha.Api.Tests.Helpers;

namespace Wikiled.SeekingAlpha.Api.Tests.Acceptance
{
    [TestFixture]
    public class ArticleDataReaderTests
    {
        private IArticleDataReader instance;

        private NetworkHelper helper;

        [SetUp]
        public void SetUp()
        {
            helper = new NetworkHelper();
            instance = helper.Container.Resolve<IArticleDataReader>();
        }

        [TearDown]
        public void TearDown()
        {
            helper.Dispose();
        }

        [Test]
        public async Task Construct()
        {
            var article = new ArticleDefinition
            {
                Id = "4210510",
                Url = new Uri("https://seekingalpha.com/article/4210510-apple-price-matters")
            };
            var tokenSource = new CancellationTokenSource(10000);
            Article result = await instance.Read(article, tokenSource.Token).ConfigureAwait(false);
            Assert.AreEqual(6673, result.Content.Text.Length);
            Assert.Greater(result.Comments.Length, 10);
        }
    }
}
