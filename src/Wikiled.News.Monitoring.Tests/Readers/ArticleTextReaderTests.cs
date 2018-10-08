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
    public class ArticleTextReaderTests
    {
        private Mock<ILoggerFactory> mockLoggerFactory;

        private Mock<IHtmlReader> mockHtmlReader;

        private ArticleTextReader instance;

        [SetUp]
        public void SetUp()
        {
            mockLoggerFactory = new Mock<ILoggerFactory>();
            mockHtmlReader = new Mock<IHtmlReader>();
            instance = CreateInstance();
        }

        [Test]
        public async Task ReadArticle()
        {
            ArticleDefinition definition = new ArticleDefinition();
            definition.Url = new Uri("https://www.delfi.lt/veidai/zmones/petro-grazulio-dukreles-mama-paviesino-ju-susirasinejima-galite-suprasti-kaip-jauciausi.d?id=78390475");
            var text = await instance.ReadArticle(definition);
            Assert.IsNotNull(text);
            Assert.GreaterOrEqual(text.Text.Length, 100);
            Assert.GreaterOrEqual(text.Description.Length, 100);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ArticleTextReader(
                null,
                mockHtmlReader.Object));
            Assert.Throws<ArgumentNullException>(() => new ArticleTextReader(
                mockLoggerFactory.Object,
                null));
        }

        private ArticleTextReader CreateInstance()
        {
            return new ArticleTextReader(new NullLoggerFactory(), new HtmlReader());
        }
    }
}
