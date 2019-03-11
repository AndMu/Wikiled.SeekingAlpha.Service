using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;
using Wikiled.SeekingAlpha.Api.Containers;
using Wikiled.SeekingAlpha.Api.Tests.Helpers;

namespace Wikiled.SeekingAlpha.Api.Tests.Acceptance
{
    [TestFixture]
    public class ArticleDataReaderTests
    {
        private IArticleDataReader instance;

        private IContainer container;

        [SetUp]
        public void SetUp()
        {
            var helper =  new NetworkHelper(new AlphaModule("Data", "AAPL"));
            container = helper.Container;
            instance = container.Resolve<IArticleDataReader>();
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public async Task Construct()
        {
            var article = new ArticleDefinition();
            article.Id = "4210510";
            article.Url = new Uri("https://seekingalpha.com/article/4210510-apple-price-matters");
            var tokenSource = new CancellationTokenSource(10000);
            var result = await instance.Read(article, tokenSource.Token).ConfigureAwait(false);
            Assert.AreEqual(6673, result.Content.Text.Length);
            Assert.Greater(result.Comments.Length, 10);
        }
    }
}
