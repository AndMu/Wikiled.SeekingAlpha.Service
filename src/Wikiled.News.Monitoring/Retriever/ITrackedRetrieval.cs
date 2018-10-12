using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Wikiled.News.Monitoring.Retriever
{
    public interface ITrackedRetrieval
    {
        Task Authenticate(Uri uri, string data, Action<HttpWebRequest> modify = null);

        Task<string> Read(Uri uri, Action<HttpWebRequest> modify = null);

        Task ReadFile(Uri uri, Stream stream);
    }
}