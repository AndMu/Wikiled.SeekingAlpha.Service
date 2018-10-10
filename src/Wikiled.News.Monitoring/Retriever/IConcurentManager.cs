using System;
using System.Net;

namespace Wikiled.News.Monitoring.Retriever
{
    public interface IConcurentManager
    {
        void FinishedDownloading(Uri uri, IPAddress address);

        IPAddress StartDownloading(Uri uri);
    }
}
