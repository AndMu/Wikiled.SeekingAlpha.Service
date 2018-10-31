using System;
using System.Net;
using System.Threading.Tasks;

namespace Wikiled.News.Monitoring.Retriever
{
    public class ConcurrentManager : IConcurentManager
    {
        private readonly IIPHandler factory;

        private readonly RetrieveConfguration config;

        public ConcurrentManager(IIPHandler factory, RetrieveConfguration config)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task FinishedDownloading(Uri uri, IPAddress address)
        {
            await Task.Delay(config.CallDelay).ConfigureAwait(false);
            factory.Release(address);
        }

        public async Task<IPAddress> StartDownloading(Uri uri)
        {
            var result = await factory.GetAvailable().ConfigureAwait(false);
            await Task.Delay(config.CallDelay).ConfigureAwait(false);
            return result;
        }
    }
}
