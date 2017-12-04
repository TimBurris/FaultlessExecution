using System;

namespace FaultlessExecution.Abstractions
{
    public class FuncExecutionResult<T> : ExecutionResult
    {
        public FuncExecutionResult(IFaultlessExecutionService executedBy, Func<T> executedCode)
            : base(executedBy)
        {
            this.ExecutedCode = executedCode;
        }

        public Func<T> ExecutedCode { get; }

        public T ReturnValue { get; set; }


        public static FuncExecutionResult<T> FailedResult(IFaultlessExecutionService executedBy, Func<T> executedCode, Exception exception)
        {
            return FuncExecutionResult<T>.FailedResult(executedBy, executedCode, exception, result: default(T));
        }
        public static FuncExecutionResult<T> FailedResult(IFaultlessExecutionService executedBy, Func<T> executedCode, Exception exception, T result)
        {
            return new FuncExecutionResult<T>(executedBy, executedCode)
            {
                Exception = exception,
                ReturnValue = result
            };
        }
        public static FuncExecutionResult<T> SuccessResult(IFaultlessExecutionService executedBy, Func<T> executedCode, T value)
        {
            return new FuncExecutionResult<T>(executedBy, executedCode) { ReturnValue = value, WasSuccessful = true };
        }
    }
}
