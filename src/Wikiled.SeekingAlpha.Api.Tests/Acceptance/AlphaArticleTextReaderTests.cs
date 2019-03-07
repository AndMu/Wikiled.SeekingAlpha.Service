using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Wikiled.SeekingAlpha.Api.Tests.Acceptance
{
    [TestFixture]
    public class AlphaArticleTextReaderTests : BaseAlphaTests
    {
        [Test]
        public async Task ReadArticle()
        {
            var tokenSource = new CancellationTokenSource(1000);
            var article = await Session.ReadArticle(Article, tokenSource.Token).ConfigureAwait(false);
            Assert.AreEqual("Apple: Price Matters", article.Title);
            Assert.AreEqual(6673, article.Text.Length);
        }
    }
}
