using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class ProfileService : IBaseService<ProfileModel, ProfileEagerDTO, ProfileSimpleDTO>
    {
        private readonly JPMDatabaseContext _context;
        private readonly ILogger<ProfileService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IMapper _mapper;

        public ProfileService(
            JPMDatabaseContext context,
            ILogger<ProfileService> logger,
            ErrorHandlingService errorHandlingService,
            IMapper mapper
        )
        {
            _context = context;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
            _mapper = mapper;
        }


        public async Task<ListResponseDTO<ProfileModel>> GetAll()
        {
            try
            {
                List<ProfileModel> data = await _context.Profiles.ToListAsync();
                int totalResults = data.Count;

                var response = new ListResponseDTO<ProfileModel>
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
                    className: nameof(ProfileService), methodName: nameof(GetAll));
                throw;
            }
        }


        public async Task<ProfileEagerDTO?> GetByPk(string id)
        {
            try
            {
                var function = await _context.Profiles
                                        .Where(p => p.Id == id)
                                        .Include(p => p.CompanyUsers)
                                        .Include(p => p.ProfilesFunctions)
                                        .FirstOrDefaultAsync()
                                        ?? throw new ItemNotFoundException(id);

                var companyUserDTOs = function.CompanyUsers.Select(cu => new CompanyUserSimpleDTO(cu)).ToList();
                var profilesFunctionsDTO = function.ProfilesFunctions.Select(pf => new ProfilesFunctionSimpleDTO(pf)).ToList();

                return new ProfileEagerDTO(function, companyUserDTOs, profilesFunctionsDTO);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileService), methodName: nameof(GetByPk));
                throw;
            }
        }


        public async Task<ProfileModel?> GetByPkNoTracking(string id)
        {
            try
            {
                return await _context.Profiles
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileService), methodName: nameof(GetByPkNoTracking));
                throw;
            }
        }


        public async Task Post(ProfileModel postBody)
        {
            try
            {
                if (await GetByPkNoTracking(postBody.Id) != null) throw new DuplicateException(postBody.Id);

                _context.Profiles.Add(postBody);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileService), methodName: nameof(Post));
                throw;
            }
        }


        public async Task<ProfileSimpleDTO> UpdateByPK(string id, ProfileSimpleDTO updatedBody)
        {
            try
            {
                var existingProfile = await _context.Profiles
                                                        .FirstOrDefaultAsync(f => f.Id == id)
                                                        ?? throw new ItemNotFoundException(id); ;

                _mapper.Map(updatedBody, existingProfile);

                _context.Entry(existingProfile).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new ProfileSimpleDTO(existingProfile);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionService), methodName: nameof(UpdateByPK));
                throw;
            }
        }
    }
}