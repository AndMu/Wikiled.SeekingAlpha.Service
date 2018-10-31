using System;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Common.Utilities.Config;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers.SeekingAlpha;
using Wikiled.News.Monitoring.Retriever;
using Wikiled.News.Monitoring.Tests.Helpers;

namespace Wikiled.News.Monitoring.Tests.Acceptance.SeekingAlpha
{
    [TestFixture]
    public abstract class BaseAlphaTests
    {
        public AlphaSessionReader Session { get; private set; }

        public NetworkHelper Helper { get; private set; }

        public ArticleDefinition Article { get; private set; }

        [SetUp]
        public void Init()
        {
            Helper = new NetworkHelper();
            Article = new ArticleDefinition();
            Article.Id = "4211146";
            Article.Url = new Uri("https://seekingalpha.com/article/4210510-apple-price-matters");
            Session = new AlphaSessionReader(
                new NullLoggerFactory(),
                new ApplicationConfiguration(),
                Helper.Retrieval);
        }

        [TearDown]
        public void TearDown()
        {
            Helper.Container.Dispose();
        }
    }

}
