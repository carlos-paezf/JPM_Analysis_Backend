using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Helpers
{
    public interface IErrorHandlingService
    {
        Task<ActionResult> HandleExceptionAsync(Exception ex, ILogger logger, string className, string methodName);
    }


    public class ErrorHandlingService : IErrorHandlingService
    {
#pragma warning disable CS1998
        /// <summary>
        /// The function `HandleExceptionAsync` handles different types of exceptions by returning
        /// appropriate HTTP status codes and messages.
        /// </summary>
        /// <param name="ex">The `HandleExceptionAsync` method is designed to handle different
        /// types of exceptions and return appropriate responses based on the type of exception
        /// encountered.</param>
        /// <param name="logger">ILogger is an interface provided by ASP.NET Core for logging. It
        /// allows you to log messages from your application code.ILogger provides methods like
        /// LogInformation, LogError, LogWarning, etc. to log messages with different log levels.ILogger
        /// is typically injected into classes where logging is needed, such as controllers</param>
        /// <param name="className">The `className` parameter in the `HandleExceptionAsync` method
        /// represents the name of the class where the exception occurred. It is used for logging
        /// purposes to identify the source of the error within the codebase.</param>
        /// <param name="methodName">The `methodName` parameter in the `HandleExceptionAsync` method
        /// refers to the name of the method where the exception occurred. It is used for logging
        /// purposes to identify the specific method that threw the exception.</param>
        /// <returns>
        /// The method `HandleExceptionAsync` returns an `ActionResult` based on the type of exception
        /// passed to it. If the exception is of type `ItemNotFoundException`, it returns a
        /// `NotFoundObjectResult` with a JSON object containing the message, status code 404, and an
        /// error flag. If the exception is of type `DuplicateException`, it returns a
        /// `ConflictObjectResult` with similar JSON structure
        /// </returns>
        public async Task<ActionResult> HandleExceptionAsync(Exception ex, ILogger logger, string className, string methodName)
        {
            if (ex is ItemNotFoundException)
            {
                return new NotFoundObjectResult(
                    new
                    {
                        message = ex.Message,
                        status = 404,
                        error = true
                    }
                );
            }
            else if (ex is DuplicateException)
            {
                return new ConflictObjectResult(
                    new
                    {
                        message = ex.Message,
                        status = 409,
                        error = true
                    }
                );
            }
            else
            {
                logger.LogError(ex, $"{className} - {methodName}: Ha ocurrido un error desconocido");
                return new StatusCodeResult(500);
            }
        }
#pragma warning restore CS1998
    }
}