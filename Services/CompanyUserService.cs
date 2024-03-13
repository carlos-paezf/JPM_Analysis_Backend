using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class CompanyUserService : IBaseService<CompanyUserModel, CompanyUserEagerDTO, CompanyUserSimpleDTO>
    {
        private readonly JPMDatabaseContext _context;
        private readonly ILogger<CompanyUserService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IMapper _mapper;

        public CompanyUserService(
            JPMDatabaseContext context,
            ILogger<CompanyUserService> logger,
            ErrorHandlingService errorHandlingService,
            IMapper mapper
        )
        {
            _context = context;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
            _mapper = mapper;
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
            try
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
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserService), methodName: nameof(GetAll));
                throw;
            }
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
            try
            {
                var companyUser = await _context.CompanyUsers
                                        .Where(a => a.AccessId == accessId)
                                        .Include(a => a.Profile)
                                        .Include(a => a.UserEntitlements)
                                        .FirstOrDefaultAsync()
                                        ?? throw new ItemNotFoundException(accessId);

                var profileDTO = new ProfileSimpleDTO(companyUser.Profile!);
                var userEntitlementDTOs = companyUser.UserEntitlements.Select(ue => new UserEntitlementSimpleDTO(ue)).ToList();

                return new CompanyUserEagerDTO(companyUser, profileDTO, userEntitlementDTOs);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserService), methodName: nameof(GetByPk));
                throw;
            }
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
            try
            {
                return await _context.CompanyUsers
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.AccessId == accessId);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserService), methodName: nameof(GetByPkNoTracking));
                throw;
            }
        }


        /// <summary>
        /// The function ProfileExistsAsync checks if a profile with a specific ID exists in the
        /// database asynchronously.
        /// </summary>
        /// <param name="profileId">The `profileId` parameter is a string representing the unique
        /// identifier of a profile that you want to check for existence in the database.</param>
        /// <returns>
        /// The method `ProfileExistsAsync` returns a `Task` of `bool` which represents an asynchronous
        /// operation that will eventually return a boolean value indicating whether a profile with the
        /// specified `profileId` exists in the database.
        /// </returns>
        private async Task<bool> ProfileExistsAsync(string profileId)
        {
            return await _context.Profiles
                            .FirstOrDefaultAsync(p => p.Id == profileId) != null;
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
            try
            {
                if (await GetByPkNoTracking(postBody.AccessId) != null) throw new DuplicateException(postBody.AccessId);

                if (postBody.ProfileId != null && !await ProfileExistsAsync(postBody.ProfileId))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el perfil exista en la base de datos");
                }

                _context.CompanyUsers.Add(postBody);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserService), methodName: nameof(Post));
                throw;
            }
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
            try
            {
                var existingCompanyUser = await GetByPkNoTracking(accessId) ?? throw new ItemNotFoundException(accessId);

                if (updatedBody.ProfileId != null && !await ProfileExistsAsync(updatedBody.ProfileId))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el perfil exista en la base de datos");
                }

                _mapper.Map(updatedBody, existingCompanyUser);

                _context.Entry(existingCompanyUser).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new CompanyUserSimpleDTO(existingCompanyUser);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserService), methodName: nameof(UpdateByPK));
                throw;
            }
        }


        /// <summary>
        /// This C# async function deletes a company user by setting the `DeletedAt` property and
        /// updating the database.
        /// </summary>
        /// <param name="accessId">The `accessId` parameter in the `Delete` method is used to identify
        /// the company user that needs to be deleted. It is a unique identifier for the company user
        /// that is being targeted for deletion.</param>
        public async Task Delete(string accessId)
        {
            try
            {
                var existingCompanyUser = await GetByPkNoTracking(accessId) ?? throw new ItemNotFoundException(accessId);

                existingCompanyUser.DeletedAt = DateTime.UtcNow;

                _context.CompanyUsers.Update(existingCompanyUser);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserService), methodName: nameof(Delete));
                throw;
            }
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
            try
            {
                var existingCompanyUser = await GetByPkNoTracking(accessId) ?? throw new ItemNotFoundException(accessId);

                existingCompanyUser.DeletedAt = null;

                _context.CompanyUsers.Update(existingCompanyUser);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(CompanyUserService), methodName: nameof(Restore));
                throw;
            }
        }
    }
}