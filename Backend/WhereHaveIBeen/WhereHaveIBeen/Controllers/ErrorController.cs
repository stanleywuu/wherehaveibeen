using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;

namespace WhereHaveIBeen.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Default action for handling all exception in the system
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AllowAnonymous]
        public IActionResult Get()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                // Get which route the exception occurred at
                string routeWhereExceptionOccurred = exceptionFeature.Path;

                // Get the exception that occurred
                Exception exception = exceptionFeature.Error;
                if (exception is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                else if (
                    exception is ArgumentOutOfRangeException ||
                    exception is SqlException)
                {
                    return ValidationProblem(new ValidationProblemDetails() { Detail = exception.Message });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}