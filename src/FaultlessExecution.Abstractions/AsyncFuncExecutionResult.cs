using System;
using System.Threading.Tasks;

namespace FaultlessExecution.Abstractions
{
    public class AsyncFuncExecutionResult<T> : ExecutionResult
    {
        public AsyncFuncExecutionResult(IFaultlessExecutionService executedBy, Func<Task<T>> executedCode)
            : base(executedBy)
        {
            this.ExecutedCode = executedCode;
        }
        public T ReturnValue { get; set; }

        public Func<Task<T>> ExecutedCode { get; }

        public static AsyncFuncExecutionResult<T> FailedResult(IFaultlessExecutionService executedBy, Func<Task<T>> executedCode, Exception exception)
        {
            return AsyncFuncExecutionResult<T>.FailedResult(executedBy, executedCode, exception, value: default(T));
        }
        public static AsyncFuncExecutionResult<T> FailedResult(IFaultlessExecutionService executedBy, Func<Task<T>> executedCode, Exception exception, T value)
        {
            return new AsyncFuncExecutionResult<T>(executedBy, executedCode)
            {
                Exception = exception,
                ReturnValue = value
            };
        }
        public static AsyncFuncExecutionResult<T> SuccessResult(IFaultlessExecutionService executedBy, Func<Task<T>> executedCode, T value)
        {
            return new AsyncFuncExecutionResult<T>(executedBy, executedCode) { ReturnValue = value, WasSuccessful = true };
        }
    }
}
