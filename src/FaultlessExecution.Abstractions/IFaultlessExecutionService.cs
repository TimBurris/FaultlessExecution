using System;
using System.Threading.Tasks;

namespace FaultlessExecution.Abstractions
{
    public interface IFaultlessExecutionService
    {
        #region Func<T>
        /// <summary>
        /// Peforms an Invoke for a delegate with return value of type <typeparamref name="T"/>, trapping any Exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code">The code to be executed</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>a FuncExecutionResult which indicates WasSuccessful, the exception that was raised(if one raised), as well as the return value T of your delegate code</returns>
        FuncExecutionResult<T> TryExecute<T>(Func<T> code, string message = null, params object[] args);

        /// <summary>
        /// awaits an Invoke on the <paramref name="code"/>, trapping any Exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code">The code to be executed</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>a <see cref="AsyncFuncExecutionResult"/> which indicates WasSuccessful, the exception that was raised(if one raised), as well as the return value T of your delegate code</returns>
        Task<AsyncFuncExecutionResult<T>> TryExecuteAsync<T>(Func<Task<T>> code, string message = null, params object[] args);

        /// <summary>
        /// Peforms a <see cref="TryExecuteSyncAsAsync{T}(Func{T})"/> on <paramref name="code"/> by wrapping it inside of a Task.Run
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code">The code to be executed</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>a <see cref="AsyncFuncExecutionResult"/> which indicates WasSuccessful, the exception that was raised(if one raised), as well as the return value T of your delegate code</returns>
        Task<AsyncFuncExecutionResult<T>> TryExecuteSyncAsAsync<T>(Func<T> code, string message = null, params object[] args);

        #endregion

        #region Action
        /// <summary>
        /// Peforms an Invoke for a delegate with return value of type void, trapping any Exceptions
        /// </summary>
        /// <param name="code">The code to be executed</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>a ActionExecutionResult which indicates WasSuccessful and the exception that was raised(if one raised)</returns>
        ActionExecutionResult TryExecute(Action code, string message = null, params object[] args);

        /// <summary>
        /// awaits an Invoke for a delegate with return value of type void, trapping any Exceptions
        /// </summary>
        /// <param name="code">The code to be executed</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>a ActionExecutionResult which indicates WasSuccessful and the exception that was raised(if one raised)</returns>
        Task<AsyncActionExecutionResult> TryExecuteAsync(Func<Task> code, string message = null, params object[] args);

        /// <summary>
        /// Peforms a <see cref="TryExecuteAsync(Func{Task})"/> on <paramref name="code"/> by wrapping it inside of a Task.Run
        /// </summary>
        /// <param name="code">The code to be executed</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>a <see cref="AsyncActionExecutionResult"/> which indicates WasSuccessful and the exception that was raised(if one raised)</returns>
        Task<AsyncActionExecutionResult> TryExecuteSyncAsAsync(Action code, string message = null, params object[] args);

        #endregion


        /// <summary>
        /// specifieds whether or not all errors should be logged
        /// </summary>
        /// <remarks>Defaut: true</remarks>
        bool LogErrors { get; set; }

    }
}
