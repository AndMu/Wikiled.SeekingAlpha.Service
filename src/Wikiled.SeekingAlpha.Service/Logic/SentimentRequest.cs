using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Wikiled.SeekingAlpha.Service.Logic
{
    public class SentimentRequest
    {
        [FromRoute]
        [Required]
        public SentimentType Type { get; set; }

        [FromRoute]
        [Required]
        public string Name { get; set; }

        public int[] Steps { get; set; }
    }
}
