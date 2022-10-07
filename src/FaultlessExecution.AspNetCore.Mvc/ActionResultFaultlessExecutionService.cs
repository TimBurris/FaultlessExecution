using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FaultlessExecution.AspNetCore.Mvc
{
    public class ActionResultFaultlessExecutionService : Abstractions.IActionResultFaultlessExecutionService
    {
        private readonly FaultlessExecution.Abstractions.IFaultlessExecutionService _faultlessExecutionService;
        private readonly Microsoft.Extensions.Logging.ILogger<ActionResultFaultlessExecutionService> _logger;

        public ActionResultFaultlessExecutionService(FaultlessExecution.Abstractions.IFaultlessExecutionService faultlessExecutionService,
            Microsoft.Extensions.Logging.ILogger<ActionResultFaultlessExecutionService> logger)
        {
            _faultlessExecutionService = faultlessExecutionService;
            _faultlessExecutionService.LogErrors = false;//bypass the built in error log, we'll log the errors ourself
            _logger = logger;
        }

        public async Task<IActionResult> TryExecuteAsync<T>(Func<Task<T>> code, string message = "Error caught by Faultless", params object[] args)
        {
            var result = await _faultlessExecutionService.TryExecuteAsync(code, message, args);
            if (result.WasSuccessful)
            {
                return new OkObjectResult(result.ReturnValue);
            }
            else
            {
                _logger.LogInformation("Exception caught; BadRequest with FailModel will be sent to client");
                return new BadRequestObjectResult(this.BuildBadRequestObject(result));
            }
        }

        public async Task<IActionResult> TryExecuteSyncAsAsync<T>(Func<T> code, string message = "Error caught by Faultless", params object[] args)
        {
            Func<Task<T>> x = () => Task.Run<T>(code);
            return await this.TryExecuteAsync(x, message, args, message, args);
        }


        public async Task<IActionResult> TryExecuteAsync(Func<Task> code, string message = "Error caught by Faultless", params object[] args)
        {
            var result = await _faultlessExecutionService.TryExecuteAsync(code, message, args);
            if (result.WasSuccessful)
            {
                return new NoContentResult();
            }
            else
            {
                _logger.LogInformation("Exception caught; BadRequest with FailModel will be sent to client");
                return new BadRequestObjectResult(this.BuildBadRequestObject(result));
            }
        }


        public async Task<IActionResult> TryExecuteSyncAsAsync(Action code, string message = "Error caught by Faultless", params object[] args)
        {
            Func<Task> x = () => Task.Run(code);
            return await this.TryExecuteAsync(x, message, args);
        }

        public ActionResultFaultlessExecutionServiceConfiguration Configuration { get; set; }

        protected object BuildBadRequestObject(FaultlessExecution.Abstractions.ExecutionResult result)
        {
            var config = (this.Configuration ?? ActionResultFaultlessExecutionServiceConfiguration.DefaultConfiguration);


            return config?.BadRequestObjectBuilder?.Invoke(result);
        }
    }

    //T O D O: find away to make this class name longer
    public class ActionResultFaultlessExecutionServiceConfiguration : ICloneable
    {
        #region Static Default Config
        private static ActionResultFaultlessExecutionServiceConfiguration _defaultConfiguration;

        /// <summary>
        /// gets or sets the configuration that will be used by all instances of <see cref="Abstractions.IActionResultFaultlessExecutionService"/> who don't have an explicit Configuration assigned
        /// </summary>
        public static ActionResultFaultlessExecutionServiceConfiguration DefaultConfiguration
        {
            get
            {
                if (_defaultConfiguration == null)
                {
                    _defaultConfiguration = new ActionResultFaultlessExecutionServiceConfiguration()
                    {
                        BadRequestObjectBuilder = (result) => result?.Exception?.Message, //default is to just use the errormessage
                    };
                }
                return _defaultConfiguration;
            }
        }

        #endregion

        private Func<FaultlessExecution.Abstractions.ExecutionResult, object> _badRequestObjectBuilder;

        public Func<FaultlessExecution.Abstractions.ExecutionResult, object> BadRequestObjectBuilder
        {
            get { return _badRequestObjectBuilder; }
            set { _badRequestObjectBuilder = value; }
        }

        public ActionResultFaultlessExecutionServiceConfiguration Clone()
        {
            return new ActionResultFaultlessExecutionServiceConfiguration()
            {
                BadRequestObjectBuilder = this.BadRequestObjectBuilder,
            };
        }

        object ICloneable.Clone() { return Clone(); }
    }
}
