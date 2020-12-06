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
    public class AsyncFuncExecutionResultExtensionTests
    {
        private FaultlessExecutionService _service;
        private int _numberOfTimesActionCodeRan;
        private int _numberOfTimesIfCodeRan;
        private Func<AsyncFuncExecutionResult<string>, bool> _ifcodeTrue;
        private Func<AsyncFuncExecutionResult<string>, bool> _ifcodeFalse;

        private Func<Task<string>> _successfulActionCode;
        private Func<Task<string>> _failActionCode;

        [TestInitialize]
        public void Init()
        {
            _service = new FaultlessExecutionService(logger: null);
            _successfulActionCode = async () => { _numberOfTimesActionCodeRan++; await Task.Delay(1); return "yay, i ran"; };
            _failActionCode = () => { _numberOfTimesActionCodeRan++; throw new ApplicationException(); };

            _ifcodeTrue = (a) => { _numberOfTimesIfCodeRan++; return true; };
            _ifcodeFalse = (a) => { _numberOfTimesIfCodeRan++; return false; };
        }

        [TestMethod]
        public void RetryOnce_does_not_retry_when_result_succeeds()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecuteAsync(_successfulActionCode)
                 .RetryOnce()
                 .Result;

            //*************  assert   ******************
            _numberOfTimesActionCodeRan.Should().Be(1);
        }

        [TestMethod]
        public void RetryOnce_does_retry_when_result_fails()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecuteAsync(_failActionCode)
                 .RetryOnce()
                 .Result;

            //*************  assert   ******************
            _numberOfTimesActionCodeRan.Should().Be(2);
        }


        [TestMethod]
        public void RetryIf_does_not_execute_if_when_successful()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecuteAsync(_successfulActionCode)
                 .RetryOnceIf(_ifcodeTrue)
                 .Result;

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(0);
            _numberOfTimesActionCodeRan.Should().Be(1);

        }

        [TestMethod]
        public void RetryIf_does_not_retry_when_if_false()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecuteAsync(_failActionCode)
                 .RetryOnceIf(_ifcodeFalse)
                 .Result;

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(1);
            _numberOfTimesActionCodeRan.Should().Be(1);
        }

        [TestMethod]
        public void RetryIf_does_retry_when_if_true()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecuteAsync(_failActionCode)
                 .RetryOnceIf(_ifcodeTrue)
                 .Result;

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(1);
            _numberOfTimesActionCodeRan.Should().Be(2);

        }

        [TestMethod]
        public void RetryIf_retries_until_number_of_times_reached()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecuteAsync(_failActionCode)
                 .RetryIf(_ifcodeTrue, numberOfRetries: 10)
                 .Result;

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(10);
            _numberOfTimesActionCodeRan.Should().Be(11);

        }

        [TestMethod]
        public void RetryIf_retries_until_success()
        {
            //*************  arrange  ******************
            Func<Task<string>> actionCode = async () =>
            {
                _numberOfTimesActionCodeRan++;
                if (_numberOfTimesActionCodeRan < 4)
                    throw new ApplicationException();

                await Task.Delay(1);
                return "yay, we ran";
            };

            //*************    act    ******************
            var result = _service.TryExecuteAsync(actionCode)
                 .RetryIf(_ifcodeTrue, numberOfRetries: 10)
                 .Result;

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(3);
            _numberOfTimesActionCodeRan.Should().Be(4);
            result.WasSuccessful.Should().BeTrue();
        }
    }
}
