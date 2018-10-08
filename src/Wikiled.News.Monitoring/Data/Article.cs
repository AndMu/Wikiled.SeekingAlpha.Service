using System;

namespace Wikiled.News.Monitoring.Data
{
    public class Article
    {
        public Article(ArticleDefinition definition, CommentsData anonymous, CommentsData registered, ArticleText articleText, DateTime dateTime)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            ArticleText = articleText ?? throw new ArgumentNullException(nameof(articleText));
            DateTime = dateTime;
            Anonymous = anonymous ?? throw new ArgumentNullException(nameof(anonymous));
            Registered = registered ?? throw new ArgumentNullException(nameof(registered));
        }

        public DateTime DateTime { get; }

        public ArticleDefinition Definition { get; }

        public CommentsData Anonymous { get; }

        public CommentsData Registered { get; }

        public ArticleText ArticleText { get; }
    }
}
