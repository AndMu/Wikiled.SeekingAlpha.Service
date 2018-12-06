using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Net.Client;
using Wikiled.SeekingAlpha.Api.Request;
using Wikiled.SeekingAlpha.Api.Service;
using Wikiled.Server.Core.Testing.Server;

namespace Wikiled.SeekingAlpha.Service.Tests.Acceptance
{
    [TestFixture]
    public class AcceptanceTests
    {
        private ServerWrapper wrapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            wrapper = ServerWrapper.Create<Startup>(TestContext.CurrentContext.TestDirectory, services => { });
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            wrapper.Dispose();
        }

        [Test]
        public async Task Version()
        {
            ServiceResponse<RawResponse<string>> response = await wrapper.ApiClient.GetRequest<RawResponse<string>>("api/monitor/version", CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(response.IsSuccess);
        }

        [Test]
        public async Task GetTrackingResults()
        {
            AlphaAnalysis analysis = new AlphaAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            Sentiment.Tracking.Logic.TrackingResults result = await analysis.GetTrackingResults(new SentimentRequest("AMD", SentimentType.Article), CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual("AMD", result.Keyword);
            Assert.AreEqual(0, result.Total);
        }


        [Test]
        public async Task GetTrackingResultsPost()
        {
            AlphaAnalysis analysis = new AlphaAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            Sentiment.Tracking.Logic.TrackingResults result = await analysis.GetTrackingResults(new SentimentRequest("AMD", SentimentType.Article) { Steps = new[] { 100 } }, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual("AMD", result.Keyword);
            Assert.AreEqual(0, result.Sentiment["100H"].TotalMessages);
        }

        [Test]
        public async Task GetTrackingResultsAll()
        {
            AlphaAnalysis analysis = new AlphaAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            System.Collections.Generic.Dictionary<string, Sentiment.Tracking.Logic.TrackingResults> result = await analysis.GetTrackingResults(
                    new[]
                    {
                        new SentimentRequest("AMD", SentimentType.Article),
                        new SentimentRequest("TSLA", SentimentType.Article)
                    },
                    CancellationToken.None)
                .ConfigureAwait(false);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("AMD", result["AMD"].Keyword);
        }

        [Test]
        public async Task GetTrackingHistory()
        {
            AlphaAnalysis analysis = new AlphaAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            Sentiment.Tracking.Logic.RatingRecord[] result = await analysis.GetTrackingHistory(new SentimentRequest("AMD", SentimentType.Article), 10, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(0, result.Length);
        }
        [Test]
        public async Task GetTrackingHistoryAll()
        {
            AlphaAnalysis analysis = new AlphaAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            System.Collections.Generic.Dictionary<string, Sentiment.Tracking.Logic.RatingRecord[]> result = await analysis.GetTrackingHistory(
                new[]
                    {
                    new SentimentRequest("AMD", SentimentType.Article),
                        new SentimentRequest("TSLA", SentimentType.Article),
                        },
                    10,
                    CancellationToken.None)
                .ConfigureAwait(false);
            Assert.AreEqual(2, result.Count);
        }
    }
}
