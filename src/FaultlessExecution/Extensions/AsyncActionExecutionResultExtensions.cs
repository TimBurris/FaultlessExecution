using System;
using System.Threading.Tasks;

namespace FaultlessExecution.Extensions
{
    public static class AsyncActionExecutionResultExtensions
    {
        public static async Task<Abstractions.AsyncActionExecutionResult> RetryOnce(this Task<Abstractions.AsyncActionExecutionResult> task)
        {
            return await Retry(task, numberOfRetries: 1);
        }

        public static async Task<Abstractions.AsyncActionExecutionResult> Retry(this Task<Abstractions.AsyncActionExecutionResult> task, int numberOfRetries)
        {
            return await RetryIf(task,
                retryIfCode: (a) => true,
                numberOfRetries: numberOfRetries);
        }

        public static async Task<Abstractions.AsyncActionExecutionResult> RetryOnceIf(this Task<Abstractions.AsyncActionExecutionResult> task, Func<Abstractions.AsyncActionExecutionResult, bool> retryIfCode)
        {
            return await RetryIf(task, retryIfCode, numberOfRetries: 1);
        }

        public static async Task<Abstractions.AsyncActionExecutionResult> RetryIf(this Task<Abstractions.AsyncActionExecutionResult> task, Func<Abstractions.AsyncActionExecutionResult, bool> retryIfCode, int numberOfRetries)
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
