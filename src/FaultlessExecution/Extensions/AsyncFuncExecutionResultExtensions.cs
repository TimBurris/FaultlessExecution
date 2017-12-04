using System;
using System.Threading.Tasks;

namespace FaultlessExecution.Extensions
{
    public static class AsyncFuncExecutionResultExtensions
    {
        public static async Task<Abstractions.AsyncFuncExecutionResult<T>> RetryOnce<T>(this Task<Abstractions.AsyncFuncExecutionResult<T>> task)
        {
            return await Retry(task, numberOfRetries: 1);
        }

        public static async Task<Abstractions.AsyncFuncExecutionResult<T>> Retry<T>(this Task<Abstractions.AsyncFuncExecutionResult<T>> task, int numberOfRetries)
        {
            return await RetryIf(task,
                retryIfCode: (a) => true,
                numberOfRetries: numberOfRetries);
        }

        public static async Task<Abstractions.AsyncFuncExecutionResult<T>> RetryOnceIf<T>(this Task<Abstractions.AsyncFuncExecutionResult<T>> task, Func<Abstractions.AsyncFuncExecutionResult<T>, bool> retryIfCode)
        {
            return await RetryIf(task, retryIfCode, numberOfRetries: 1);
        }

        public static async Task<Abstractions.AsyncFuncExecutionResult<T>> RetryIf<T>(this Task<Abstractions.AsyncFuncExecutionResult<T>> task, Func<Abstractions.AsyncFuncExecutionResult<T>, bool> retryIfCode, int numberOfRetries)
        {
            var result = await task;

            if (result == null || result.WasSuccessful) return result;

            int count = 0;
            while (count < numberOfRetries)
            {
                if (retryIfCode.Invoke(result))
                {
                    var newResult = await result.ExecutedBy.TryExecuteAsync(result.ExecutedCode);
                    if (newResult.WasSuccessful)
                        return newResult;
                }
                count++;
            }

            return result;
        }
    }

}
