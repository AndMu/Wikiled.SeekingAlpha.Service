using System.Net;

namespace Wikiled.News.Monitoring.Retriever
{
    public interface IIPHandler
    {
        IPAddress GetAvailable();

        void Release(IPAddress ipAddress);
    }
}
