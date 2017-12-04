using System;

namespace FaultlessExecution.Extensions
{
    public static class ActionExecutionResultExtensions
    {
        public static Abstractions.ActionExecutionResult RetryOnce(this Abstractions.ActionExecutionResult result)
        {
            return Retry(result, numberOfRetries: 1);
        }

        public static Abstractions.ActionExecutionResult Retry(this Abstractions.ActionExecutionResult result, int numberOfRetries)
        {
            return RetryIf(result,
                retryIfCode: (a) => true,
                numberOfRetries: numberOfRetries);
        }

        public static Abstractions.ActionExecutionResult RetryOnceIf(this Abstractions.ActionExecutionResult result, Func<Abstractions.ActionExecutionResult, bool> retryIfCode)
        {
            return RetryIf(result, retryIfCode, numberOfRetries: 1);
        }

        public static Abstractions.ActionExecutionResult RetryIf(this Abstractions.ActionExecutionResult result, Func<Abstractions.ActionExecutionResult, bool> retryIfCode, int numberOfRetries)
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
