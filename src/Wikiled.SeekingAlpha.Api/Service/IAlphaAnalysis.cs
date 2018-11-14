using System.Threading;
using System.Threading.Tasks;
using Wikiled.MachineLearning.Mathematics.Tracking;

namespace Wikiled.SeekingAlpha.Api.Service
{
    public interface IAlphaAnalysis
    {
        Task<TrackingResults> GetTrackingResults(string keyword, CancellationToken token);
    }
}
