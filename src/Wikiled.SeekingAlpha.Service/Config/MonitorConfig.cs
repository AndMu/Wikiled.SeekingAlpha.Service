using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.SeekingAlpha.Service.Config
{
    public class MonitorConfig
    {
        public RetrieveConfguration Service { get; set; }

        public string[] Stocks { get; set; }

        public string Location { get; set; }
    }
}
