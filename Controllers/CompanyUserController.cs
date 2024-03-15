using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("companyUsers", Name = "CompanyUsers")]
    [Produces("application/json")]
    public class CompanyUserController : ControllerBase, IBaseApiController<CompanyUserModel, CompanyUserSimpleDTO>, ISoftDeleteController
    {
        private readonly JPMDatabaseContext _context;

        private readonly ILogger<CompanyUserController> _logger;

        private readonly CompanyUserService _service;

        private readonly IErrorHandlingService _errorHandlingService;

        public CompanyUserController(
            JPMDatabaseContext context,
            CompanyUserService service,
            ILogger<CompanyUserController> logger,
            ErrorHandlingService errorHandlingService
        )
        {
            _context = context;
            _service = service;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
        }


        /// <summary>
        /// This endpoint retrieves a list of companyUsers with the total number of results
        /// </summary>
        /// <returns>
        /// A paginated response containing a list of companyUsers is being returned. If there are no
        /// companyUsers found in the database, a "Not Found" response with a message indicating that no
        /// records were found is returned.
        /// </returns>
        /// <response code="200">Returns the companyUsers list. If there are no records in the DB, it returns an empty array/// </response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpGet(Name = "GetCompanyUsers")]
        public async Task<ActionResult<ListResponseDTO<CompanyUserModel>>> GetAll()
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
                    className: nameof(CompanyUserController), methodName: nameof(GetAll));
            }
        }


        /// <summary>
        /// This endpoint retrieves an CompanyUser entity by its id along with related profile
        /// and userEntitlements details, or a 404 code status if not found.
        /// </summary>
        /// <param name="accessId">
        /// The id is the unique identifier for each CompanyUser entry.
        /// </param>
        /// <returns>
        /// The GetById method is returning an ActionResult of type CompanyUser. If the entity is found in
        /// the database based on the provided id, it returns the entity. If the entity is
        /// not found (entity is null), it returns a NotFound result with a message indicating that the
        /// companyUser was not found for the specified id.
        /// </returns>
        /// <response code="200">Returns an companyUser by its id</response>
        /// <response code="404">Returns an error message by not found item</response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpGet("{accessId}", Name = "GetCompanyUserByAccessId")]
        public async Task<ActionResult<CompanyUserModel>> GetByPk([FromRoute] string accessId)
        {
            try
            {
                var response = await _service.GetByPk(accessId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserController), methodName: nameof(GetByPk));
            }
        }


        /// <summary>
        /// This endpoint handles a POST request to create a new companyUser, checking for duplicate
        /// ids and returning appropriate responses.
        /// </summary>
        /// <param name="body">
        /// The code you provided is a POST method in a controller that receives
        /// an CompanyUser object in the request body. The method calls a service to save the companyUser
        /// information and returns an appropriate response based on the outcome.
        /// </param>
        /// <returns>
        /// The Post method in the CompanyUserController is returning different types of ActionResult based
        /// on the outcome of the operation:
        /// </returns>
        /// <response code="201">Returns the new companyUser</response>
        /// <response code="409">Returns an error message by duplicate id</response>
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPost(Name = "PostCompanyUser")]
        public async Task<ActionResult> Post([FromBody] CompanyUserModel body)
        {
            try
            {
                await _service.Post(body);

                return CreatedAtAction(
                        nameof(GetByPk),
                        new { accessId = body.AccessId },
                        body
                    );
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserController), methodName: nameof(Post));
            }
        }


        /// <summary>
        /// This endpoint updates an companyUser by its accessId, handling error cases and
        /// returning appropriate responses.
        /// </summary>
        /// <param name="accessId">The identifier for this item.</param>
        /// <param name="body">The data to update the entity</param>
        /// <returns>
        /// The code is returning an `ActionResult` asynchronously. If the update operation is
        /// successful, it returns an `Ok` response with the updated `body` object. If the
        /// `body.AccessId` does not match the `accessId` in the route, it returns a
        /// `BadRequest` response. If the existing companyUser is not found, it returns a `NotFound`
        /// response. If an exception occurs during the execution throw an internal server error.
        /// </returns>
        /// <response code="200">Returns the updated companyUser information when successful</response>
        /// <response code="400">Alerts the user about incorrect input data</response>
        /// <response code="404">The companyUser was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPut("update/{accessId}", Name = "UpdateCompanyUser")]
        public async Task<ActionResult> UpdateByPK([FromRoute] string accessId, [FromBody] CompanyUserSimpleDTO body)
        {
            try
            {
                if (accessId != body.AccessId) return BadRequest();

                var response = await _service.UpdateByPK(accessId, body);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserController), methodName: nameof(UpdateByPK));
            }
        }


        /// <summary>
        /// This endpoint handles a DELETE request to delete an companyUser by accessId, with error
        /// handling included.
        /// </summary>
        /// <param name="accessId">The `accessId` parameter is a string that represents the
        /// unique identifier of the companyUser that needs to be deleted.</param>
        /// <returns>
        /// The Delete method is returning an ActionResult. If the deletion is successful, it returns a
        /// NoContent result (HTTP status code 204). If an exception occurs during the deletion process,
        /// it returns the result of the error handling service's HandleExceptionAsync method.
        /// </returns>
        /// <response code="204">If the delete process was success, returns an status code 204 without content</response>
        /// <response code="404">The companyUser was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpDelete("delete/{accessId}", Name = "DeleteCompanyUser")]
        public async Task<ActionResult> SoftDelete([FromRoute] string accessId)
        {
            try
            {
                await _service.SoftDelete(accessId);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(CompanyUserController), methodName: nameof(SoftDelete));
            }
        }


        /// <summary>
        /// This endpoint handles PATCH requests to restore a specific companyUser
        /// by its accessId.
        /// </summary>
        /// <param name="accessId">The `accessId` parameter is a string that represents the
        /// unique identifier of the companyUser that needs to be deleted.</param>
        /// <returns>
        /// The `Restore` method is returning an `ActionResult`. If the restoration process is
        /// successful, it returns a `NoContent` result. If an exception occurs during the restoration
        /// process, it returns the result of the `_errorHandlingService.HandleExceptionAsync` method,
        /// which handles the exception and logs the error.
        /// </returns>
        /// <response code="204">If the restore process was success, returns an status code 204 without content</response>
        /// <response code="404">The companyUser was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPatch("restore/{accessId}", Name = "RestoreCompanyUser")]
        public async Task<ActionResult> Restore([FromRoute] string accessId)
        {
            try
            {
                await _service.Restore(accessId);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(CompanyUserController), methodName: nameof(Restore));
            }
        }
    }
}