using System;

namespace Wikiled.SeekingAlpha.Api.Request
{
    public class SentimentRequest
    {
        public SentimentRequest(string name, SentimentType type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
        }

        public SentimentType Type { get; }

        public string Name { get;  }
    }
}
