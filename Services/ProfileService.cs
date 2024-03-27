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
        private readonly IMapper _mapper;

        public ProfileService(
            JPMDatabaseContext context,
            IMapper mapper
        )
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<ListResponseDTO<ProfileModel>> GetAll()
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


        public async Task<ProfileEagerDTO?> GetByPk(string id)
        {
            var function = await _context.Profiles
                .Where(p => p.Id == id)
                .Include(p => p.CompanyUsers)
                .Include(p => p.ProfilesFunctions)
                .FirstOrDefaultAsync()
                ?? throw new ItemNotFoundException(id);

            var companyUserDTOs = function.CompanyUsers.Select(cu => new CompanyUserSimpleDTO(cu)).ToList();
            var profilesFunctionsDTO = function.ProfilesFunctions.Select(pf => new ProfileFunctionSimpleDTO(pf)).ToList();

            return new ProfileEagerDTO(function, companyUserDTOs, profilesFunctionsDTO);
        }


        public async Task<ProfileModel?> GetByPkNoTracking(string id)
        {
            return await _context.Profiles
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }


        public async Task Post(ProfileModel postBody)
        {
            if (await GetByPkNoTracking(postBody.Id) != null)
                throw new DuplicateException(postBody.Id);

            _context.Profiles.Add(postBody);
            await _context.SaveChangesAsync();
        }


        public async Task<ProfileSimpleDTO> UpdateByPK(string id, ProfileSimpleDTO updatedBody)
        {
            var existingProfile = await _context.Profiles
                .FirstOrDefaultAsync(f => f.Id == id)
                ?? throw new ItemNotFoundException(id); ;

            _mapper.Map(updatedBody, existingProfile);

            _context.Entry(existingProfile).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new ProfileSimpleDTO(existingProfile);
        }
    }
}