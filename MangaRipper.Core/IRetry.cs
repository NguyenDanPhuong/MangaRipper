using System;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public interface IRetry
    {
        Task<T> DoAsync<T>(Func<Task<T>> action, TimeSpan retryInterval, int maxAttemptCount = 3);
    }
}