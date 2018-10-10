using System.Collections.Concurrent;
using System.Net;

namespace Wikiled.News.Monitoring.Retriever
{
    public class IPHandler : IIPHandler
    {
        private readonly BlockingCollection<IPAddress> addressed = new BlockingCollection<IPAddress>();

        public IPHandler(IPAddress[] ips, int maxPerIp)
        {
            for (int i = 0; i < maxPerIp; i++)
            {
                foreach (var ip in ips)
                {
                    addressed.Add(ip);
                }
            }
        }

        public IPAddress GetAvailable()
        {
            return addressed.Take();
        }

        public void Release(IPAddress ipAddress)
        {
            addressed.Add(ipAddress);
        }
    }
}
