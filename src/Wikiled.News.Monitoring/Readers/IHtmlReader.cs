﻿using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Wikiled.News.Monitoring.Readers
{
    public interface IHtmlReader
    {
        Task<HtmlDocument> ReadDocument(string url);
    }
}