using System.Threading.Tasks;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers
{
    public interface ISessionReader
    {
        ICommentsReader ReadComments();

        Task<ArticleText> ReadArticle();
    }
}
