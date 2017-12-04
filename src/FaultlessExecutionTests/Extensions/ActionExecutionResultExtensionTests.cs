﻿using System;
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
    public class ActionExecutionResultExtensionTests
    {
        private FaultlessExecutionService _service;
        private int _numberOfTimesActionCodeRan;
        private int _numberOfTimesIfCodeRan;
        private Func<ActionExecutionResult, bool> _ifcodeTrue;
        private Func<ActionExecutionResult, bool> _ifcodeFalse;

        private Action _successfulActionCode;
        private Action _failActionCode;

        [TestInitialize]
        public void Init()
        {
            _service = new FaultlessExecutionService();
            _successfulActionCode = () => _numberOfTimesActionCodeRan++;
            _failActionCode = () => { _numberOfTimesActionCodeRan++; throw new ApplicationException(); };

            _ifcodeTrue = (a) => { _numberOfTimesIfCodeRan++; return true; };
            _ifcodeFalse = (a) => { _numberOfTimesIfCodeRan++; return false; };
        }


        [TestMethod]
        public void RetryOnce_does_not_retry_when_result_succeeds()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecute(_successfulActionCode)
                            .RetryOnce();

            //*************  assert   ******************
            _numberOfTimesActionCodeRan.Should().Be(1);
        }

        [TestMethod]
        public void Chained_OnSuccess_fires_if_retry_succeeds()
        {
            //*************  arrange  ******************
            bool onExceptionFired = false;
            bool onSuccessFired = false;

            int numTimesActionFired = 0;

            Action actionCode = () =>
            {
                numTimesActionFired++;

                if (numTimesActionFired == 1)
                    throw new ApplicationException();
            };

            //*************    act    ******************
            var result = _service.TryExecute(actionCode)
                                .RetryOnce()
                                .OnException(() => onExceptionFired = true)
                                .OnSuccess(() => onSuccessFired = true);

            //*************  assert   ******************
            onSuccessFired.Should().BeTrue();
            onExceptionFired.Should().BeFalse();
            numTimesActionFired.Should().Be(2);
        }

        [TestMethod]
        public void Chained_OnException_fires_if_retry_fails()
        {
            //*************  arrange  ******************
            bool onExceptionFired = false;
            bool onSuccessFired = false;

            int numTimesActionFired = 0;

            Action actionCode = () =>
            {
                numTimesActionFired++;

                throw new ApplicationException();
            };

            //*************    act    ******************
            var result = _service.TryExecute(actionCode)
                                .RetryOnce()
                                .OnException(() => onExceptionFired = true)
                                .OnSuccess(() => onSuccessFired = true);

            //*************  assert   ******************
            onSuccessFired.Should().BeFalse();
            onExceptionFired.Should().BeTrue();
            numTimesActionFired.Should().Be(2);
        }

        [TestMethod]
        public void RetryOnce_does_retry_when_result_fails()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecute(_failActionCode)
                            .RetryOnce();

            //*************  assert   ******************
            _numberOfTimesActionCodeRan.Should().Be(2);
        }

        [TestMethod]
        public void RetryIf_does_not_execute_if_when_successful()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecute(_successfulActionCode)
                .RetryOnceIf(_ifcodeTrue);

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(0);
            _numberOfTimesActionCodeRan.Should().Be(1);

        }

        [TestMethod]
        public void RetryIf_does_not_retry_when_if_false()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecute(_failActionCode)
                            .RetryOnceIf(_ifcodeFalse);

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(1);
            _numberOfTimesActionCodeRan.Should().Be(1);
        }

        [TestMethod]
        public void RetryIf_does_retry_when_if_true()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecute(_failActionCode)
                            .RetryOnceIf(_ifcodeTrue);

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(1);
            _numberOfTimesActionCodeRan.Should().Be(2);

        }

        [TestMethod]
        public void RetryIf_retries_until_number_of_times_reached()
        {
            //*************  arrange  ******************

            //*************    act    ******************
            var result = _service.TryExecute(_failActionCode)
                            .RetryIf(_ifcodeTrue, numberOfRetries: 10);

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(10);
            _numberOfTimesActionCodeRan.Should().Be(11);

        }

        [TestMethod]
        public void RetryIf_retries_until_success()
        {
            //*************  arrange  ******************
            Action actionCode = () =>
             {
                 _numberOfTimesActionCodeRan++;
                 if (_numberOfTimesActionCodeRan < 4)
                     throw new ApplicationException();
             };


            //*************    act    ******************
            var result = _service.TryExecute(actionCode)
                            .RetryIf(_ifcodeTrue, numberOfRetries: 10);

            //*************  assert   ******************
            _numberOfTimesIfCodeRan.Should().Be(3);
            _numberOfTimesActionCodeRan.Should().Be(4);
            result.WasSuccessful.Should().BeTrue();
        }
    }
}
