using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FaultlessExecution;
using FaultlessExecution.Abstractions;
using FaultlessExecution.Extensions;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;

namespace FaultlessExecutionTests.Extensions
{
    [TestClass]
    public class ExecutionResultExtensionTests
    {
        private TestExecutionResult _executionResult;

        [TestInitialize]
        public void Init()
        {
            _executionResult = new TestExecutionResult(new Mock<IFaultlessExecutionService>());
        }

        [TestMethod]
        public void OnException_does_executes_if_result_not_successful()
        {

            //*************  arrange  ******************
            bool didRun = false;
            _executionResult.WasSuccessful = false;

            //*************    act    ******************
            _executionResult
                .OnException(() => didRun = true);

            //*************  assert   ******************
            didRun.Should().BeTrue();

        }

        [TestMethod]
        public void OnException_does_not_execute_if_result_successful()
        {

            //*************  arrange  ******************
            bool didRun = false;
            _executionResult.WasSuccessful = true;

            //*************    act    ******************
            _executionResult
                .OnException((r) => didRun = true);

            //*************  assert   ******************
            didRun.Should().BeFalse();

        }

        [TestMethod]
        public void OnSuccess_does_executes_if_result_successful()
        {

            //*************  arrange  ******************
            bool didRun = false;
            _executionResult.WasSuccessful = true;

            //*************    act    ******************
            _executionResult
                .OnSuccess(() => didRun = true);

            //*************  assert   ******************
            didRun.Should().BeTrue();

        }

        [TestMethod]
        public void OnSuccess_does_not_execute_if_result_not_successful()
        {

            //*************  arrange  ******************
            bool didRun = false;
            _executionResult.WasSuccessful = false;

            //*************    act    ******************
            _executionResult
                .OnSuccess(() => didRun = true);

            //*************  assert   ******************
            didRun.Should().BeFalse();

        }
    }
}
