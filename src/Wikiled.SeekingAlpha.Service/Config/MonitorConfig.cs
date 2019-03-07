using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.SeekingAlpha.Service.Config
{
    public class MonitorConfig
    {
        public RetrieveConfiguration Service { get; set; }

        public string[] Stocks { get; set; }

        public string Location { get; set; }
    }
}
