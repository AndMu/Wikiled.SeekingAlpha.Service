using System;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Wikiled.News.Monitoring.Retriever
{
    public interface ITrackedRetrieval
    {
        Task Authenticate(Uri uri, string data);

        Task<HtmlDocument> Read(Uri uri);

        Task ReadFile(Uri uri, Stream stream);
    }
}