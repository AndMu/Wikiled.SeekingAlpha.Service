using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Wikiled.SeekingAlpha.Api.Tests.Acceptance
{
    [TestFixture]
    public class AlphaCommentsReaderTests : BaseAlphaTests
    {
        [Test]
        public async Task ReadComments()
        {
            var tokenSource = new CancellationTokenSource(20000);
            var comments = await Readers.ReadComments(Article, tokenSource.Token).ConfigureAwait(false);
            Assert.Greater(comments.Length, 100);
        }
    }
}
