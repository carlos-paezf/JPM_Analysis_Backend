using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("usersEntitlements", Name = "UserEntitlement")]
    [Produces("application/json")]
    public class UserEntitlementController
        : ControllerBase
            , IBaseApiController<UserEntitlementModel, UserEntitlementSimpleDTO>
            , ISoftDeleteController
    {

        private readonly JPMDatabaseContext _context;

        private readonly ILogger<UserEntitlementController> _logger;

        private readonly UserEntitlementService _service;

        private readonly IErrorHandlingService _errorHandlingService;

        public UserEntitlementController(
            JPMDatabaseContext context,
            UserEntitlementService service,
            ILogger<UserEntitlementController> logger,
            ErrorHandlingService errorHandlingService
        )
        {
            _context = context;
            _service = service;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
        }


        /// <summary>
        /// Retrieves all user entitlements.
        /// </summary>
        /// <returns>
        /// ActionResult containing a list response DTO with all user entitlements,
        /// or an error response if the operation fails.
        /// </returns>
        [HttpGet(Name = "GetUserEntitlements")]
        public async Task<ActionResult<ListResponseDTO<UserEntitlementModel>>> GetAll()
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
                    className: nameof(UserEntitlementController), methodName: nameof(GetAll));
            }
        }


        /// <summary>
        /// Retrieves a user entitlement by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the user entitlement to retrieve.</param>
        /// <returns>
        /// ActionResult containing the user entitlement with the specified primary key,
        /// or an error response if the operation fails.
        /// </returns>
        [HttpGet("{id}", Name = "GetUserEntitlementById")]
        public async Task<ActionResult<UserEntitlementModel>> GetByPk([FromRoute] string id)
        {
            try
            {
                var response = await _service.GetByPk(id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementController), methodName: nameof(GetByPk));
            }
        }


        /// <summary>
        /// Creates a new user entitlement.
        /// </summary>
        /// <param name="body">The user entitlement data to be created.</param>
        /// <returns>
        /// ActionResult containing the newly created user entitlement,
        /// or an error response if the operation fails.
        /// </returns>
        [HttpPost(Name = "PostUserEntitlement")]
        public async Task<ActionResult> Post([FromBody] UserEntitlementModel body)
        {
            try
            {
                await _service.Post(body);

                return CreatedAtAction(
                        nameof(GetByPk),
                        new { id = body.Id },
                        body
                    );
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementController), methodName: nameof(Post));
            }
        }


        /// <summary>
        /// Updates an existing user entitlement by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the user entitlement to update.</param>
        /// <param name="body">The updated user entitlement data.</param>
        /// <returns>
        /// ActionResult containing the updated user entitlement,
        /// or an error response if the operation fails.
        /// </returns>
        [HttpPut("update/{id}", Name = "UpdateUserEntitlement")]
        public async Task<ActionResult> UpdateByPK([FromRoute] string id, [FromBody] UserEntitlementSimpleDTO body)
        {
            try
            {
                if (id != body.Id) return BadRequest();

                var response = await _service.UpdateByPK(id, body);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementController), methodName: nameof(UpdateByPK));
            }
        }


        /// <summary>
        /// Soft deletes a user entitlement by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the user entitlement to delete.</param>
        /// <returns>
        /// NoContent response if the user entitlement is successfully soft deleted,
        /// or an error response if the operation fails.
        /// </returns>
        [HttpPatch("delete/{id}", Name = "DeleteUserEntitlement")]
        public async Task<ActionResult> SoftDelete([FromRoute] string id)
        {
            try
            {
                await _service.SoftDelete(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(UserEntitlementController), methodName: nameof(SoftDelete));
            }
        }


        /// <summary>
        /// Restores a soft-deleted user entitlement by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the soft-deleted user entitlement to restore.</param>
        /// <returns>
        /// NoContent response if the user entitlement is successfully restored,
        /// or an error response if the operation fails.
        /// </returns>
        [HttpPatch("restore/{id}", Name = "RestoreUserEntitlement")]
        public async Task<ActionResult> Restore([FromRoute] string id)
        {
            try
            {
                await _service.Restore(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(UserEntitlementController), methodName: nameof(Restore));
            }
        }
    }
}