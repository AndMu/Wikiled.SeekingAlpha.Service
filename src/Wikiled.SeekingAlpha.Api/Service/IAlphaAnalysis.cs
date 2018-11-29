using System.Threading;
using System.Threading.Tasks;
using Wikiled.MachineLearning.Mathematics.Tracking;
using Wikiled.SeekingAlpha.Api.Request;

namespace Wikiled.SeekingAlpha.Api.Service
{
    public interface IAlphaAnalysis
    {
        Task<TrackingResults> GetTrackingResults(SentimentRequest request, CancellationToken token);

        Task<RatingRecord[]> GetTrackingHistory(SentimentRequest request, int hours, CancellationToken token);
    }
}
