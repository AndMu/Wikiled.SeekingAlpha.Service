using System.Threading.Tasks;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers
{
    public interface ISessionReader
    {
        Task<ICommentsReader> ReadComments();

        Task<ArticleText> ReadArticle();
    }
}
