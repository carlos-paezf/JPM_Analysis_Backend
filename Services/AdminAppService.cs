using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendJPMAnalysis.Services
{
    public class AdminAppService
    {
        private readonly JPMDatabaseContext _context;

        public AdminAppService(
            JPMDatabaseContext context
        )
        {
            _context = context;
        }


        /// <summary>
        /// Retrieves the data of the last uploaded report from the database.
        /// </summary>
        /// <returns>The data of the last uploaded report, or null if no reports are found.</returns>
        public async Task<ReportHistoryModel?> GetLastUploadReportData()
        {
            return await _context.ReportHistories
                .OrderByDescending(x => x.ReportUploadDate)
                .FirstOrDefaultAsync();
        }


        /// <summary>
        /// Retrieves the history of reports from the database.
        /// </summary>
        /// <returns>A list response containing the report history data.</returns>
        public async Task<ListResponseDTO<ReportHistoryModel>> GetReportsHistory()
        {
            List<ReportHistoryModel> data = await _context.ReportHistories
                .OrderByDescending(x => x.ReportUploadDate)
                .ToListAsync();
            int totalResults = data.Count;

            var response = new ListResponseDTO<ReportHistoryModel>
            {
                TotalResults = totalResults,
                Data = data
            };

            return response;
        }

    }
}