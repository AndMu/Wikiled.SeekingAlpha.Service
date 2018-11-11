using System;
using Autofac;
using NUnit.Framework;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Tests.Helpers;

namespace Wikiled.News.Monitoring.Tests.Acceptance.SeekingAlpha
{
    [TestFixture]
    public abstract class BaseAlphaTests
    {
        public ISessionReader Session { get; private set; }

        public NetworkHelper Helper { get; private set; }

        public ArticleDefinition Article { get; private set; }

        [SetUp]
        public void Init()
        {
            Helper = new NetworkHelper();
            Article = new ArticleDefinition();
            Article.Id = "4211146";
            Article.Url = new Uri("https://seekingalpha.com/article/4210510-apple-price-matters");
            Session = Helper.Container.Resolve<ISessionReader>();
        }

        [TearDown]
        public void TearDown()
        {
            Helper.Container.Dispose();
        }
    }

}
