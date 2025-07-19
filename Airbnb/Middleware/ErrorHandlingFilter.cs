using System.Net;
using Application.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Airbnb.Middleware
{
    public class ErrorHandlingFilter : IActionFilter, IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ErrorHandlingFilter> _logger;

        public ErrorHandlingFilter(IWebHostEnvironment env, ILogger<ErrorHandlingFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Handle model validation errors early
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.ExtractErrorList();
                _logger.LogWarning("Model validation failed: {Errors}", string.Join(", ", errors)); // Log warning for invalid model
                context.Result = new BadRequestObjectResult(errors);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Skip if there was an exception — handled in OnException
            if (context.Exception != null)
                return;

            if (context.Result is ObjectResult objectResult)
            {
                var statusCode = objectResult.StatusCode ?? 200;

                if (statusCode >= 400)
                {
                    string rawError = objectResult.Value switch
                    {
                        IEnumerable<string> strList => string.Join(" | ", strList),
                        Exception ex => $"{ex.Message}{Environment.NewLine}{ex.StackTrace}",
                        string str => str,
                        _ => objectResult.Value?.ToString() ?? "null",
                    };

                    _logger.LogError(
                        "An error occurred with status code {StatusCode}. Raw error: {Error}",
                        statusCode,
                        rawError
                    );

                    // Prevent re-wrapping if already List<string>
                    if (objectResult.Value is not List<string>)
                    {
                        var errorList = objectResult.Value switch
                        {
                            string str => new List<string> { str },
                            IEnumerable<string> strList => strList.ToList(),
                            Exception ex => new List<string> { ex.Message },
                            _ => new List<string> { "An unexpected error occurred." },
                        };

                        context.Result = new ObjectResult(errorList) { StatusCode = statusCode };
                    }
                }
            }
        }

        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            // Log the actual exception details
            _logger.LogError(
                context.Exception,
                "An unhandled exception occurred while processing the request."
            );

            var errorMessages = new List<string>();
            int statusCode = 500;

            if (context.Exception is ApiException apiEx)
            {
                statusCode = (int)apiEx.StatusCode;

                if (apiEx is ValidationException validationEx)
                {
                    errorMessages = validationEx.Errors;
                }
                else
                {
                    errorMessages.Add(apiEx.Message);
                }
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorMessages.Add("An error occurred while processing your request.");

                if (!_env.IsProduction())
                {
                    errorMessages.Add(context.Exception.ToString());
                }
                else
                {
                    errorMessages.Add(context.Exception.Message);
                }
            }

            context.Result = new ObjectResult(errorMessages) { StatusCode = statusCode };
        }
    }
}
