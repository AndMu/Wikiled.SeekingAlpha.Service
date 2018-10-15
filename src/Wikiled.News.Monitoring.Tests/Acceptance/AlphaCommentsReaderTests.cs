using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Wikiled.News.Monitoring.Tests.Acceptance
{
    [TestFixture]
    public class AlphaCommentsReaderTests : BaseAlphaTests
    {
        [Test]
        public async Task ReadComments()
        {
            var commentsReader = await Session.ReadComments().ConfigureAwait(false);
            var comments = await commentsReader.ReadAllComments().ToArray();
            Assert.Greater(comments.Length, 100);
        }
    }
}
