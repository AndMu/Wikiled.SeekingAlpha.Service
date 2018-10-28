using System.Reactive.Linq;
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
            var commentsReader = Session.ReadComments();
            var comments = await commentsReader.ReadAllComments().ToArray();
            Assert.Greater(comments.Length, 100);
        }
    }
}
