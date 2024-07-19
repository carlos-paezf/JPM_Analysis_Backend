using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("departments", Name = "DepartmentController")]
    [Produces("application/json")]
    public class DepartmentController : ControllerBase, IBaseApiController<DepartmentModel, DepartmentSimpleDTO>, ISoftDeleteController
    {
        private readonly JPMDatabaseContext _context;

        private readonly ILogger<DepartmentController> _logger;

        private readonly DepartmentService _service;

        private readonly IErrorHandlingService _errorHandlingService;

        public DepartmentController(
            JPMDatabaseContext context,
            DepartmentService service,
            ILogger<DepartmentController> logger,
            ErrorHandlingService errorHandlingService
        )
        {
            _context = context;
            _service = service;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
        }


        [HttpGet(Name = "GetDepartments")]
        public async Task<ActionResult<ListResponseDTO<DepartmentModel>>> GetAll()
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
                    className: nameof(DepartmentController), methodName: nameof(GetAll));
            }
        }


        [HttpGet("{initials}", Name = "GetDepartmentByInitials")]
        public async Task<ActionResult<DepartmentModel>> GetByPk([FromRoute] string initials)
        {
            try
            {
                var response = await _service.GetByPk(initials);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(DepartmentController), methodName: nameof(GetByPk));
            }
        }


        [HttpPost(Name = "PostDepartment")]
        public async Task<ActionResult> Post([FromBody] DepartmentModel body)
        {
            try
            {
                await _service.Post(body);

                return CreatedAtAction(
                        nameof(GetByPk),
                        new { initials = body.Initials },
                        body
                    );
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(DepartmentController), methodName: nameof(Post));
            }
        }


        [HttpPut("update/{initials}", Name = "UpdateDepartment")]
        public async Task<ActionResult> UpdateByPK([FromRoute] string initials, [FromBody] DepartmentSimpleDTO body)
        {
            try
            {
                if (initials != body.Initials) return BadRequest();

                var response = await _service.UpdateByPK(initials, body);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(DepartmentController), methodName: nameof(UpdateByPK));
            }
        }


        [HttpPatch("delete/{initials}", Name = "DeleteDepartment")]
        public async Task<ActionResult> SoftDelete([FromRoute] string initials)
        {
            try
            {
                await _service.SoftDelete(initials);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(DepartmentController), methodName: nameof(SoftDelete));
            }
        }


        [HttpPatch("restore/{initials}", Name = "RestoreDepartment")]
        public async Task<ActionResult> Restore([FromRoute] string initials)
        {
            try
            {
                await _service.Restore(initials);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(DepartmentController), methodName: nameof(Restore));
            }
        }
    }
}