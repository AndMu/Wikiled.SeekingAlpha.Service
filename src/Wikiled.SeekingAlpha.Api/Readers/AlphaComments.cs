using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wikiled.SeekingAlpha.Api.Readers
{
    public class AlphaComments
    {
        public Dictionary<string, AlphaComment> Comments { get; set; }

        [JsonProperty(PropertyName = "comments_count")]
        public Dictionary<string, int> CommentsCount { get; set; }

        public Dictionary<string, string> Images { get; set; }

        public string Status { get; set; }

        public int Total { get; set; }

        public string[] Trackers { get; set; }

        [JsonProperty(PropertyName = "user_likes")]
        public Dictionary<string, string> Likes { get; set; }
    }
}
