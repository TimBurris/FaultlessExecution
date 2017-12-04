using FaultlessExecution.Abstractions;
using Moq;
using System;

namespace FaultlessExecutionTests
{
    public class TestExecutionResult : ExecutionResult
    {
        public Mock<IFaultlessExecutionService> _mockService;

        public TestExecutionResult(Mock<IFaultlessExecutionService> mockService)
            : base(mockService.Object)
        {
            _mockService = mockService;
        }
    }

}
