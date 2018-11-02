using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Persistency
{
    public interface IArticlesPersistency
    {
        void Save(Article article);
    }
}