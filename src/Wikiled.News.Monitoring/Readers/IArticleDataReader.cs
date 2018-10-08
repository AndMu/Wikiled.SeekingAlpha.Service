﻿using System.Threading.Tasks;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers
{
    public interface IArticleDataReader
    {
        Task<Article> Read(ArticleDefinition definition);
    }
}