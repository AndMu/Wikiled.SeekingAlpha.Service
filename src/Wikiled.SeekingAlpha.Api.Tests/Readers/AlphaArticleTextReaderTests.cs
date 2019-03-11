using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Retriever;
using Wikiled.SeekingAlpha.Api.Readers;

namespace Wikiled.SeekingAlpha.Api.Tests.Readers
{
    [TestFixture]
    public class AlphaArticleTextReaderTests
    {
        private Mock<ITrackedRetrieval> mockHtmlReader;

        private AlphaArticleTextReader instance;

        [SetUp]
        public void SetUp()
        {
            mockHtmlReader = new Mock<ITrackedRetrieval>();
            var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "data.html"));
            mockHtmlReader.Setup(item => item.Read(It.IsAny<Uri>(), CancellationToken.None, It.IsAny<Action<HttpWebRequest>>())).Returns(Task.FromResult(text));
            instance = CreateInstance();
        }

        [Test]
        public async Task ReadArticle()
        {
            var definition = new ArticleDefinition();
            definition.Url = new Uri("https://seekingalpha.com/article/4210510-apple-price-matters");
            var text = await instance.ReadArticle(definition, CancellationToken.None).ConfigureAwait(false);
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
                new NullLogger<AlphaArticleTextReader>(),
                null));
        }

        private AlphaArticleTextReader CreateInstance()
        {
            return new AlphaArticleTextReader(new NullLogger<AlphaArticleTextReader>(), mockHtmlReader.Object);
        }
    }
}
