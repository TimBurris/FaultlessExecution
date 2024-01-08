using System;
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
        public FuncExecutionResult<T> TryExecute<T>(Func<T> code, string message = null, params object[] args)
        {
            FuncExecutionResult<T> result;
            try
            {
                var codeResult = code.Invoke();

                result = FuncExecutionResult<T>.SuccessResult(executedBy: this, executedCode: code, value: codeResult);
            }
            catch (Exception ex)
            {
                this.HandleException(ex, message, args);
                result = FuncExecutionResult<T>.FailedResult(executedBy: this, executedCode: code, exception: ex);
            }

            this.OnResult(result);
            return result;
        }

        public async Task<AsyncFuncExecutionResult<T>> TryExecuteAsync<T>(Func<Task<T>> code, string message = null, params object[] args)
        {
            AsyncFuncExecutionResult<T> result;

            try
            {
                var codeResult = await code.Invoke();

                result = AsyncFuncExecutionResult<T>.SuccessResult(executedBy: this, executedCode: code, value: codeResult);
            }
            catch (Exception ex)
            {
                this.HandleException(ex, message, args);
                result = AsyncFuncExecutionResult<T>.FailedResult(executedBy: this, executedCode: code, exception: ex);
            }

            this.OnResult(result);
            return result;
        }

        public async Task<AsyncFuncExecutionResult<T>> TryExecuteSyncAsAsync<T>(Func<T> code, string message = null, params object[] args)
        {
            Func<Task<T>> x = () => Task.Run<T>(code);
            return await this.TryExecuteAsync(x, message, args);
        }
        #endregion

        #region Action Implementation

        public ActionExecutionResult TryExecute(Action code, string message = null, params object[] args)
        {
            ActionExecutionResult result;
            try
            {
                code.Invoke();

                result = ActionExecutionResult.SuccessResult(executedBy: this, executedCode: code);
            }
            catch (Exception ex)
            {
                this.HandleException(ex, message, args);
                result = ActionExecutionResult.FailedResult(executedBy: this, executedCode: code, exception: ex);
            }

            this.OnResult(result);
            return result;
        }

        public async Task<AsyncActionExecutionResult> TryExecuteAsync(Func<Task> code, string message = null, params object[] args)
        {
            AsyncActionExecutionResult result;
            try
            {
                await code.Invoke();

                result = AsyncActionExecutionResult.SuccessResult(executedBy: this, executedCode: code);
            }
            catch (Exception ex)
            {
                this.HandleException(ex, message, args);
                result = AsyncActionExecutionResult.FailedResult(executedBy: this, executedCode: code, exception: ex);
            }

            this.OnResult(result);
            return result;
        }

        public async Task<AsyncActionExecutionResult> TryExecuteSyncAsAsync(Action code, string message = null, params object[] args)
        {
            Func<Task> x = () => Task.Run(code);

            return await this.TryExecuteAsync(x, message, args);
        }
        #endregion


        private void HandleException(Exception ex, string message, params object[] args)
        {
            if (this.LogErrors && _logger != null)
            {
                if (string.IsNullOrEmpty(message))
                {
                    message = "Error caught by Faultless: " + ex?.Message;
                }

                _logger.LogError(ex, message, args);
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
