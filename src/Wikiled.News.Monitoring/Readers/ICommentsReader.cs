using System;
using System.Threading.Tasks;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.News.Monitoring.Readers
{
    public interface ICommentsReader
    {
        int Total { get; }

        Task Init();

        IObservable<CommentData> ReadAllComments();
    }
}