using System;
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

        [Route("sentiment/{type}/{name}")]
        [HttpGet]
        public IActionResult GetResult(SentimentRequest request)
        {
            if (string.IsNullOrEmpty(request?.Name))
            {
                Logger.LogWarning("Empty keyword");
                return NoContent();
            }

            var tracker = tracking.Resolve(request.Name, request.Type.ToString());
            TrackingResults result = new TrackingResults
            {
                Keyword = tracker.Name,
                Type =  tracker.Type
            };

            int[] steps = { 24, 12, 6, 1 };
            foreach (int step in steps)
            {
                result.Sentiment[$"{step}H"] = new TrackingResult { Average = tracker.CalculateAverageRating(step), TotalMessages = tracker.Count(lastHours: step) };
            }

            result.Total = tracker.Count(false);
            return Ok(result);
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

            var tracker = tracking.Resolve(request.Name, request.Type.ToString());
            return Ok(tracker.GetRatings(hours).OrderByDescending(item => item.Date));
        }
    }
}
