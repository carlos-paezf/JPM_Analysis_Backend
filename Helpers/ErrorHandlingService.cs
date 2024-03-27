using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;


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
        /// The <c>HandleExceptionAsync</c> method handles different types of exceptions by returning
        /// appropriate HTTP status codes and messages. It checks the type of exception encountered
        /// and returns corresponding HTTP status codes along with relevant error messages in JSON format.
        /// If the exception is a known type, such as <see cref="BadRequestException"/>,
        /// <see cref="ItemNotFoundException"/>, or <see cref="DuplicateException"/>, it returns specific
        /// status codes (404 for not found, 409 for conflict, etc.) and custom error messages.
        /// If the exception is of type <see cref="DbUpdateException"/> and contains an inner
        /// exception of type <see cref="MySqlException"/> with error number 1062, indicating a duplicate
        /// entry error in MySQL, it returns a conflict status code (409) with a predefined message
        /// for duplicate entries.
        /// If the exception is of an unknown type, it logs the error and returns a generic
        /// status code 500 (Internal Server Error) along with a generic error message.
        /// </summary>
        /// <param name="ex">The <paramref name="ex"/> parameter represents the exception that occurred
        /// and needs to be handled.</param>
        /// <param name="logger">The <paramref name="logger"/> parameter is an instance of the
        /// <see cref="ILogger"/> interface used for logging error messages.</param>
        /// <param name="className">The <paramref name="className"/> parameter represents the name of
        /// the class where the exception occurred, used for logging purposes.</param>
        /// <param name="methodName">The <paramref name="methodName"/> parameter represents the name
        /// of the method where the exception occurred, used for logging purposes.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> representing an HTTP response containing the appropriate
        /// status code and error message based on the type of exception encountered.
        /// </returns>

        public async Task<ActionResult> HandleExceptionAsync(Exception ex, ILogger logger, string className, string methodName)
        {
            if (ex is BadRequestException)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message = ex.Message,
                        status = 400,
                        error = true
                    }
                );
            }
            else if (ex is ItemNotFoundException)
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
            else if (ex is DbUpdateException dbUpdateException && dbUpdateException.InnerException is MySqlException mySqlException && mySqlException.Number == 1062)
            {
                return new ConflictObjectResult(
                    new
                    {
                        message = "Ya existe un registro duplicado en la base de datos.",
                        status = 409,
                        error = true
                    }
                );
            }
            else
            {
                logger.LogError(ex, $"{className} - {methodName}: Ha ocurrido un error desconocido");
                return new ObjectResult(
                    new
                    {
                        message = ex.Message,
                        status = 500,
                        error = true
                    }
                )
                { StatusCode = 500 };
            }
        }
#pragma warning restore CS1998
    }
}