using System;
using System.Net;

namespace Wikiled.News.Monitoring.Retriever
{
    public class ConcurentManager : IConcurentManager
    {
        private readonly IIPHandler factory;

        public ConcurentManager(IIPHandler factory)
        {
            this.factory = factory;
        }

        public static ConcurentManager CreateDefault()
        {
            return new ConcurentManager(new IPHandler(new[] { IPAddress.Any }, 2));
        }

        public void FinishedDownloading(Uri uri, IPAddress address)
        {
            factory.Release(address);
        }

        public IPAddress StartDownloading(Uri uri)
        {
            return factory.GetAvailable();
        }
    }
}
