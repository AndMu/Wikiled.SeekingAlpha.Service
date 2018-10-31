using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Wikiled.News.Monitoring.Retriever
{
    public class IPHandler : IIPHandler
    {
        private readonly SemaphoreSlim semaphore;

        private readonly ConcurrentQueue<IPAddress> addressed = new ConcurrentQueue<IPAddress>();

        public IPHandler(IPAddress[] ips, RetrieveConfguration config)
        {
            if (ips == null)
            {
                throw new ArgumentNullException(nameof(ips));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

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
            for (; ; )
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                if (addressed.TryDequeue(out IPAddress item))
                {
                    return item;
                }
            }
        }

        public void Release(IPAddress ipAddress)
        {
            addressed.Enqueue(ipAddress);
            semaphore.Release();
        }
    }
}
