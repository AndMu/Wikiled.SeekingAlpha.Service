using System;
using System.Reactive.Linq;
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
            var comments = await Readers.ReadComments(Article).Timeout(TimeSpan.FromSeconds(3)).ToArray();
            Assert.Greater(comments.Length, 100);
        }
    }
}
