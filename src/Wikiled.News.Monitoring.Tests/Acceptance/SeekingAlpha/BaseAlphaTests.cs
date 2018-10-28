using System;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Common.Utilities.Config;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers.SeekingAlpha;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.News.Monitoring.Tests.Acceptance.SeekingAlpha
{
    [TestFixture]
    public abstract class BaseAlphaTests
    {
        public AlphaSessionReader Session { get; private set; }

        [SetUp]
        public void Init()
        {
            var article = new ArticleDefinition();
            article.Id = "4211146";
            article.Url = new Uri("https://seekingalpha.com/article/4210510-apple-price-matters");
            Session = new AlphaSessionReader(
                new NullLoggerFactory(),
                new ApplicationConfiguration(),
                new TrackedRetrieval(new NullLoggerFactory(), ConcurentManager.CreateDefault()),
                article);
        }
    }

}
