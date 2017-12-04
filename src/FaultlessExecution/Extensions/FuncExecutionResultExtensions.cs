using System;

namespace FaultlessExecution.Extensions
{
    public static class FuncExecutionResultExtensions
    {
        public static Abstractions.FuncExecutionResult<T> RetryOnce<T>(this Abstractions.FuncExecutionResult<T> result)
        {
            return Retry(result, numberOfRetries: 1);
        }

        public static Abstractions.FuncExecutionResult<T> Retry<T>(this Abstractions.FuncExecutionResult<T> result, int numberOfRetries)
        {
            return RetryIf(result,
                retryIfCode: (a) => true,
                numberOfRetries: numberOfRetries);
        }

        public static Abstractions.FuncExecutionResult<T> RetryOnceIf<T>(this Abstractions.FuncExecutionResult<T> result, Func<Abstractions.FuncExecutionResult<T>, bool> retryIfCode)
        {
            return RetryIf(result, retryIfCode, numberOfRetries: 1);
        }

        public static Abstractions.FuncExecutionResult<T> RetryIf<T>(this Abstractions.FuncExecutionResult<T> result, Func<Abstractions.FuncExecutionResult<T>, bool> retryIfCode, int numberOfRetries)
        {

            if (result == null || result.WasSuccessful) return result;
            int count = 0;
            while (count < numberOfRetries)
            {
                if (retryIfCode.Invoke(result))
                {
                    var newResult = result.ExecutedBy.TryExecute(result.ExecutedCode);
                    if (newResult.WasSuccessful)
                        return newResult;
                }
                count++;
            }

            return result;
        }
    }

}
