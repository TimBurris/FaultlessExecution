using System;
using System.Collections.Generic;
using System.Text;

namespace FaultlessExecution.Extensions
{
    public static class ExecutionResultExtensions
    {
        public static TExecutionResult OnException<TExecutionResult>(this TExecutionResult result, Action errorHandler)
            where TExecutionResult : Abstractions.ExecutionResult
        {
            if (result == null || result.WasSuccessful) return result;

            errorHandler.Invoke();

            return result;
        }

        public static TExecutionResult OnException<TExecutionResult>(this TExecutionResult result, Action<TExecutionResult> errorHandler)
          where TExecutionResult : Abstractions.ExecutionResult
        {
            if (result == null || result.WasSuccessful) return result;

            errorHandler.Invoke(result);

            return result;
        }

        public static TExecutionResult OnSuccess<TExecutionResult>(this TExecutionResult result, Action successHandler)
            where TExecutionResult : Abstractions.ExecutionResult
        {
            if (result == null || !result.WasSuccessful) return result;

            successHandler.Invoke();

            return result;
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
