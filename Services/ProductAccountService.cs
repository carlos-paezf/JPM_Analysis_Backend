using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class ProductAccountService
        : IBaseService<ProductAccountModel, ProductAccountEagerDTO, ProductAccountSimpleDTO>
            , ISoftDeleteService
    {
        private readonly JPMDatabaseContext _context;
        private readonly ILogger<ProductAccountService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IMapper _mapper;

        public ProductAccountService(
            JPMDatabaseContext context,
            ILogger<ProductAccountService> logger,
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
        /// The function GetAll retrieves all clients from the database and returns them in a
        /// ListResponse object.
        /// </summary>
        /// <returns>
        /// The method `GetAll` is returning a `Task` that will eventually resolve to a
        /// `ListResponse` of `ProductAccount`. The `ListResponse` of `ProductAccount` object contains the total number of
        /// results (`TotalResults`) and a list of `ProductAccount` objects (`Data`).
        /// </returns>
        public async Task<ListResponseDTO<ProductAccountModel>> GetAll()
        {
            try
            {
                List<ProductAccountModel> data = await _context.ProductsAccounts.ToListAsync();
                int totalResults = data.Count;

                var response = new ListResponseDTO<ProductAccountModel>
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
                    className: nameof(ProductAccountService), methodName: nameof(GetAll));
                throw;
            }
        }


        /// <summary>
        /// The function retrieves client details along with associated clients and user entitlements
        /// based on the provided client number.
        /// </summary>
        /// <param name="id">
        /// The ProductAccount Number is the unique identifier for each client
        /// </param>
        /// <returns>
        /// The `GetByPk` method is returning an `ProductAccountDetailsDTO` object or `null` if no matching
        /// client is found in the database. The `ProductAccountDetailsDTO` object contains details about the
        /// client, including client number, client name, client type, a list of clients associated
        /// with the client (represented as `ProductAccountDTO` objects), and a list of user entitlements
        /// associated with the client (represented as `UserEntitlementsDTO` objects).
        /// </returns>
        public async Task<ProductAccountEagerDTO?> GetByPk(string id)
        {
            try
            {
                var client = await _context.ProductsAccounts
                                        .Include(c => c.Product)
                                        .Include(c => c.Account)
                                        .FirstOrDefaultAsync(c => c.Id == id)
                                        ?? throw new ItemNotFoundException(id.ToString());

                var productDTO = (client.Product == null) ? null : new ProductSimpleDTO(client.Product);
                var accountDTO = (client.Account == null) ? null : new AccountSimpleDTO(client.Account);

                return new ProductAccountEagerDTO(client, productDTO, accountDTO);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductAccountService), methodName: nameof(GetByPk));
                throw;
            }
        }


        /// <summary>
        /// This C# async method retrieves an ProductAccount entity by its primary key without tracking changes
        /// in the context.
        /// </summary>
        /// <param name="id">The `pk` parameter in the `GetByPkNoTracking` method is a string
        /// representing the primary key value used to retrieve an `ProductAccount` entity from the
        /// database.</param>
        /// <returns>
        /// The method `GetByPkNoTracking` is returning a `Task` that may contain an `ProductAccount` object or
        /// `null`.
        /// </returns>
        public async Task<ProductAccountModel?> GetByPkNoTracking(string id)
        {
            try
            {
                return await _context.ProductsAccounts
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductAccountService), methodName: nameof(GetByPkNoTracking));
                throw;
            }
        }


        private async Task<bool> ProductExistsAsync(string producId)
        {
            return await _context.Products
                            .FirstOrDefaultAsync(p => p.Id == producId) != null;
        }

        private async Task<bool> AccountExistsAsync(string accountNumber)
        {
            return await _context.Accounts
                            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber) != null;
        }

        /// <summary>
        /// The function `Post` asynchronously adds a new client to the database if it doesn't already
        /// exist and logs any errors that occur.
        /// </summary>
        /// <param name="postBody">
        /// The `Post` method you provided is an asynchronous method that attempts
        /// to create a new client in a database. Here's a breakdown of the method:</param>
        /// <returns>
        /// The `Post` method returns a `Task` of `bool`. If a new client is successfully added to the
        /// database, it returns `true`. If an client with the same client number already exists, it
        /// returns `false`. If an exception occurs during the process, it will log the error and
        /// rethrow the exception.
        /// </returns>
        public async Task Post(ProductAccountModel postBody)
        {
            try
            {
                var existingEntity = await _context.ProductsAccounts
                                    .FirstOrDefaultAsync(c => c.ProductId == postBody.ProductId && c.AccountNumber == postBody.AccountNumber);

                if (existingEntity != null) throw new DuplicateException($"{postBody.ProductId}-{postBody.AccountNumber}");

                if ((postBody.ProductId != null && !await ProductExistsAsync(postBody.ProductId))
                    || (postBody.AccountNumber != null && !await AccountExistsAsync(postBody.AccountNumber)))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el producto o la cuenta existan en la base de datos");
                }

                _context.ProductsAccounts.Add(postBody);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductAccountService), methodName: nameof(Post));
                throw;
            }
        }


        /// <summary>
        /// This C# function updates an client entity by its primary key using asynchronous operations
        /// and error handling.
        /// </summary>
        /// <param name="id">The `pk` parameter in the `UpdateByPK` method likely stands for the primary
        /// key of the client entity that you want to update. It is used to identify the specific
        /// client record that needs to be updated in the database.</param>
        /// <param name="updatedBody">ProductAccountSimpleDTO is a data transfer object (DTO) that
        /// represents a simplified version of an client entity. It is used for transferring
        /// client-related data between different parts of the application, such as between the client
        /// and the server or between different layers of the application. In the provided code snippet,
        /// the Update</param>
        /// <returns>
        /// The method `UpdateByPK` returns an `ProductAccountSimpleDTO` object after updating an existing
        /// client entity in the database.
        /// </returns>
        public async Task<ProductAccountSimpleDTO> UpdateByPK(string id, ProductAccountSimpleDTO updatedBody)
        {
            try
            {
                var existingProductAccount = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id.ToString());

                if ((updatedBody.ProductId != null && !await ProductExistsAsync(updatedBody.ProductId))
                    || (updatedBody.AccountNumber != null && !await AccountExistsAsync(updatedBody.AccountNumber)))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el producto o la cuenta existan en la base de datos");
                }

                _mapper.Map(updatedBody, existingProductAccount);

                _context.Entry(existingProductAccount).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new ProductAccountSimpleDTO(existingProductAccount);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductAccountService), methodName: nameof(UpdateByPK));
                throw;
            }
        }


        /// <summary>
        /// The Delete method asynchronously deletes an client by setting the DeletedAt property to the
        /// current time and saving changes to the database, handling exceptions and logging errors if
        /// they occur.
        /// </summary>
        /// <param name="id">The `pk` parameter in the `Delete` method is a string representing the
        /// primary key of the client that needs to be deleted.</param>
        public async Task SoftDelete(string id)
        {
            try
            {
                var existingProductAccount = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id.ToString());

                existingProductAccount.DeletedAt = DateTime.UtcNow;

                _context.ProductsAccounts.Update(existingProductAccount);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductAccountService), methodName: nameof(SoftDelete));
                throw;
            }
        }


        /// <summary>
        /// The `Restore` function restores a deleted client by setting its `DeletedAt` property to
        /// null.
        /// </summary>
        /// <param name="id">The `pk` parameter in the `Restore` method is a string representing the
        /// primary key of the client that needs to be restored.</param>
        public async Task Restore(string id)
        {
            try
            {
                var existingProductAccount = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id.ToString());

                existingProductAccount.DeletedAt = null;

                _context.ProductsAccounts.Update(existingProductAccount);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductAccountService), methodName: nameof(Restore));
                throw;
            }
        }
    }
}
