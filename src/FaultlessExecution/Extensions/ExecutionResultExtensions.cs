using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FaultlessExecution.Extensions
{
    public static class ExecutionResultExtensions
    {
        public async static Task<TExecutionResult> OnException<TExecutionResult>(this Task<TExecutionResult> task, Action errorHandler)
            where TExecutionResult : Abstractions.ExecutionResult
        {
            var result = await task;

            return OnException(result, errorHandler);
        }

        public static TExecutionResult OnException<TExecutionResult>(this TExecutionResult result, Action errorHandler)
            where TExecutionResult : Abstractions.ExecutionResult
        {
            if (result == null || result.WasSuccessful) return result;

            errorHandler.Invoke();

            return result;
        }

        public async static Task<TExecutionResult> OnException<TExecutionResult>(this Task<TExecutionResult> task, Action<TExecutionResult> errorHandler)
            where TExecutionResult : Abstractions.ExecutionResult
        {
            var result = await task;

            return OnException(result, errorHandler);
        }


        public static TExecutionResult OnException<TExecutionResult>(this TExecutionResult result, Action<TExecutionResult> errorHandler)
          where TExecutionResult : Abstractions.ExecutionResult
        {
            if (result == null || result.WasSuccessful) return result;

            errorHandler.Invoke(result);

            return result;
        }

        public async static Task<TExecutionResult> OnSuccess<TExecutionResult>(this Task<TExecutionResult> task, Action successHandler)
           where TExecutionResult : Abstractions.ExecutionResult
        {
            var result = await task;

            return OnSuccess(result, successHandler);
        }

        public static TExecutionResult OnSuccess<TExecutionResult>(this TExecutionResult result, Action successHandler)
            where TExecutionResult : Abstractions.ExecutionResult
        {
            if (result == null || !result.WasSuccessful) return result;

            successHandler.Invoke();

            return result;
        }

        public async static Task<TExecutionResult> OnSuccess<TExecutionResult>(this Task<TExecutionResult> task, Action<TExecutionResult> successHandler)
            where TExecutionResult : Abstractions.ExecutionResult
        {
            var result = await task;

            return OnSuccess(result, successHandler);
        }


        public static TExecutionResult OnSuccess<TExecutionResult>(this TExecutionResult result, Action<TExecutionResult> successHandler)
            where TExecutionResult : Abstractions.ExecutionResult
        {
            if (result == null || !result.WasSuccessful) return result;

            successHandler.Invoke(result);

            return result;
        }
    }

}
