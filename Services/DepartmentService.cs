using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class DepartmentService
    : IBaseService<DepartmentModel, DepartmentEagerDTO, DepartmentSimpleDTO>
    , ISoftDeleteService
    {
        private readonly JPMDatabaseContext _context;
        private readonly IMapper _mapper;

        public DepartmentService(
            JPMDatabaseContext context,
            ILogger<DepartmentService> logger,
            IMapper mapper
        )
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ListResponseDTO<DepartmentModel>> GetAll()
        {
            List<DepartmentModel> data = await _context.Departments.ToListAsync();
            int totalResults = data.Count;

            var response = new ListResponseDTO<DepartmentModel>
            {
                TotalResults = totalResults,
                Data = data
            };

            return response;
        }


        public async Task<DepartmentEagerDTO?> GetByPk(string initials)
        {
            var account = await _context.Departments
                .Where(a => a.Initials == initials)
                .Include(a => a.CompanyUsers)
                .FirstOrDefaultAsync()
                ?? throw new ItemNotFoundException(initials);

            var companyUsersDTOs = account.CompanyUsers.Select(c => new CompanyUserSimpleDTO(c)).ToList();

            return new DepartmentEagerDTO(account, companyUsersDTOs);
        }


        public async Task<DepartmentModel?> GetByPkNoTracking(string pk)
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Initials == pk);
        }


        public async Task Post(DepartmentModel postBody)
        {
            if (await GetByPkNoTracking(postBody.Initials) != null)
                throw new DuplicateException(postBody.Initials);

            _context.Departments.Add(postBody);
            await _context.SaveChangesAsync();
        }


        public async Task<DepartmentSimpleDTO> UpdateByPK(string pk, DepartmentSimpleDTO updatedBody)
        {
            var existingDepartment = await GetByPkNoTracking(pk)
                ?? throw new ItemNotFoundException(pk);

            _mapper.Map(updatedBody, existingDepartment);

            _context.Entry(existingDepartment).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new DepartmentSimpleDTO(existingDepartment);
        }


        public async Task SoftDelete(string pk)
        {
            var existingDepartment = await GetByPkNoTracking(pk)
                ?? throw new ItemNotFoundException(pk);

            existingDepartment.DeletedAt = DateTime.UtcNow;

            _context.Departments.Update(existingDepartment);

            await _context.SaveChangesAsync();
        }


        public async Task Restore(string pk)
        {
            var existingDepartment = await GetByPkNoTracking(pk)
                ?? throw new ItemNotFoundException(pk);

            existingDepartment.DeletedAt = null;

            _context.Departments.Update(existingDepartment);

            await _context.SaveChangesAsync();
        }
    }
}