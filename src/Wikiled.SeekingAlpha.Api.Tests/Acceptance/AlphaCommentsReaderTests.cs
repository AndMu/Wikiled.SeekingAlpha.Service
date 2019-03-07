using System.Threading.Tasks;
using NUnit.Framework;

namespace Wikiled.News.Monitoring.Tests.Acceptance.SeekingAlpha
{
    [TestFixture]
    public class AlphaCommentsReaderTests : BaseAlphaTests
    {
        [Test]
        public async Task ReadComments()
        {
            var comments = await Session.ReadComments(Article);
            Assert.Greater(comments.Length, 100);
        }
    }
}
