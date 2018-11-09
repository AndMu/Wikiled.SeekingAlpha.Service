using Wikiled.MachineLearning.Mathematics.Tracking;
using Wikiled.News.Monitoring.Persistency;

namespace Wikiled.SeekingAlpha.Service.Logic.Tracking
{
    public interface ITrackingInstance : IArticlesPersistency
    {
        ITracker Resolve(string key);
    }
}