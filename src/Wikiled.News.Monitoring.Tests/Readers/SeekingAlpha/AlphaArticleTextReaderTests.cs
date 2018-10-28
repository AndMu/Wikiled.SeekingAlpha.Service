using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers.SeekingAlpha;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Tests.Readers.SeekingAlpha
{
    [TestFixture]
    public class AlphaArticleTextReaderTests
    {
        private Mock<ILoggerFactory> mockLoggerFactory;

        private Mock<ITrackedRetrieval> mockHtmlReader;

        private AlphaArticleTextReader instance;

        [SetUp]
        public void SetUp()
        {
            mockLoggerFactory = new Mock<ILoggerFactory>();
            mockHtmlReader = new Mock<ITrackedRetrieval>();
            var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "data.html"));
            mockHtmlReader.Setup(item => item.Read(It.IsAny<Uri>(), It.IsAny<Action<HttpWebRequest>>())).Returns(Task.FromResult(text));
            instance = CreateInstance();
        }

        [Test]
        public async Task ReadArticle()
        {
            ArticleDefinition definition = new ArticleDefinition();
            definition.Url = new Uri("https://seekingalpha.com/article/4210510-apple-price-matters");
            var text = await instance.ReadArticle(definition);
            Assert.IsNotNull(text);
            Assert.GreaterOrEqual(text.Text.Length, 100);
            Assert.AreEqual("Apple: Price Matters", text.Title);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new AlphaArticleTextReader(
                null,
                mockHtmlReader.Object));
            Assert.Throws<ArgumentNullException>(() => new AlphaArticleTextReader(
                mockLoggerFactory.Object,
                null));
        }

        private AlphaArticleTextReader CreateInstance()
        {
            return new AlphaArticleTextReader(new NullLoggerFactory(), mockHtmlReader.Object);
        }
    }
}
