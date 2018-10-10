using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;

namespace Wikiled.News.Monitoring.Tests.Readers
{
    [TestFixture]
    public class ArticleDataReaderTests
    {
        private Mock<ILoggerFactory> mockLoggerFactory;

        private Mock<ICommentsReaderFactory> mockCommentsReaderFactory;

        private Mock<IArticleTextReader> mockArticleTextReader;

        private ArticleDataReader instance;

        [SetUp]
        public void SetUp()
        {
            mockLoggerFactory = new Mock<ILoggerFactory>();
            mockCommentsReaderFactory = new Mock<ICommentsReaderFactory>();
            mockArticleTextReader = new Mock<IArticleTextReader>();
            instance = CreateInstance();
        }

        [Test]
        public async Task ReadAll()
        {
            Assert.Fail();
            //    var realReader =  new ArticleDataReader(
            //        new NullLoggerFactory(),
            //        new BaseCommentsReaderFactory(new NullLoggerFactory(), new HtmlReader()),
            //        new ArticleTextReader(new NullLoggerFactory(), new HtmlReader()));
            //    ArticleDefinition definition = new ArticleDefinition();
            //    definition.Url = new Uri("https://seekingalpha.com/article/4210510-apple-price-matters");
            //    var result = await realReader.Read(definition);
            //    Assert.IsNotNull(result);
            //    Assert.GreaterOrEqual(result.ArticleText.Text.Length, 100);
            //    Assert.GreaterOrEqual(result.ArticleText.Description.Length, 100);
            //    Assert.GreaterOrEqual(result.Comments.Length, 1000);
        }

        [Test]
        public void Construct()
        {
            Assert.Fail();
            //Assert.Throws<ArgumentNullException>(() => new ArticleDataReader(
            //    null,
            //    mockCommentsReaderFactory.Object,
            //    mockArticleTextReader.Object));
            //Assert.Throws<ArgumentNullException>(() => new ArticleDataReader(
            //    mockLoggerFactory.Object,
            //    null,
            //    mockArticleTextReader.Object));
            //Assert.Throws<ArgumentNullException>(() => new ArticleDataReader(
            //    mockLoggerFactory.Object,
            //    mockCommentsReaderFactory.Object,
            //    null));
        }

        private ArticleDataReader CreateInstance()
        {
            throw new NotImplementedException();
            //return new ArticleDataReader(
            //    mockLoggerFactory.Object,
            //    mockCommentsReaderFactory.Object,
            //    mockArticleTextReader.Object);
        }
    }
}
