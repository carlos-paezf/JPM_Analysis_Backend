using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class ProfileFunctionService
        : IBaseService<ProfileFunctionModel, ProfileFunctionEagerDTO, ProfileFunctionSimpleDTO>
            , IHardDeleteService
            , IBulkPostService<ProfileFunctionModel>
            , IBulkHardDeleteService
    {
        private readonly JPMDatabaseContext _context;
        private readonly ILogger<ProfileFunctionService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IMapper _mapper;

        public ProfileFunctionService(
            JPMDatabaseContext context,
            ILogger<ProfileFunctionService> logger,
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
        /// Retrieves a list of all profile functions.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing a <c>ListResponseDTO</c> of <c>ProfileFunctionModel</c> objects.
        /// If successful, the response includes the total count of records and the list of profile functions.
        /// If an exception occurs during the process, it returns an Internal Server Error alert.
        /// </returns>
        public async Task<ListResponseDTO<ProfileFunctionModel>> GetAll()
        {
            try
            {
                List<ProfileFunctionModel> data = await _context.ProfilesFunctions.ToListAsync();
                int totalResults = data.Count;

                var response = new ListResponseDTO<ProfileFunctionModel>
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
                    className: nameof(ProfileFunctionService), methodName: nameof(GetAll));
                throw;
            }
        }


        /// <summary>
        /// Retrieves a profile function by its primary key.
        /// </summary>
        /// <param name="id">The unique identifier of the profile function.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing the <c>ProfileFunctionEagerDTO</c> object representing the profile function.
        /// If successful, it returns the profile function with related entities (Function and Profile).
        /// If the profile function is not found, it returns a Not Found error.
        /// If an exception occurs during the process, it returns an Internal Server Error alert.
        /// </returns>
        public async Task<ProfileFunctionEagerDTO?> GetByPk(string id)
        {
            try
            {
                var profileFunction = await _context.ProfilesFunctions
                                        .Where(pf => pf.Id == id)
                                        .Include(p => p.Function)
                                        .Include(p => p.Profile)
                                        .FirstOrDefaultAsync()
                                        ?? throw new ItemNotFoundException(id);

                var functionDTO = new FunctionSimpleDTO(profileFunction.Function!);
                var profileDTO = new ProfileSimpleDTO(profileFunction.Profile!);

                return new ProfileFunctionEagerDTO(profileFunction, functionDTO, profileDTO);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionService), methodName: nameof(GetByPk));
                throw;
            }
        }


        /// <summary>
        /// Retrieves a profile function by its primary key without tracking changes.
        /// </summary>
        /// <param name="id">The unique identifier of the profile function.</param>
        /// <returns>
        /// A <c>ProfileFunctionModel</c> object representing the profile function.
        /// If successful, it returns the profile function.
        /// If the profile function is not found, it returns null.
        /// If an exception occurs during the process, it returns an Internal Server Error alert.
        /// </returns>
        public async Task<ProfileFunctionModel?> GetByPkNoTracking(string id)
        {
            try
            {
                return await _context.ProfilesFunctions
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionService), methodName: nameof(GetByPkNoTracking));
                throw;
            }
        }


        private async Task<bool> ProfileExistsAsync(string producId)
        {
            return await _context.Profiles
                            .FirstOrDefaultAsync(p => p.Id == producId) != null;
        }


        private async Task<bool> FunctionExistsAsync(string functionId)
        {
            return await _context.Functions
                            .FirstOrDefaultAsync(a => a.Id == functionId) != null;
        }


        /// <summary>
        /// Creates a new profile function.
        /// </summary>
        /// <param name="postBody">The data for the new profile function.</param>
        /// <returns>
        /// No return value.
        /// If successful, it creates the profile function.
        /// If the provided profile or function ID is invalid or a duplicate entry already exists,
        /// it returns a Bad Request error.
        /// If an exception occurs during the process, it returns an Internal Server Error alert.
        /// </returns>
        public async Task Post(ProfileFunctionModel postBody)
        {
            try
            {
                var existingEntity = await _context.ProfilesFunctions
                                    .FirstOrDefaultAsync(pf => pf.ProfileId == postBody.ProfileId && pf.FunctionId == postBody.FunctionId);

                if (existingEntity != null) throw new DuplicateException(postBody.ToString()!);

                if ((postBody.ProfileId != null && !await ProfileExistsAsync(postBody.ProfileId))
                    || (postBody.FunctionId != null && !await FunctionExistsAsync(postBody.FunctionId)))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el perfil o la función existan en la base de datos");
                }

                _context.ProfilesFunctions.Add(postBody);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionService), methodName: nameof(Post));
                throw;
            }
        }


        /// <summary>
        /// Updates a profile function by its primary key.
        /// </summary>
        /// <param name="id">The unique identifier of the profile function to update.</param>
        /// <param name="updatedBody">The updated data for the profile function.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing the <c>ProfileFunctionSimpleDTO</c> object representing the updated profile function.
        /// If successful, it returns the updated profile function.
        /// If the provided profile or function ID is invalid or a duplicate entry already exists,
        /// it returns a Bad Request error.
        /// If the profile function to update is not found, it returns a Not Found error.
        /// If an exception occurs during the process, it returns an Internal Server Error alert.
        /// </returns>
        public async Task<ProfileFunctionSimpleDTO> UpdateByPK(string id, ProfileFunctionSimpleDTO updatedBody)
        {
            try
            {
                var existingProfileFunction = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id);

                var existingEntity = await _context.ProfilesFunctions
                                                    .FirstOrDefaultAsync(pf => pf.ProfileId == updatedBody.ProfileId && pf.FunctionId == updatedBody.FunctionId);

                if (existingEntity != null) throw new DuplicateException(updatedBody.ToString()!);

                if ((updatedBody.ProfileId != null && !await ProfileExistsAsync(updatedBody.ProfileId))
                                    || (updatedBody.FunctionId != null && !await FunctionExistsAsync(updatedBody.FunctionId)))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el perfil o la función existan en la base de datos");
                }

                _mapper.Map(updatedBody, existingProfileFunction);

                _context.Entry(existingProfileFunction).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new ProfileFunctionSimpleDTO(existingProfileFunction);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionService), methodName: nameof(UpdateByPK));
                throw;
            }
        }


        /// <summary>
        /// Deletes a profile function by its primary key.
        /// </summary>
        /// <param name="id">The unique identifier of the profile function to delete.</param>
        /// <returns>
        /// No return value.
        /// If successful, it deletes the profile function.
        /// If the profile function to delete is not found, it returns a Not Found error.
        /// If an exception occurs during the process, it returns an Internal Server Error alert.
        /// </returns>
        public async Task HardDelete(string id)
        {
            try
            {
                var existingProfileFunction = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id);

                _context.ProfilesFunctions.Remove(existingProfileFunction);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionService), methodName: nameof(HardDelete));
                throw;
            }
        }


        /// <summary>
        /// Creates multiple profile functions in bulk.
        /// </summary>
        /// <param name="collectionBody">The collection of profile functions to create.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing a <c>ListResponseDTO</c> of <c>ProfileFunctionModel</c> objects.
        /// If successful, it creates the profile functions and returns the total count of records created.
        /// If any of the provided profile or function IDs is invalid or if duplicate entries are found,
        /// it returns a Bad Request error.
        /// If an exception occurs during the process, it returns an Internal Server Error alert.
        /// </returns>
        public async Task<ListResponseDTO<ProfileFunctionModel>> BulkPost(ICollection<ProfileFunctionModel> collectionBody)
        {
            try
            {
                foreach (var profileFunction in collectionBody)
                {
                    if (!await ProfileExistsAsync(profileFunction.FunctionId) || !await FunctionExistsAsync(profileFunction.FunctionId))
                    {
                        throw new BadRequestException($"Propiedades Invalidas, por favor revisar que el perfil o la función existan en la base de datos para el registro '{profileFunction.Id}'");
                    }
                }

                var existingEntries = await _context.ProfilesFunctions.ToListAsync();

                var duplicateEntries = collectionBody.Join(
                                                        existingEntries,
                                                        newEntry => newEntry.Id,
                                                        existingEntry => existingEntry.Id,
                                                        (newEntry, existingEntry) => newEntry);

                if (duplicateEntries.Any())
                {
                    throw new BadRequestException("Se encontraron registros duplicados en la base de datos");
                }

                await _context.ProfilesFunctions.AddRangeAsync(collectionBody);
                await _context.SaveChangesAsync();

                int totalResults = collectionBody.Count;

                var response = new ListResponseDTO<ProfileFunctionModel>
                {
                    TotalResults = totalResults,
                    Data = (List<ProfileFunctionModel>)collectionBody
                };

                return response;
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionService), methodName: nameof(BulkPost));
                throw;
            }
        }


        /// <summary>
        /// Deletes multiple profile functions in bulk.
        /// </summary>
        /// <param name="pks">The collection of primary keys (IDs) of profile functions to delete.</param>
        /// <returns>
        /// The number of profile functions deleted.
        /// If successful, it deletes the profile functions and returns the count of records affected.
        /// If any of the provided primary keys do not exist, it returns a Bad Request error.
        /// If an exception occurs during the process, it returns an Internal Server Error alert.
        /// </returns>
        public async Task<int> BulkHardDelete(ICollection<string> pks)
        {
            try
            {
                var entitiesToDelete = await _context.ProfilesFunctions
                                                        .Where(pf => pks.Contains(pf.Id))
                                                        .ToListAsync();

                var nonExistentPks = pks.Except(entitiesToDelete.Select(pf => pf.Id));
                if (nonExistentPks.Any())
                {
                    throw new BadRequestException($"Los siguientes PKs no existen en la base de datos: {string.Join(", ", nonExistentPks)}");
                }

                _context.ProfilesFunctions.RemoveRange(entitiesToDelete);
                var affectedRecordsCount = await _context.SaveChangesAsync();

                return affectedRecordsCount;
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionService), methodName: nameof(BulkHardDelete));
                throw;
            }
        }
    }
}