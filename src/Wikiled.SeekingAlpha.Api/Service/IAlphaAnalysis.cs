﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.SeekingAlpha.Api.Request;
using Wikiled.Sentiment.Tracking.Logic;

namespace Wikiled.SeekingAlpha.Api.Service
{
    public interface IAlphaAnalysis
    {
        Task<TrackingResults> GetTrackingResults(SentimentRequest request, CancellationToken token);

        Task<RatingRecord[]> GetTrackingHistory(SentimentRequest request, int hours, CancellationToken token);

        Task<Dictionary<string, TrackingResults>> GetTrackingResults(SentimentRequest[] request, CancellationToken token);

        Task<Dictionary<string, RatingRecord[]>> GetTrackingHistory(SentimentRequest[] request, int hours, CancellationToken token);
    }
}
