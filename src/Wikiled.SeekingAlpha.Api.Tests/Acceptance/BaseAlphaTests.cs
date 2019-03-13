using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;
using Wikiled.SeekingAlpha.Api.Tests.Helpers;

namespace Wikiled.SeekingAlpha.Api.Tests.Acceptance
{
    [TestFixture]
    public abstract class BaseAlphaTests
    {
        public NetworkHelper Helper { get; private set; }

        public ArticleDefinition Article { get; private set; }

        public IArticleDataReader Readers { get; private set; }

        [SetUp]
        public async Task Init()
        {
            Helper = new NetworkHelper();
            Article = new ArticleDefinition();
            Article.Id = "4211146";
            Article.Url = new Uri("https://seekingalpha.com/article/4210510-apple-price-matters");
            await Helper.Container.Resolve<IAuthentication>().Authenticate(CancellationToken.None).ConfigureAwait(false);
            Readers = Helper.Container.Resolve<IArticleDataReader>();
        }

        [TearDown]
        public void TearDown()
        {
            Helper.Dispose();
        }
    }
}
