using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("userEntitlement", Name = "UserEntitlement")]
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