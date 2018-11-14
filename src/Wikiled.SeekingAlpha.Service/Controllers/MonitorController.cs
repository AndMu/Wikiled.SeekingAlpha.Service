using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wikiled.MachineLearning.Mathematics.Tracking;
using Wikiled.SeekingAlpha.Service.Logic.Tracking;
using Wikiled.Server.Core.ActionFilters;
using Wikiled.Server.Core.Controllers;

namespace Wikiled.SeekingAlpha.Service.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequestValidationAttribute))]
    public class MonitorController : BaseController
    {
        private readonly ITrackingInstance tracking;

        public MonitorController(ILoggerFactory loggerFactory, ITrackingInstance tracking)
            : base(loggerFactory)
        {
            this.tracking = tracking ?? throw new ArgumentNullException(nameof(tracking));
        }

        [Route("sentiment/{keyword}")]
        [HttpGet]
        public IActionResult GetResult(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                Logger.LogWarning("Empty keyword");
                return NoContent();
            }

            var tracker = tracking.Resolve(keyword);
            if (tracker == null)
            {
                Logger.LogWarning("Unknown keyword + " + keyword);
                return NotFound("Unknown keyword + " + keyword);
            }

            TrackingResults result = new TrackingResults
            {
                Keyword = keyword
            };
            int[] steps = { 24, 12, 6, 1 };
            foreach (int step in steps)
            {
                result.Sentiment[$"{step}H"] = new TrackingResult { Average = tracker.AverageSentiment(step), TotalMessages = tracker.Count(lastHours: step) };
            }

            result.Total = tracker.Count(false);
            return Ok(result);
        }
    }
}
