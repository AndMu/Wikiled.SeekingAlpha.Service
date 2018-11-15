using System;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Containers.Alpha;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Tests.Helpers;

namespace Wikiled.News.Monitoring.Tests.Acceptance.SeekingAlpha
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
            var result = await instance.Read(article);
            Assert.AreEqual(6673, result.ArticleText.Text.Length);
            Assert.Greater(result.Comments.Length, 10);
        }
    }
}
