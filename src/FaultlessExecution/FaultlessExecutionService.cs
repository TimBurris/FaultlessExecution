using System;
using System.Collections.Generic;
using System.Text;
using FaultlessExecution.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FaultlessExecution
{
    public class FaultlessExecutionService : IFaultlessExecutionService
    {
        private readonly ILogger<FaultlessExecutionService> _logger;

        public FaultlessExecutionService(ILogger<FaultlessExecutionService> logger)
        {
            _logger = logger;
        }
        #region Func<T> implementation
        public FuncExecutionResult<T> TryExecute<T>(Func<T> code)
        {
            FuncExecutionResult<T> result;
            try
            {
                var codeResult = code.Invoke();

                result = FuncExecutionResult<T>.SuccessResult(executedBy: this, executedCode: code, value: codeResult);
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                result = FuncExecutionResult<T>.FailedResult(executedBy: this, executedCode: code, exception: ex);
            }

            this.OnResult(result);
            return result;
        }

        public async Task<AsyncFuncExecutionResult<T>> TryExecuteAsync<T>(Func<Task<T>> code)
        {
            AsyncFuncExecutionResult<T> result;

            try
            {
                var codeResult = await code.Invoke();

                result = AsyncFuncExecutionResult<T>.SuccessResult(executedBy: this, executedCode: code, value: codeResult);
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                result = AsyncFuncExecutionResult<T>.FailedResult(executedBy: this, executedCode: code, exception: ex);
            }

            this.OnResult(result);
            return result;
        }

        public async Task<AsyncFuncExecutionResult<T>> TryExecuteSyncAsAsync<T>(Func<T> code)
        {
            Func<Task<T>> x = () => Task.Run<T>(code);
            return await this.TryExecuteAsync(x);
        }
        #endregion

        #region Action Implementation

        public ActionExecutionResult TryExecute(Action code)
        {
            ActionExecutionResult result;
            try
            {
                code.Invoke();

                result = ActionExecutionResult.SuccessResult(executedBy: this, executedCode: code);
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                result = ActionExecutionResult.FailedResult(executedBy: this, executedCode: code, exception: ex);
            }

            this.OnResult(result);
            return result;
        }

        public async Task<AsyncActionExecutionResult> TryExecuteAsync(Func<Task> code)
        {
            AsyncActionExecutionResult result;
            try
            {
                await code.Invoke();

                result = AsyncActionExecutionResult.SuccessResult(executedBy: this, executedCode: code);
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                result = AsyncActionExecutionResult.FailedResult(executedBy: this, executedCode: code, exception: ex);
            }

            this.OnResult(result);
            return result;
        }

        public async Task<AsyncActionExecutionResult> TryExecuteSyncAsAsync(Action code)
        {
            Func<Task> x = () => Task.Run(code);

            return await this.TryExecuteAsync(x);
        }
        #endregion


        private void HandleException(Exception ex)
        {
            if (this.LogErrors && _logger != null)
            {
                _logger.LogError(ex, "Error caught by Faultless");
            }
            this.OnException(ex);
        }

        /// <summary>
        /// specifieds whether or not all errors should be logged
        /// </summary>
        public bool LogErrors { get; set; } = true;//default is true

        /// <summary>
        /// an overridable method that consumers can use to handle all errors
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void OnException(Exception ex) { }

        protected virtual void OnResult(AsyncActionExecutionResult result) { }
        protected virtual void OnResult(ActionExecutionResult result) { }
        protected virtual void OnResult<T>(AsyncFuncExecutionResult<T> result) { }
        protected virtual void OnResult<T>(FuncExecutionResult<T> result) { }
    }

}
