using System;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Containers.Alpha;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;

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
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule<AlphaModule>();
            builder.RegisterModule<MainModule>();
            container = builder.Build();
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
