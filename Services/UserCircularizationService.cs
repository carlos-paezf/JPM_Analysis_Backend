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
        private readonly DepartmentService _departmentService;
        private readonly ProfileService _profileService;

        public UserCircularizationService(
            JPMDatabaseContext context,
            ILogger<UserCircularizationService> logger,
            IMapper mapper,
            CompanyUserService companyUserService,
            DepartmentService departmentService,
            ProfileService profileService
        )
        {
            _context = context;
            _mapper = mapper;
            _companyUserService = companyUserService;
            _departmentService = departmentService;
            _profileService = profileService;
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
                    companyUser.DepartmentInitials = assignment.DepartmentInitials;
                    _context.CompanyUsers.Update(companyUser);
                }
            }

            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Generates a user circularization report, grouping users by their department.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a dictionary where the keys are department initials 
        /// and the values are lists of UserCircularizationDTO objects representing users in those departments.
        /// </returns>
        /// <remarks>
        /// This method fetches all departments, profiles, and company users, and then constructs a list of UserCircularizationDTO objects.
        /// Each user is associated with their department and profile. Users are then grouped by their department initials.
        /// </remarks>
        public async Task<object> GenerateUserCircularization()
        {
            var departmentsResponse = await _departmentService.GetAll();
            var profilesResponse = await _profileService.GetAll();
            var companyUsersResponse = await _companyUserService.GetAll();

            var departments = departmentsResponse.Data.ToDictionary(d => d.Initials);
            var profiles = profilesResponse.Data.ToDictionary(p => p.Id);

            var userCircularizationList = companyUsersResponse.Data
                .Select(companyUser =>
                {
                    var department = companyUser.DepartmentInitials != null
                        ? departments.GetValueOrDefault(companyUser.DepartmentInitials)
                        : null;
                    var profile = profiles.GetValueOrDefault(companyUser.ProfileId);

                    if (profile != null)
                    {
                        return new UserCircularizationDTO(companyUser, department, profile);
                    }

                    return null;
                })
                .Where(dto => dto != null)
                .ToList();

            var groupedByDepartment = userCircularizationList
                .GroupBy(dto => dto!.DepartmentInitials)
                .Select(group => new
                {
                    Department = group.Key ?? "No Department",
                    Users = group.ToList()
                }).ToList();

            return groupedByDepartment;
        }
    }
}