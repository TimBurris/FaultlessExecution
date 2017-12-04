using System;
using System.Threading.Tasks;

namespace FaultlessExecution.Abstractions
{
    public class AsyncActionExecutionResult : ExecutionResult
    {
        public AsyncActionExecutionResult(IFaultlessExecutionService executedBy, Func<Task> executedCode)
            : base(executedBy)
        {
            this.ExecutedCode = executedCode;
        }
        public Func<Task> ExecutedCode { get; }

        public static AsyncActionExecutionResult SuccessResult(IFaultlessExecutionService executedBy, Func<Task> executedCode)
        {
            return new AsyncActionExecutionResult(executedBy, executedCode) { WasSuccessful = true };
        }

        public static AsyncActionExecutionResult FailedResult(IFaultlessExecutionService executedBy, Func<Task> executedCode, Exception exception)
        {
            return new AsyncActionExecutionResult(executedBy, executedCode)
            {
                Exception = exception,
            };
        }
    }
}
