using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;


namespace BackendJPMAnalysis.Services
{
    public class UserEntitlementService
        : IBaseService<UserEntitlementModel, UserEntitlementEagerDTO, UserEntitlementSimpleDTO>
            , ISoftDeleteService
    {
        private readonly JPMDatabaseContext _context;
        private readonly ILogger<UserEntitlementService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IMapper _mapper;

        public UserEntitlementService(
            JPMDatabaseContext context,
            ILogger<UserEntitlementService> logger,
            ErrorHandlingService errorHandlingService,
            IMapper mapper
        )
        {
            _context = context;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
            _mapper = mapper;
        }


        public async Task<ListResponseDTO<UserEntitlementModel>> GetAll()
        {
            try
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
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementService), methodName: nameof(GetAll));
                throw;
            }
        }


        public async Task<UserEntitlementEagerDTO?> GetByPk(string id)
        {
            try
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
                var accountDTO = (userEntitlement.Account == null) ? null : new AccountSimpleDTO(userEntitlement.Account);
                var functionDTO = (userEntitlement.Function == null) ? null : new FunctionSimpleDTO(userEntitlement.Function);

                return new UserEntitlementEagerDTO(userEntitlement, companyUserDTO, accountDTO, productDTO, functionDTO);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementService), methodName: nameof(GetByPk));
                throw;
            }
        }


        public async Task<UserEntitlementModel?> GetByPkNoTracking(string id)
        {
            try
            {
                return await _context.UserEntitlements
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementService), methodName: nameof(GetByPkNoTracking));
                throw;
            }
        }


        private async Task<bool> CompanyUserExistsAsync(string accessId)
        {
            return await _context.CompanyUsers
                            .FirstOrDefaultAsync(cu => cu.AccessId == accessId) != null;
        }

        private async Task<bool> AccountExistsAsync(string accountNumber)
        {
            return await _context.Accounts
                            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber) != null;
        }

        private async Task<bool> ProductExistsAsync(string producId)
        {
            return await _context.Products
                            .FirstOrDefaultAsync(p => p.Id == producId) != null;
        }

        private async Task<bool> FunctionExistsAsync(string functionId)
        {
            return await _context.Functions
                            .FirstOrDefaultAsync(f => f.Id == functionId) != null;
        }


        public async Task Post(UserEntitlementModel postBody)
        {
            try
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

                if ((postBody.AccessId != null && !await CompanyUserExistsAsync(postBody.AccessId))
                    || (postBody.AccountNumber != null && !await AccountExistsAsync(postBody.AccountNumber))
                    || (postBody.ProductId != null && !await ProductExistsAsync(postBody.ProductId))
                    || (postBody.FunctionId != null && !await FunctionExistsAsync(postBody.FunctionId)))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el el usuario, cuenta, producto o función existan en la base de datos");
                }

                _context.UserEntitlements.Add(postBody);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementService), methodName: nameof(Post));
                throw;
            }
        }

        public async Task<UserEntitlementSimpleDTO> UpdateByPK(string id, UserEntitlementSimpleDTO updatedBody)
        {
            try
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

                if ((updatedBody.AccessId != null && !await CompanyUserExistsAsync(updatedBody.AccessId))
                   || (updatedBody.AccountNumber != null && !await AccountExistsAsync(updatedBody.AccountNumber))
                   || (updatedBody.ProductId != null && !await ProductExistsAsync(updatedBody.ProductId))
                   || (updatedBody.FunctionId != null && !await FunctionExistsAsync(updatedBody.FunctionId)))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el usuario, cuenta, producto o función existan en la base de datos");
                }

                _mapper.Map(updatedBody, existingUserEntitlement);

                _context.Entry(existingUserEntitlement).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new UserEntitlementSimpleDTO(existingUserEntitlement);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementService), methodName: nameof(UpdateByPK));
                throw;
            }
        }


        public async Task SoftDelete(string id)
        {
            try
            {
                var existingUserEntitlement = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id);

                existingUserEntitlement.DeletedAt = DateTime.UtcNow;

                _context.UserEntitlements.Update(existingUserEntitlement);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementService), methodName: nameof(SoftDelete));
                throw;
            }
        }


        public async Task Restore(string id)
        {
            try
            {
                var existingUserEntitlement = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id);

                existingUserEntitlement.DeletedAt = null;

                _context.UserEntitlements.Update(existingUserEntitlement);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(UserEntitlementService), methodName: nameof(Restore));
                throw;
            }
        }
    }
}