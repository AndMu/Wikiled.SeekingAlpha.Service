using System.Threading.Tasks;
using NUnit.Framework;

namespace Wikiled.News.Monitoring.Tests.Acceptance.SeekingAlpha
{
    [TestFixture]
    public class AlphaArticleTextReader : BaseAlphaTests
    {
        [Test]
        public async Task ReadArticle()
        {
            var article = await Session.ReadArticle().ConfigureAwait(false);
            Assert.AreEqual("Apple: Price Matters", article.Title);
            Assert.AreEqual(6673, article.Text.Length);
        }
    }
}
