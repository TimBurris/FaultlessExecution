using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FaultlessExecution.Extensions
{
    public static class ActionResultExtensions
    {
        public static async Task<IActionResult> OnOkNullReturnNotFound(this Task<IActionResult> t)
        {
            var result = await t;

            return OnOkNullReturnNotFound(result);
        }

        public static IActionResult OnOkNullReturnNotFound(this IActionResult actionResult)
        {
            var okResult = actionResult as OkObjectResult;

            if (okResult == null)
                return actionResult;
            else if (okResult.Value == null)
                return new NotFoundResult();
            else
                return actionResult;
        }
    }
}
