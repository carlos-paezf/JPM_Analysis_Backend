using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class UserEntitlementService
        : IBaseService<UserEntitlementModel, UserEntitlementEagerDTO, UserEntitlementSimpleDTO>
            , ISoftDeleteService
    {
        private readonly JPMDatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly DataAccessService _dataAccessService;


        public UserEntitlementService(
            JPMDatabaseContext context,
            IMapper mapper,
            DataAccessService dataAccessService
        )
        {
            _context = context;
            _mapper = mapper;
            _dataAccessService = dataAccessService;
        }


        public async Task<ListResponseDTO<UserEntitlementModel>> GetAll()
        {
            List<UserEntitlementModel> data = await _context.UserEntitlements.ToListAsync();
            int totalResults = data.Count;

            var response = new ListResponseDTO<UserEntitlementModel>
            {
                TotalResults = totalResults,
                Data = data
            };

            return response;
        }


        public async Task<UserEntitlementEagerDTO?> GetByPk(string id)
        {
            var userEntitlement = await _context.UserEntitlements
                .Where(ue => ue.Id == id)
                .Include(ue => ue.CompanyUser)
                .Include(ue => ue.Account)
                .Include(ue => ue.Product)
                .Include(ue => ue.Function)
                .FirstOrDefaultAsync()
                ?? throw new ItemNotFoundException(id);

            var companyUserDTO = new CompanyUserSimpleDTO(userEntitlement.CompanyUser!);
            var productDTO = new ProductSimpleDTO(userEntitlement.Product!);
            var accountDTO = (userEntitlement.Account == null)
                ? null : new AccountSimpleDTO(userEntitlement.Account);
            var functionDTO = (userEntitlement.Function == null)
                ? null : new FunctionSimpleDTO(userEntitlement.Function);

            return new UserEntitlementEagerDTO(userEntitlement, companyUserDTO, accountDTO, productDTO, functionDTO);
        }


        public async Task<UserEntitlementModel?> GetByPkNoTracking(string id)
        {
            return await _context.UserEntitlements
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }


        public async Task Post(UserEntitlementModel postBody)
        {
            var existingEntity = await _context.UserEntitlements
                .FirstOrDefaultAsync(ue =>
                    ue.AccessId == postBody.AccessId
                    && ue.AccountNumber == postBody.AccountNumber
                    && ue.ProductId == postBody.ProductId
                    && ue.FunctionId == postBody.FunctionId
                    && ue.FunctionId == postBody.FunctionType
                );

            if (existingEntity != null) throw new DuplicateException(postBody.Id);

            if ((postBody.AccessId != null && !await _dataAccessService.EntityExistsAsync<CompanyUserModel>(postBody.AccessId))
                || (postBody.AccountNumber != null && !await _dataAccessService.EntityExistsAsync<AccountModel>(postBody.AccountNumber))
                || (postBody.ProductId != null && !await _dataAccessService.EntityExistsAsync<ProductModel>(postBody.ProductId))
                || (postBody.FunctionId != null && !await _dataAccessService.EntityExistsAsync<FunctionModel>(postBody.FunctionId)))
            {
                throw new BadRequestException("Propiedades Invalidas, por favor revisar que el el usuario, cuenta, producto o función existan en la base de datos");
            }

            _context.UserEntitlements.Add(postBody);
            await _context.SaveChangesAsync();
        }


        public async Task<UserEntitlementSimpleDTO> UpdateByPK(string id, UserEntitlementSimpleDTO updatedBody)
        {
            var existingUserEntitlement = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id);

            var existingEntity = await _context.UserEntitlements
                .FirstOrDefaultAsync(ue =>
                    ue.AccessId == updatedBody.AccessId
                    && ue.AccountNumber == updatedBody.AccountNumber
                    && ue.ProductId == updatedBody.ProductId
                    && ue.FunctionId == updatedBody.FunctionId
                    && ue.FunctionId == updatedBody.FunctionType
                );

            if (existingEntity != null) throw new DuplicateException(updatedBody.ToString()!);

            if ((updatedBody.AccessId != null && !await _dataAccessService.EntityExistsAsync<CompanyUserModel>(updatedBody.AccessId))
               || (updatedBody.AccountNumber != null && !await _dataAccessService.EntityExistsAsync<AccountModel>(updatedBody.AccountNumber))
               || (updatedBody.ProductId != null && !await _dataAccessService.EntityExistsAsync<ProductModel>(updatedBody.ProductId))
               || (updatedBody.FunctionId != null && !await _dataAccessService.EntityExistsAsync<FunctionModel>(updatedBody.FunctionId)))
            {
                throw new BadRequestException("Propiedades Invalidas, por favor revisar que el usuario, cuenta, producto o función existan en la base de datos");
            }

            _mapper.Map(updatedBody, existingUserEntitlement);

            _context.Entry(existingUserEntitlement).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new UserEntitlementSimpleDTO(existingUserEntitlement);
        }


        public async Task SoftDelete(string id)
        {
            var existingUserEntitlement = await GetByPkNoTracking(id)
                ?? throw new ItemNotFoundException(id);

            existingUserEntitlement.DeletedAt = DateTime.UtcNow;

            _context.UserEntitlements.Update(existingUserEntitlement);

            await _context.SaveChangesAsync();
        }


        public async Task Restore(string id)
        {
            var existingUserEntitlement = await GetByPkNoTracking(id)
                ?? throw new ItemNotFoundException(id);

            existingUserEntitlement.DeletedAt = null;

            _context.UserEntitlements.Update(existingUserEntitlement);

            await _context.SaveChangesAsync();
        }
    }
}