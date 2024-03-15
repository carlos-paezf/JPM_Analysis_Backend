using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("accounts", Name = "AccountController")]
    [Produces("application/json")]
    public class AccountController : ControllerBase, IBaseApiController<AccountModel, AccountSimpleDTO>, ISoftDeleteController
    {
        private readonly JPMDatabaseContext _context;

        private readonly ILogger<AccountController> _logger;

        private readonly AccountService _service;

        private readonly IErrorHandlingService _errorHandlingService;

        public AccountController(
            JPMDatabaseContext context,
            AccountService service,
            ILogger<AccountController> logger,
            ErrorHandlingService errorHandlingService
        )
        {
            _context = context;
            _service = service;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
        }


        /// <summary>
        /// This endpoint retrieves a list of accounts with the total number of results
        /// </summary>
        /// <returns>
        /// A paginated response containing a list of accounts is being returned. If there are no
        /// accounts found in the database, a "Not Found" response with a message indicating that no
        /// records were found is returned.
        /// </returns>
        /// <response code="200">Returns the accounts list. If there are no records in the DB, it returns an empty array/// </response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpGet(Name = "GetAccounts")]
        public async Task<ActionResult<ListResponseDTO<AccountModel>>> GetAll()
        {
            try
            {
                var response = await _service.GetAll();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountController), methodName: nameof(GetAll));
            }
        }


        /// <summary>
        /// This endpoint retrieves an Account entity by its account number along with related client
        /// and user entitlement details, or a 404 code status if not found.
        /// </summary>
        /// <param name="accountNumber">
        /// The Account Number is the unique identifier for each Account entry.
        /// </param>
        /// <returns>
        /// The GetById method is returning an ActionResult of type Account. If the entity is found in
        /// the database based on the provided accountNumber, it returns the entity. If the entity is
        /// not found (entity is null), it returns a NotFound result with a message indicating that the
        /// account was not found for the specified accountNumber.
        /// </returns>
        /// <response code="200">Returns an accounts by its account_number</response>
        /// <response code="404">Returns an error message by not found item</response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpGet("{accountNumber}", Name = "GetAccountByAccountNumber")]
        public async Task<ActionResult<AccountModel>> GetByPk([FromRoute] string accountNumber)
        {
            try
            {
                var response = await _service.GetByPk(accountNumber);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountController), methodName: nameof(GetByPk));
            }
        }


        /// <summary>
        /// This endpoint handles a POST request to create a new account, checking for duplicate
        /// account numbers and returning appropriate responses.
        /// </summary>
        /// <param name="body">
        /// The code you provided is a POST method in a controller that receives
        /// an Account object in the request body. The method calls a service to save the account
        /// information and returns an appropriate response based on the outcome.
        /// </param>
        /// <returns>
        /// The Post method in the AccountController is returning different types of ActionResult based
        /// on the outcome of the operation:
        /// </returns>
        /// <remarks>
        /// Sample Request:
        /// {
        ///     "accountNumber": "string",
        ///     "accountName": "string",
        ///     "accountType": "string",
        ///     "bankCurrency": "string"
        /// }
        /// </remarks>
        /// <response code="201">Returns the new account</response>
        /// <response code="409">Returns an error message by duplicate account number</response>
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPost(Name = "PostAccount")]
        public async Task<ActionResult> Post([FromBody] AccountModel body)
        {
            try
            {
                await _service.Post(body);

                return CreatedAtAction(
                        nameof(GetByPk),
                        new { accountNumber = body.AccountNumber },
                        body
                    );
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountController), methodName: nameof(Post));
            }
        }


        /// <summary>
        /// This endpoint updates an account by its account number, handling error cases and
        /// returning appropriate responses.
        /// </summary>
        /// <param name="accountNumber">The identifier for this item.</param>
        /// <param name="body">The data to update the entity</param>
        /// <returns>
        /// The code is returning an `ActionResult` asynchronously. If the update operation is
        /// successful, it returns an `Ok` response with the updated `body` object. If the
        /// `body.AccountNumber` does not match the `accountNumber` in the route, it returns a
        /// `BadRequest` response. If the existing account is not found, it returns a `NotFound`
        /// response. If an exception occurs during the execution throw an internal server error.
        /// </returns>
        /// <response code="200">Returns the updated  account information when successful</response>
        /// <response code="400">Alerts the user about incorrect input data</response>
        /// <response code="404">The account was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        /// 
        [HttpPut("update/{accountNumber}", Name = "UpdateAccount")]
        public async Task<ActionResult> UpdateByPK([FromRoute] string accountNumber, [FromBody] AccountSimpleDTO body)
        {
            try
            {
                if (accountNumber != body.AccountNumber) return BadRequest();

                var response = await _service.UpdateByPK(accountNumber, body);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountController), methodName: nameof(UpdateByPK));
            }
        }


        /// <summary>
        /// This endpoint handles a DELETE request to delete an account by account number, with error
        /// handling included.
        /// </summary>
        /// <param name="accountNumber">The `accountNumber` parameter is a string that represents the
        /// unique identifier of the account that needs to be deleted.</param>
        /// <returns>
        /// The Delete method is returning an ActionResult. If the deletion is successful, it returns a
        /// NoContent result (HTTP status code 204). If an exception occurs during the deletion process,
        /// it returns the result of the error handling service's HandleExceptionAsync method.
        /// </returns>
        /// <response code="204">If the delete process was success, returns an status code 204 without content</response>
        /// <response code="404">The account was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpDelete("delete/{accountNumber}", Name = "DeleteAccount")]
        public async Task<ActionResult> SoftDelete([FromRoute] string accountNumber)
        {
            try
            {
                await _service.SoftDelete(accountNumber);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(AccountController), methodName: nameof(SoftDelete));
            }
        }


        /// <summary>
        /// This endpoint handles PATCH requests to restore a specific account
        /// by its account number.
        /// </summary>
        /// <param name="accountNumber">The `accountNumber` parameter is a string that represents the
        /// unique identifier of the account that needs to be deleted.</param>
        /// <returns>
        /// The `Restore` method is returning an `ActionResult`. If the restoration process is
        /// successful, it returns a `NoContent` result. If an exception occurs during the restoration
        /// process, it returns the result of the `_errorHandlingService.HandleExceptionAsync` method,
        /// which handles the exception and logs the error.
        /// </returns>
        /// <response code="204">If the restore process was success, returns an status code 204 without content</response>
        /// <response code="404">The account was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPatch("restore/{accountNumber}", Name = "RestoreAccount")]
        public async Task<ActionResult> Restore([FromRoute] string accountNumber)
        {
            try
            {
                await _service.Restore(accountNumber);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(AccountController), methodName: nameof(Restore));
            }
        }
    }
}