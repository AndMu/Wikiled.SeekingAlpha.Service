using System;
using System.Text.RegularExpressions;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers.SeekingAlpha
{
    public class AlphaDefinitionTransformer : IDefinitionTransformer
    {
        public ArticleDefinition Transform(ArticleDefinition definition)
        {
            var original = definition.Id;
            definition.Id = Regex.Replace(definition.Id, "^.*:(.*)", "$1", RegexOptions.IgnoreCase);
            if (definition.Url.ToString().ToLower().Contains("news?"))
            {
                definition.Url = new Uri($"https://seekingalpha.com/news/{definition.Id}");
            }
            else if (definition.Url.ToString().ToLower().Contains("article/"))
            {
                definition.Url = new Uri($"https://seekingalpha.com/article/{definition.Id}");
            }

            return definition;
        }
    }
}
