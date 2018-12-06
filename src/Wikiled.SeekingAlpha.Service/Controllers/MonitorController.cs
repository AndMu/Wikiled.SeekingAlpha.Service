using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wikiled.Sentiment.Tracking.Logic;
using Wikiled.Server.Core.ActionFilters;
using Wikiled.Server.Core.Controllers;
using SentimentRequest = Wikiled.SeekingAlpha.Service.Logic.SentimentRequest;

namespace Wikiled.SeekingAlpha.Service.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequestValidationAttribute))]
    public class MonitorController : BaseController
    {
        private readonly ITrackingManager tracking;

        public MonitorController(ILoggerFactory loggerFactory, ITrackingManager tracking)
            : base(loggerFactory)
        {
            this.tracking = tracking ?? throw new ArgumentNullException(nameof(tracking));
        }

        [Route("sentimentex")]
        [HttpPost]
        public IActionResult GetResultEx([FromBody] SentimentRequest request)
        {
            return GetResult(request);
        }

        [Route("sentimentall")]
        [HttpPost]
        public IActionResult GetResultAll([FromBody] SentimentRequest[] requests)
        {
            Dictionary<string, TrackingResults> results = new Dictionary<string, TrackingResults>();
            foreach (var sentimentRequest in requests)
            {
                results[sentimentRequest.Name] = GetSingle(sentimentRequest);
            }

            return Ok(results);
        }

        [Route("sentiment/{type}/{name}")]
        [HttpGet]
        public IActionResult GetResult(SentimentRequest request)
        {
            if (string.IsNullOrEmpty(request?.Name))
            {
                Logger.LogWarning("Empty keyword");
                return NoContent();
            }

            return Ok(GetSingle(request));
        }

        [Route("history/{hours}/{type}/{name}")]
        [HttpGet]
        public IActionResult GetResultHistory(SentimentRequest request, int hours)
        {
            if (string.IsNullOrEmpty(request?.Name))
            {
                Logger.LogWarning("Empty keyword");
                return NoContent();
            }

            return Ok(GetSingleHistory(request, hours));
        }

        [Route("historyall/{hours}")]
        [HttpPost]
        public IActionResult GetResultHistoryAll([FromBody] SentimentRequest[] request, int hours)
        {
            Dictionary<string, RatingRecord[]> results = new Dictionary<string, RatingRecord[]>();
            foreach (var sentimentRequest in request)
            {
                results[sentimentRequest.Name] = GetSingleHistory(sentimentRequest, hours);
            }

            return Ok(results);
        }

        private RatingRecord[] GetSingleHistory(SentimentRequest request, int hours)
        {
            var tracker = tracking.Resolve(request.Name, request.Type.ToString());
            return tracker.GetRatings(hours).OrderByDescending(item => item.Date).ToArray();
        }

        private TrackingResults GetSingle(SentimentRequest request)
        {
            var tracker = tracking.Resolve(request.Name, request.Type.ToString());
            TrackingResults result = new TrackingResults { Keyword = tracker.Name, Type = tracker.Type };

            int[] steps = { 24, 12, 6, 1 };
            if (request.Steps != null)
            {
                steps = request.Steps;
            }

            foreach (int step in steps)
            {
                result.Sentiment[$"{step}H"] = new TrackingResult
                {
                    Average = tracker.CalculateAverageRating(step),
                    TotalMessages = tracker.Count(lastHours: step)
                };
            }

            result.Total = tracker.Count(false);
            return result;
        }
    }
}
