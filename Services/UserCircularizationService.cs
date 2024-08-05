using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;

namespace BackendJPMAnalysis.Services
{
    public class UserCircularizationService
    {
        private readonly JPMDatabaseContext _context;
        private readonly IMapper _mapper;

        private readonly CompanyUserService _companyUserService;

        public UserCircularizationService(
            JPMDatabaseContext context,
            ILogger<UserCircularizationService> logger,
            IMapper mapper,
            CompanyUserService companyUserService
        )
        {
            _context = context;
            _mapper = mapper;
            _companyUserService = companyUserService;
        }


        public async Task AssignDepartmentToCompanyUsers(List<UserDepartmentAssignmentDTO> assignments)
        {
            var accessIds = assignments.Select(a => a.AccessId).ToList();

            var existingCompanyUsers = await _companyUserService.GetByPksNoTracking(accessIds)
                ?? throw new ItemNotFoundException("One or more accessIds not found");

            foreach (var assignment in assignments)
            {
                var companyUser = existingCompanyUsers.FirstOrDefault(companyUser => companyUser.AccessId == assignment.AccessId);

                if (companyUser != null)
                {
                    companyUser.DepartmentId = assignment.DepartmentInitials;
                    _context.CompanyUsers.Update(companyUser);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}