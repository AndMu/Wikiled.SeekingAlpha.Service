using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Wikiled.News.Monitoring.Retriever
{
    public interface IDataRetriever : IDisposable
    {
        CookieCollection AllCookies { get; set; }

        bool AllowGlobalRedirection { get; set; }

        ICredentials Credentials { get; set; }

        string Data { get; }

        Uri DocumentUri { get; }

        IPAddress Ip { get; }

        bool IsDispossed { get; }

        string Referer { get; set; }

        HttpWebRequest Request { get; }

        Uri ResponseUri { get; }

        bool Success { get; }

        int Timeout { get; set; }

        void Dispose();
        Task PostData(string postData, bool prepareCall = true);
        Task PostData(Tuple<string, string>[] parameters, bool prepareCall = true);
        Task ReceiveData(Stream stream = null);
    }
}