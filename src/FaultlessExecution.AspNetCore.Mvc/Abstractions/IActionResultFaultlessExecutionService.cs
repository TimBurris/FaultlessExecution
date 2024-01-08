using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FaultlessExecution.AspNetCore.Mvc.Abstractions
{
    public interface IActionResultFaultlessExecutionService
    {

        /// <summary>
        /// Invokes <paramref name="code"/> handling errors and returning an <see cref="IActionResult"/>
        /// </summary>
        /// <typeparam name="T">The type to be returned by <paramref name="code"/> which will be included in <see cref="OkObjectResult"/> </typeparam>
        /// <param name="code">The code to invoke</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>If successful, an <see cref="OkObjectResult"/> with the <typeparamref name="T"/> value as the content
        /// if unsuccessful, a <see cref="BadRequestObjectResult"/> with content provided by <see cref="ActionResultFaultlessExecutionServiceConfiguration.BadRequestObjectBuilder"/>
        /// </returns>
        Task<IActionResult> TryExecuteAsync<T>(Func<Task<T>> code, string message = null, params object[] args);

        /// <summary>
        /// Invokes <paramref name="code"/> handling errors and returning an <see cref="IActionResult"/>
        /// </summary>
        /// <param name="code">The code to invoke</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>If successful, a <see cref="NoContentResult"/> 
        /// if unsuccessful, a <see cref="BadRequestObjectResult"/> with content provided by <see cref="ActionResultFaultlessExecutionServiceConfiguration.BadRequestObjectBuilder"/>
        /// </returns>
        Task<IActionResult> TryExecuteAsync(Func<Task> code, string message = null, params object[] args);

        /// <summary>
        /// Peforms a <see cref="TryExecuteAsync{T}(Func{T})"/> on <paramref name="code"/> by wrapping it inside of a Task.Run
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code">The code to be executed</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>If successful, an <see cref="OkObjectResult"/> with the <typeparamref name="T"/> value as the content
        /// if unsuccessful, a <see cref="BadRequestObjectResult"/> with content provided by <see cref="ActionResultFaultlessExecutionServiceConfiguration.BadRequestObjectBuilder"/>
        /// </returns>
        Task<IActionResult> TryExecuteSyncAsAsync<T>(Func<T> code, string message = null, params object[] args);

        /// <summary>
        /// Peforms a <see cref="TryExecuteAsync(Func{Task})"/> on <paramref name="code"/> by wrapping it inside of a Task.Run
        /// </summary>
        /// <param name="code">The code to be executed</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>If successful, a <see cref="NoContentResult"/> 
        /// if unsuccessful, a <see cref="BadRequestObjectResult"/> with content provided by <see cref="ActionResultFaultlessExecutionServiceConfiguration.BadRequestObjectBuilder"/>
        /// </returns>
        Task<IActionResult> TryExecuteSyncAsAsync(Action code, string message = null, params object[] args);

        /// <summary>
        /// Optional Configuration Options for this instance.  If not specified a <see cref="ActionResultFaultlessExecutionServiceConfiguration.DefaultConfiguration"/> will be used
        /// </summary>
        ActionResultFaultlessExecutionServiceConfiguration Configuration { get; set; }
    }
}
