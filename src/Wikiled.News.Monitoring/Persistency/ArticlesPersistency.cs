using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Persistency
{
    public class ArticlesPersistency
    {
        private ILogger<ArticlesPersistency> logger;

        private string path;

        private object syncRoot = new object();

        public ArticlesPersistency(ILogger<ArticlesPersistency> logger, string path)
        {
            this.logger = logger;
            this.path = path;
        }

        public void Save(Article article)
        {
            logger.LogInformation("Saving: {0}", article.Definition.Title);
            string output = JsonConvert.SerializeObject(article, Formatting.Indented);
            var file = Path.Combine(path, $"{article.Definition.Title}_{article.DateTime:yyyy-MM-dd}.json");
            lock (syncRoot)
            {
                File.WriteAllText(file, output);
            }
        }
    }
}
