using System;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Net.Client;
using Wikiled.SeekingAlpha.Api.Request;
using Wikiled.Sentiment.Tracking.Logic;

namespace Wikiled.SeekingAlpha.Api.Service
{
    public class AlphaAnalysis : IAlphaAnalysis
    {
        private readonly IApiClient client;

        public AlphaAnalysis(IApiClientFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            client = factory.GetClient();
            client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<TrackingResults> GetTrackingResults(SentimentRequest request, CancellationToken token)
        {
            CheckArguments(request);
            var result = request.Steps?.Length > 0
                             ? await client.PostRequest<SentimentRequest, RawResponse<TrackingResults>>("api/monitor/sentimentex", request, token).ConfigureAwait(false)
                             : await client.GetRequest<RawResponse<TrackingResults>>($"api/monitor/sentiment/{request.Type}/{request.Name}", token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to retrieve:" + result.HttpResponseMessage);
            }

            return result.Result.Value;
        }

        public async Task<RatingRecord[]> GetTrackingHistory(SentimentRequest request, int hours, CancellationToken token)
        {
            CheckArguments(request);
            var result = await client.GetRequest<RawResponse<RatingRecord[]>>($"api/monitor/history/{hours}/{request.Type}/{request.Name}", token).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                throw new ApplicationException("Failed to retrieve:" + result.HttpResponseMessage);
            }

            return result.Result.Value;
        }

        private static void CheckArguments(SentimentRequest request)
        {
            if (request?.Name is null)
            {
                throw new ArgumentNullException(nameof(request.Name));
            }

            if (request?.Type is null)
            {
                throw new ArgumentNullException(nameof(request.Type));
            }
        }
    }
}
