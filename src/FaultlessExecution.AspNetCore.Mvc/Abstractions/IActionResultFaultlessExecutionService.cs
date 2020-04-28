using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <returns>If successful, an <see cref="OkObjectResult"/> with the <typeparamref name="T"/> value as the content
        /// if unsuccessful, a <see cref="BadRequestObjectResult"/> with content provided by <see cref="ActionResultFaultlessExecutionServiceConfiguration.BadRequestObjectBuilder"/>
        /// </returns>
        Task<IActionResult> TryExecuteAsync<T>(Func<Task<T>> code);

        /// <summary>
        /// Invokes <paramref name="code"/> handling errors and returning an <see cref="IActionResult"/>
        /// </summary>
        /// <param name="code">The code to invoke</param>
        /// <returns>If successful, a <see cref="NoContentResult"/> 
        /// if unsuccessful, a <see cref="BadRequestObjectResult"/> with content provided by <see cref="ActionResultFaultlessExecutionServiceConfiguration.BadRequestObjectBuilder"/>
        /// </returns>
        Task<IActionResult> TryExecuteAsync(Func<Task> code);

        /// <summary>
        /// Peforms a <see cref="TryExecuteAsync{T}(Func{T})"/> on <paramref name="code"/> by wrapping it inside of a Task.Run
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code">The code to be executed</param>
        /// <returns>If successful, an <see cref="OkObjectResult"/> with the <typeparamref name="T"/> value as the content
        /// if unsuccessful, a <see cref="BadRequestObjectResult"/> with content provided by <see cref="ActionResultFaultlessExecutionServiceConfiguration.BadRequestObjectBuilder"/>
        /// </returns>
        Task<IActionResult> TryExecuteSyncAsAsync<T>(Func<T> code);

        /// <summary>
        /// Peforms a <see cref="TryExecuteAsync(Func{Task})"/> on <paramref name="code"/> by wrapping it inside of a Task.Run
        /// </summary>
        /// <param name="code">The code to be executed</param>
        /// <returns>If successful, a <see cref="NoContentResult"/> 
        /// if unsuccessful, a <see cref="BadRequestObjectResult"/> with content provided by <see cref="ActionResultFaultlessExecutionServiceConfiguration.BadRequestObjectBuilder"/>
        /// </returns>
        Task<IActionResult> TryExecuteSyncAsAsync(Action code);

        /// <summary>
        /// Optional Configuration Options for this instance.  If not specified a <see cref="ActionResultFaultlessExecutionServiceConfiguration.DefaultConfiguration"/> will be used
        /// </summary>
        ActionResultFaultlessExecutionServiceConfiguration Configuration { get; set; }
    }
}
