using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("adminApp", Name = "AdminApp")]
    [Produces("application/json")]
    public class AdminAppController : ControllerBase
    {
        private readonly ILogger<CompanyUserController> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly AdminAppService _adminAppService;


        public AdminAppController(
            ILogger<CompanyUserController> logger,
            ErrorHandlingService errorHandlingService,
            AdminAppService service
        )
        {
            _adminAppService = service;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
        }


        /// <summary>
        /// Retrieves the data of the last uploaded report from the database.
        /// </summary>
        /// <returns>
        /// ActionResult containing the data of the last uploaded report if successful; otherwise, an error response.
        /// </returns>
        [HttpGet("last-upload-report-data", Name = "GetLastUploadReportData")]
        public async Task<ActionResult> GetLastUploadReportData()
        {
            try
            {
                var response = await _adminAppService.GetLastUploadReportData();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AdminAppController), methodName: nameof(GetLastUploadReportData));
            }
        }


        /// <summary>
        /// Retrieves the history of uploaded reports from the database.
        /// </summary>
        /// <returns>
        /// ActionResult containing the history of uploaded reports if successful; otherwise, an error response.
        /// </returns>
        [HttpGet("reports-history", Name = "ReportsHistory")]
        public async Task<ActionResult> GetReportsHistory()
        {
            try
            {
                var response = await _adminAppService.GetReportsHistory();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AdminAppController), methodName: nameof(GetReportsHistory));
            }
        }
    }
}