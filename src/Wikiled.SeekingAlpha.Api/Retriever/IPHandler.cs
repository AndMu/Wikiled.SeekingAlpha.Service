using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Wikiled.News.Monitoring.Retriever
{
    public class IPHandler : IIPHandler
    {
        private readonly ILogger<IPHandler> logger;

        private readonly SemaphoreSlim semaphore;

        private readonly ConcurrentQueue<IPAddress> addressed = new ConcurrentQueue<IPAddress>();

        public IPHandler(ILogger<IPHandler> logger, IPAddress[] ips, RetrieveConfguration config)
        {
            if (ips == null)
            {
                throw new ArgumentNullException(nameof(ips));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            for (int i = 0; i < config.MaxConcurrent; i++)
            {
                foreach (IPAddress ip in ips)
                {
                    addressed.Enqueue(ip);
                }
            }

            semaphore = new SemaphoreSlim(addressed.Count);
        }

        public async Task<IPAddress> GetAvailable()
        {
            logger.LogDebug("GetAvailable");
            for (; ; )
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                if (addressed.TryDequeue(out IPAddress item))
                {
                    logger.LogDebug("GetAvailable - DONE");
                    return item;
                }
            }
        }

        public void Release(IPAddress ipAddress)
        {
            logger.LogDebug("Release");
            addressed.Enqueue(ipAddress);
            semaphore.Release();
        }
    }
}
