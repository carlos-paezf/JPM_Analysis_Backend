using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("excel", Name = "ExcelFile")]
    [Produces("application/json")]
    public class ExcelProcessingController : ControllerBase
    {
        private readonly ILogger<CompanyUserController> _logger;
        private readonly IErrorHandlingService _errorHandlingService;

        private readonly ExcelProcessingService _excelProcessingService;
        private readonly BulkSeedService _bulkSeedService;
        private readonly CompareSeedService _compareSeedService;


        public ExcelProcessingController(
            ILogger<CompanyUserController> logger,
            ErrorHandlingService errorHandlingService,
            ExcelProcessingService service,
            BulkSeedService bulkSeedService,
            CompareSeedService compareSeedService
        )
        {
            _excelProcessingService = service;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
            _bulkSeedService = bulkSeedService;
            _compareSeedService = compareSeedService;
        }


        /// <summary>
        /// Uploads an Excel file containing report data, processes it, and optionally loads it into the database or compares it with existing data.
        /// </summary>
        /// <param name="action">The action to perform: 'load' to load the data into the database or 'compare' to compare it with existing data.</param>
        /// <param name="observations">Additional observations or comments about the uploaded file.</param>
        /// <param name="appUserId">The ID of the user who uploaded the file.</param>
        /// <param name="file">The Excel file containing report data to upload.</param>
        /// <returns>
        /// ActionResult containing the result of the upload or comparison process if successful; otherwise, an error response.
        /// </returns>
        [HttpPost(Name = "UploadExcelFileReport")]
        public async Task<ActionResult> UploadExcelFileReport(
            [FromForm] QueryOption action,
            [FromForm] string? observations,
            [FromForm] string appUserId,
            [FromForm] IFormFile file
        )
        {
            try
            {
                if (file == null || file.Length <= 0)
                    return BadRequest("No se proporcionó ningún archivo, o el archivo está vacío");

                if (Request.Form.Files.Count > 1)
                    return BadRequest("Solo se permite cargar un archivo a la vez");

                if (Path.GetExtension(file.FileName).ToLower() != ".xlsx"
                    && Path.GetExtension(file.FileName).ToLower() != ".xls")
                    return BadRequest("El archivo debe ser de tipo Excel (.xlsx o .xls)");

                string fileName = file.FileName;

                using var memoryStream = new MemoryStream();

                await file.CopyToAsync(memoryStream);

                memoryStream.Position = 0;

                ExcelDataDTO excelData = _excelProcessingService.ExtractDataFromExcelFile(memoryStream, fileName);

                if (action == QueryOption.load)
                {
                    await _bulkSeedService.PostSeedInDatabase(excelData);

                    // TODO: Se debe actualizar la tabla de app_user
                    await _bulkSeedService.UpdateReportHistory(fileName, appUserId, observations, excelData.RunReportDate);

                    return Ok(new { Action = action, Result = excelData });
                }

                var result = await _compareSeedService.CompareSeedWithDBData(excelData, fileName, observations);

                return Ok(new { Action = action, Result = result });
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ExcelProcessingController), methodName: nameof(UploadExcelFileReport));
            }
        }


        /// <summary>
        /// Approves changes specified in the provided data and updates the report history accordingly.
        /// </summary>
        /// <param name="body">The data containing the entities to be changed, along with additional information such as file name, user ID, observations, and report run date.</param>
        /// <returns>
        /// ActionResult containing the number of affected rows if the changes are successfully approved; otherwise, an error response.
        /// </returns>
        [HttpPost("approve-changes", Name = "ApproveChanges")]
        public async Task<ActionResult> ApproveChanges(
            [FromBody] ApproveChangesDTO body
            )
        {
            try
            {
                var affectedRows = await _bulkSeedService.ApproveChanges(body.EntitiesToChange);

                // TODO: Se debe actualizar la tabla de app_user
                await _bulkSeedService.UpdateReportHistory(body.FileName, body.AppUserId, body.Observations, body.RunReportDate);

                return Ok(new { AffectedRows = affectedRows });
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ExcelProcessingController), methodName: nameof(UploadExcelFileReport));
            }
        }

    }
}