using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("userCircularization", Name = "UserCircularization")]
    [Produces("application/json")]
    public class UserCircularizationController : ControllerBase
    {
        private readonly JPMDatabaseContext _context;

        private readonly ILogger<UserCircularizationController> _logger;

        private readonly IErrorHandlingService _errorHandlingService;

        private readonly UserCircularizationService _service;

        public UserCircularizationController(
            JPMDatabaseContext context,
            ILogger<UserCircularizationController> logger,
            UserCircularizationService service,
            ErrorHandlingService errorHandlingService
        )
        {
            _context = context;
            _logger = logger;
            _service = service;
            _errorHandlingService = errorHandlingService;
        }



        [HttpPatch("assign-department-to-company-users", Name = "AssignDepartmentToCompanyUser")]
        public async Task<ActionResult> AssignDepartmentToCompanyUsers([FromBody] List<UserDepartmentAssignmentDTO> body)
        {
            try
            {
                await _service.AssignDepartmentToCompanyUsers(body);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserController), methodName: nameof(AssignDepartmentToCompanyUsers)
                );
            }
        }
    }
}