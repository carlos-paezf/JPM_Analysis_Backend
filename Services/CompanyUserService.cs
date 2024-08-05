using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class CompanyUserService : IBaseService<CompanyUserModel, CompanyUserEagerDTO, CompanyUserSimpleDTO>, ISoftDeleteService
    {
        private readonly JPMDatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly DataAccessService _dataAccessService;


        public CompanyUserService(
            JPMDatabaseContext context,
            IMapper mapper,
            DataAccessService dataAccessService
        )
        {
            _context = context;
            _mapper = mapper;
            _dataAccessService = dataAccessService;
        }


        /// <summary>
        /// The function GetAll retrieves all companyUsers from the database and returns them in a
        /// ListResponse object.
        /// </summary>
        /// <returns>
        /// The method `GetAll` is returning a `Task` that will eventually resolve to a
        /// `ListResponse` of `CompanyUser`. The `ListResponse` of `CompanyUser` object contains the total number of
        /// results (`TotalResults`) and a list of `CompanyUser` objects (`Data`).
        /// </returns>
        public async Task<ListResponseDTO<CompanyUserModel>> GetAll()
        {
            List<CompanyUserModel> data = await _context.CompanyUsers.ToListAsync();
            int totalResults = data.Count;

            var response = new ListResponseDTO<CompanyUserModel>
            {
                TotalResults = totalResults,
                Data = data
            };

            return response;
        }



        /// <summary>
        /// This C# function retrieves a CompanyUser entity by its accessId along with related Profile
        /// and UserEntitlements entities, and returns a CompanyUserEagerDTO object containing the
        /// retrieved data.
        /// </summary>
        /// <param name="accessId">The `accessId` parameter is used to uniquely identify a company user
        /// in the system. It is used to query the database and retrieve the company user information
        /// based on this identifier.</param>
        /// <returns>
        /// The method `GetByPk` returns a `Task` that resolves to a `CompanyUserEagerDTO` object or
        /// `null` if no matching record is found in the database.
        /// </returns>
        public async Task<CompanyUserEagerDTO?> GetByPk(string accessId)
        {
            var companyUser = await _context.CompanyUsers
                .Where(cu => cu.AccessId == accessId)
                .Include(cu => cu.Profile)
                .Include(cu => cu.UserEntitlements)
                .FirstOrDefaultAsync()
                ?? throw new ItemNotFoundException(accessId);

            var profileDTO = new ProfileSimpleDTO(companyUser.Profile!);
            var departmentDTO = (companyUser.Department == null)
                ? null : new DepartmentSimpleDTO(companyUser.Department);
            var userEntitlementDTOs = companyUser.UserEntitlements.Select(ue => new UserEntitlementSimpleDTO(ue)).ToList();

            return new CompanyUserEagerDTO(companyUser, profileDTO, departmentDTO, userEntitlementDTOs);
        }


        /// <summary>
        /// This C# async method retrieves a CompanyUserModel by primary key without tracking changes in
        /// the context.
        /// </summary>
        /// <param name="accessId">The `accessId` parameter is used to find a `CompanyUserModel` entity
        /// in the database based on its `AccessId` property.</param>
        /// <returns>
        /// The method `GetByPkNoTracking` returns a `Task` that will eventually yield a
        /// `CompanyUserModel` object or `null` if no matching record is found in the database. The
        /// method uses Entity Framework's `AsNoTracking` method to retrieve the entity without tracking
        /// changes and `FirstOrDefaultAsync` to asynchronously retrieve the first entity that matches
        /// the specified condition.
        /// </returns>
        public async Task<CompanyUserModel?> GetByPkNoTracking(string accessId)
        {
            return await _context.CompanyUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(cu => cu.AccessId == accessId);
        }


        /// <summary>
        /// The function `GetByPksNoTracking` retrieves a list of `CompanyUserModel` objects based on a
        /// list of accessIds without tracking changes.
        /// </summary>
        /// <param name="accessIds">The `GetByPksNoTracking` method retrieves a list of
        /// `CompanyUserModel` objects from the database based on the provided list of `accessIds`. The
        /// method filters the results to include only those `CompanyUserModel` objects whose `AccessId`
        /// property matches any of the `access</param>
        /// <returns>
        /// A list of CompanyUserModel objects that match the accessIds provided, without tracking
        /// changes in the context.
        /// </returns>
        public async Task<List<CompanyUserModel>> GetByPksNoTracking(List<string> accessIds)
        {
            return await _context.CompanyUsers
                .Where(companyUser => accessIds.Contains(companyUser.AccessId))
                .AsNoTracking()
                .ToListAsync();
        }


        /// <summary>
        /// The function `Post` in C# asynchronously adds a `CompanyUserModel` to the database after
        /// performing validation checks.
        /// </summary>
        /// <param name="postBody">The `Post` method you provided is an asynchronous method that
        /// handles the creation of a new `CompanyUserModel` entity in the database. Here's a breakdown
        /// of the method and its parameters:</param>
        public async Task Post(CompanyUserModel postBody)
        {
            if (await GetByPkNoTracking(postBody.AccessId) != null)
                throw new DuplicateException(postBody.AccessId);

            if (
                postBody.ProfileId != null
                && !await _dataAccessService.EntityExistsAsync<ProfileModel>(postBody.ProfileId))
            {
                throw new BadRequestException("Propiedades Invalidas, por favor revisar que el perfil exista en la base de datos");
            }

            _context.CompanyUsers.Add(postBody);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// This C# function updates a company user entity by primary key, handling exceptions and
        /// logging errors.
        /// </summary>
        /// <param name="accessId">The `accessId` parameter in the `UpdateByPK` method is a unique
        /// identifier used to locate the specific company user that needs to be updated. It is likely a
        /// string or an identifier that uniquely identifies the company user within the system.</param>
        /// <param name="updatedBody">CompanyUserSimpleDTO is a data transfer object (DTO) that
        /// represents a simplified version of a company user entity. It likely contains properties such
        /// as Id, Name, Email, ProfileId, etc. The UpdateByPK method is used to update an existing
        /// company user entity in the database based on a</param>
        /// <returns>
        /// The method `UpdateByPK` is returning a `Task` that resolves to a `CompanyUserSimpleDTO`
        /// object after updating the existing company user with the provided updated body.
        /// </returns>
        public async Task<CompanyUserSimpleDTO> UpdateByPK(string accessId, CompanyUserSimpleDTO updatedBody)
        {
            var existingCompanyUser = await GetByPkNoTracking(accessId)
                ?? throw new ItemNotFoundException(accessId);

            if (
                updatedBody.ProfileId != null
                && !await _dataAccessService.EntityExistsAsync<ProfileModel>(updatedBody.ProfileId))
            {
                throw new BadRequestException("Propiedades Invalidas, por favor revisar que el perfil exista en la base de datos");
            }

            _mapper.Map(updatedBody, existingCompanyUser);

            _context.Entry(existingCompanyUser).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new CompanyUserSimpleDTO(existingCompanyUser);
        }


        /// <summary>
        /// This C# async function deletes a company user by setting the `DeletedAt` property and
        /// updating the database.
        /// </summary>
        /// <param name="accessId">The `accessId` parameter in the `Delete` method is used to identify
        /// the company user that needs to be deleted. It is a unique identifier for the company user
        /// that is being targeted for deletion.</param>
        public async Task SoftDelete(string accessId)
        {
            var existingCompanyUser = await GetByPkNoTracking(accessId)
                ?? throw new ItemNotFoundException(accessId);

            existingCompanyUser.DeletedAt = DateTime.UtcNow;

            _context.CompanyUsers.Update(existingCompanyUser);

            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// The `Restore` function restores a company user by setting the `DeletedAt` property to null
        /// in C#.
        /// </summary>
        /// <param name="accessId">The `accessId` parameter in the `Restore` method is used to identify
        /// the company user that needs to be restored. It is passed to the method to locate the
        /// specific company user record that should be restored from a "soft deleted" state by setting
        /// the `DeletedAt` property to null.</param>
        public async Task Restore(string accessId)
        {
            var existingCompanyUser = await GetByPkNoTracking(accessId)
                ?? throw new ItemNotFoundException(accessId);

            existingCompanyUser.DeletedAt = null;

            _context.CompanyUsers.Update(existingCompanyUser);

            await _context.SaveChangesAsync();
        }
    }
}