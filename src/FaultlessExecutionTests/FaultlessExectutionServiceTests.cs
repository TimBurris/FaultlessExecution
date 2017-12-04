﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using FaultlessExecution;
using System.Threading.Tasks;

namespace FaultlessExecutionTests
{
    [TestClass]
    public class FaultlessExectutionService_Action_Sync_Tests
    {
        private int _numTimesCodeRan = 0;
        private Action _successfulAction;
        private Action _failAction;

        [TestInitialize]
        public void Init()
        {
            _successfulAction = () => _numTimesCodeRan++;
            _failAction = () =>
            {
                _numTimesCodeRan++;
                throw new ApplicationException("i failed");
            };
        }

        [TestMethod]
        public void TryExecute_when_no_error_returns_success()
        {

            //*************  arrange  ******************
            var service = new FaultlessExecutionService();

            //*************    act    ******************
            var result = service.TryExecute(_successfulAction);

            //*************  assert   ******************
            result.WasSuccessful.Should().BeTrue();
            _numTimesCodeRan.Should().Be(1);
            result.ExecutedBy.Should().Be(service);
            result.ExecutedCode.Should().Be(_successfulAction);
        }

        [TestMethod]
        public void TryExecute_when_error_returns_fail()
        {

            //*************  arrange  ******************
            var service = new FaultlessExecutionService();

            //*************    act    ******************
            var result = service.TryExecute(_failAction);

            //*************  assert   ******************
            _numTimesCodeRan.Should().Be(1);
            result.WasSuccessful.Should().BeFalse();
            result.Exception.Should().NotBeNull();
            result.Exception.Message.Should().Be("i failed");
            result.ExecutedBy.Should().Be(service);
            result.ExecutedCode.Should().Be(_failAction);

        }
    }

    [TestClass]
    public class FaultlessExectutionService_Action_Async_Tests
    {
        private int _numTimesCodeRan = 0;
        private Func<Task> _successfulFunc;
        private Func<Task> _failFunc;

        [TestInitialize]
        public void Init()
        {
            _successfulFunc = () => this.RunSuccessfullyAsync();
            _failFunc = () => this.RunFailAsync();
        }

        private async Task RunSuccessfullyAsync()
        {
            _numTimesCodeRan++;
            await Task.Delay(2);
        }
        private async Task RunFailAsync()
        {
            _numTimesCodeRan++;
            await Task.Delay(2);
            throw new ApplicationException("i failed");
        }

        [TestMethod]
        public void TryExecute_when_no_error_returns_success()
        {

            //*************  arrange  ******************
            var service = new FaultlessExecutionService();

            //*************    act    ******************
            var result = service.TryExecuteAsync(_successfulFunc).Result;

            //*************  assert   ******************
            result.WasSuccessful.Should().BeTrue();
            _numTimesCodeRan.Should().Be(1);
            result.ExecutedBy.Should().Be(service);
            result.ExecutedCode.Should().Be(_successfulFunc);
        }

        [TestMethod]
        public void TryExecute_when_error_returns_fail()
        {
            //*************  arrange  ******************
            var service = new FaultlessExecutionService();

            //*************    act    ******************
            var result = service.TryExecuteAsync(_failFunc).Result;

            //*************  assert   ******************
            _numTimesCodeRan.Should().Be(1);
            result.WasSuccessful.Should().BeFalse();
            result.Exception.Should().NotBeNull();
            result.Exception.Message.Should().Be("i failed");
            result.ExecutedBy.Should().Be(service);
            result.ExecutedCode.Should().Be(_failFunc);
        }

    }

    [TestClass]
    public class FaultlessExectutionService_FuncT_Sync_Tests
    {
        private int _numTimesCodeRan = 0;
        private Func<string> _successfulFunc;
        private Func<string> _failFunc;

        [TestInitialize]
        public void Init()
        {
            _successfulFunc = () =>
            {
                _numTimesCodeRan++;
                return "i ran successfully";
            };
            _failFunc = () =>
            {
                _numTimesCodeRan++;
                throw new ApplicationException("i failed");
            };
        }

        [TestMethod]
        public void TryExecute_when_no_error_returns_success()
        {

            //*************  arrange  ******************
            var service = new FaultlessExecutionService();

            //*************    act    ******************
            var result = service.TryExecute(_successfulFunc);

            //*************  assert   ******************
            result.WasSuccessful.Should().BeTrue();
            result.ReturnValue.Should().Be("i ran successfully");
            _numTimesCodeRan.Should().Be(1);
            result.ExecutedBy.Should().Be(service);
            result.ExecutedCode.Should().Be(_successfulFunc);
        }

        [TestMethod]
        public void TryExecute_when_error_returns_fail()
        {

            //*************  arrange  ******************
            var service = new FaultlessExecutionService();

            //*************    act    ******************
            var result = service.TryExecute(_failFunc);

            //*************  assert   ******************
            _numTimesCodeRan.Should().Be(1);
            result.WasSuccessful.Should().BeFalse();
            result.Exception.Should().NotBeNull();
            result.Exception.Message.Should().Be("i failed");
            result.ExecutedBy.Should().Be(service);
            result.ExecutedCode.Should().Be(_failFunc);
        }

    }

    [TestClass]
    public class FaultlessExectutionService_FuncT_Async_Tests
    {
        private int _numTimesCodeRan = 0;
        private Func<Task<string>> _successfulFunc;
        private Func<Task<string>> _failFunc;

        [TestInitialize]
        public void Init()
        {
            _successfulFunc = () => this.RunSuccessfullyAsync();
            _failFunc = () => this.RunFailAsync();
        }

        private async Task<string> RunSuccessfullyAsync()
        {
            _numTimesCodeRan++;
            await Task.Delay(2);
            return "i ran successfully";
        }
        private async Task<string> RunFailAsync()
        {
            _numTimesCodeRan++;
            await Task.Delay(2);
            throw new ApplicationException("i failed");
        }

        [TestMethod]
        public void TryExecute_when_no_error_returns_success()
        {

            //*************  arrange  ******************
            var service = new FaultlessExecutionService();

            //*************    act    ******************
            var result = service.TryExecuteAsync(_successfulFunc).Result;

            //*************  assert   ******************
            result.WasSuccessful.Should().BeTrue();
            result.ReturnValue.Should().Be("i ran successfully");
            _numTimesCodeRan.Should().Be(1);
            result.ExecutedBy.Should().Be(service);
            result.ExecutedCode.Should().Be(_successfulFunc);
        }

        [TestMethod]
        public void TryExecute_when_error_returns_fail()
        {

            //*************  arrange  ******************
            var service = new FaultlessExecutionService();

            //*************    act    ******************
            var result = service.TryExecuteAsync(_failFunc).Result;

            //*************  assert   ******************
            _numTimesCodeRan.Should().Be(1);
            result.WasSuccessful.Should().BeFalse();
            result.Exception.Should().NotBeNull();
            result.Exception.Message.Should().Be("i failed");
            result.ExecutedBy.Should().Be(service);
            result.ExecutedCode.Should().Be(_failFunc);
        }

    }
}
