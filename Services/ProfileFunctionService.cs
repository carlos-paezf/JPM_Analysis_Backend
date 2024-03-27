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
        private readonly IMapper _mapper;
        public readonly DataAccessService _dataAccessService;

        public ProfileFunctionService(
            JPMDatabaseContext context,
            DataAccessService dataAccessService,
            IMapper mapper
        )
        {
            _context = context;
            _dataAccessService = dataAccessService;
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
            List<ProfileFunctionModel> data = await _context.ProfilesFunctions.ToListAsync();
            int totalResults = data.Count;

            var response = new ListResponseDTO<ProfileFunctionModel>
            {
                TotalResults = totalResults,
                Data = data
            };

            return response;
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
            return await _context.ProfilesFunctions
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
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
            var existingEntity = await _context.ProfilesFunctions
                .FirstOrDefaultAsync(pf => pf.ProfileId == postBody.ProfileId
                    && pf.FunctionId == postBody.FunctionId);

            if (existingEntity != null)
                throw new DuplicateException(postBody.ToString()!);

            if ((postBody.ProfileId != null && !await _dataAccessService.EntityExistsAsync<ProfileModel>(postBody.ProfileId))
                || (postBody.FunctionId != null && !await _dataAccessService.EntityExistsAsync<FunctionModel>(postBody.FunctionId)))
            {
                throw new BadRequestException("Propiedades Invalidas, por favor revisar que el perfil o la función existan en la base de datos");
            }

            _context.ProfilesFunctions.Add(postBody);
            await _context.SaveChangesAsync();
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
            var existingProfileFunction = await GetByPkNoTracking(id)
                ?? throw new ItemNotFoundException(id);

            var existingEntity = await _context.ProfilesFunctions
                .FirstOrDefaultAsync(pf => pf.ProfileId == updatedBody.ProfileId && pf.FunctionId == updatedBody.FunctionId);

            if (existingEntity != null)
                throw new DuplicateException(updatedBody.ToString()!);

            if ((updatedBody.ProfileId != null && !await _dataAccessService.EntityExistsAsync<ProfileModel>(updatedBody.ProfileId))
                || (updatedBody.FunctionId != null && !await _dataAccessService.EntityExistsAsync<FunctionModel>(updatedBody.FunctionId)))
            {
                throw new BadRequestException("Propiedades Invalidas, por favor revisar que el perfil o la función existan en la base de datos");
            }

            _mapper.Map(updatedBody, existingProfileFunction);

            _context.Entry(existingProfileFunction).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new ProfileFunctionSimpleDTO(existingProfileFunction);
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
            var existingProfileFunction = await GetByPkNoTracking(id)
                ?? throw new ItemNotFoundException(id);

            _context.ProfilesFunctions.Remove(existingProfileFunction);

            await _context.SaveChangesAsync();
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
            var profilesIds = collectionBody.Select(pf => pf.ProfileId).Distinct().ToList();
            var functionsIds = collectionBody.Select(pf => pf.FunctionId).Distinct().ToList();

            var existingProfiles = await _dataAccessService.CheckExistingEntitiesAsync<ProfileModel>(profilesIds);
            var existingFunctions = await _dataAccessService.CheckExistingEntitiesAsync<FunctionModel>(functionsIds);

            var invalidProfilesIds = profilesIds.Where(id => !existingProfiles.Contains(id)).ToList();
            var invalidFunctionsIds = functionsIds.Where(id => !existingFunctions.Contains(id)).ToList();

            var invalidIds = invalidProfilesIds.Concat(invalidFunctionsIds).Distinct().ToList();

            if (invalidIds.Any())
            {
                throw new BadRequestException($"Propiedades Invalidas, por favor revisar que el perfil o la función existan en la base de datos para los registros con Ids '{string.Join(", ", invalidIds)}'");
            }

            var existingProfileFunctionIds = await _context.ProfilesFunctions.Select(pf => pf.Id).ToListAsync();
            var duplicateEntries = collectionBody.Where(pf => existingProfileFunctionIds.Contains(pf.Id)).ToList();

            if (duplicateEntries.Any())
            {
                throw new BadRequestException($"Se encontraron {duplicateEntries.Count()} registros duplicados en la base de datos para 'profiles_functions'. Estos registros ya existen y no pueden ser agregados nuevamente.");
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
    }
}