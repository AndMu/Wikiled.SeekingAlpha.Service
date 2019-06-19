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
            var tokenSource = new CancellationTokenSource(200000);
            var article = await Readers.Read(Article, tokenSource.Token).ConfigureAwait(false);
            Assert.AreEqual("Apple: Price Matters", article.Definition.Title);
            Assert.AreEqual(6673, article.Content.Text.Length);
        }
    }
}
 