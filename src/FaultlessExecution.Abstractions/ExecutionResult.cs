using System;
using System.Collections.Generic;
using System.Text;

namespace FaultlessExecution.Abstractions
{
    public abstract class ExecutionResult
    {
        public ExecutionResult(IFaultlessExecutionService executedBy)
        {
            this.ExecutedBy = executedBy;
        }
        public IFaultlessExecutionService ExecutedBy { get; }
        public bool WasSuccessful { get; set; }
        public Exception Exception { get; set; }
    }
}
