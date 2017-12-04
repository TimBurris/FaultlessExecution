using System;

namespace FaultlessExecution.Abstractions
{
    public class ActionExecutionResult : ExecutionResult
    {
        public ActionExecutionResult(IFaultlessExecutionService executedBy, Action executedCode)
            : base(executedBy)
        {
            this.ExecutedCode = executedCode;
        }
        public Action ExecutedCode { get; }

        public static ActionExecutionResult SuccessResult(IFaultlessExecutionService executedBy, Action executedCode)
        {
            return new ActionExecutionResult(executedBy, executedCode) { WasSuccessful = true };
        }

        public static ActionExecutionResult FailedResult(IFaultlessExecutionService executedBy, Action executedCode, Exception exception)
        {
            return new ActionExecutionResult(executedBy, executedCode)
            {
                Exception = exception,
            };
        }
    }
}
