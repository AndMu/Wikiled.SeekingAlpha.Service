﻿using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wikiled.Common.Extensions;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Persistency
{
    public class ArticlesPersistency
    {
        private readonly ILogger<ArticlesPersistency> logger;

        private readonly string path;

        private readonly object syncRoot = new object();

        public ArticlesPersistency(ILogger<ArticlesPersistency> logger, string path)
        {
            this.logger = logger;
            this.path = path;
        }

        public void Save(Article article)
        {
            logger.LogInformation("Saving: {0}", article.Definition.Title);
            string output = JsonConvert.SerializeObject(article, Formatting.Indented);
            var currentPath = Path.Combine(path, article.Definition.Feed.Category);
            var file = Path.Combine(currentPath, $"{article.Definition.Title.CreateLetterText()}_{article.DateTime:yyyy-MM-dd}.json");
            lock (syncRoot)
            {
                currentPath.EnsureDirectoryExistence();
                File.WriteAllText(file, output);
            }
        }
    }
}
