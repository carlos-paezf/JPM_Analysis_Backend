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
        private readonly IMapper _mapper;
        private readonly DataAccessService _dataAccessService;


        public ProductAccountService(
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
            List<ProductAccountModel> data = await _context.ProductsAccounts.ToListAsync();
            int totalResults = data.Count;

            var response = new ListResponseDTO<ProductAccountModel>
            {
                TotalResults = totalResults,
                Data = data
            };

            return response;
        }


        /// <summary>
        /// Retrieves all ProductAccount entities from the database eagerly loading related Account and Product entities, and returns them wrapped in a response DTO containing total results count.
        /// </summary>
        /// <returns>A response DTO containing a list of ProductAccountEagerDTOs, each representing a ProductAccount entity with eagerly loaded related Account and Product entities, along with the total count of results.</returns>
        public async Task<ListResponseDTO<ProductAccountEagerV2DTO>> GetAllEager()
        {
            List<ProductAccountModel> resultsQuery = await _context.ProductsAccounts
                .Include(pa => pa.Account)
                .Include(pa => pa.Product)
                .ToListAsync();

            int totalResults = resultsQuery.Count;

            List<ProductAccountEagerV2DTO> data = resultsQuery.Select(
                pa => new ProductAccountEagerV2DTO(
                        pa,
                        pa.Product != null ? new ProductSimpleDTO(pa.Product) : null,
                        pa.Account != null ? new AccountSimpleDTO(pa.Account) : null
                    )
            ).ToList();

            var response = new ListResponseDTO<ProductAccountEagerV2DTO>
            {
                TotalResults = totalResults,
                Data = data
            };

            return response;
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
            var client = await _context.ProductsAccounts
                .Include(c => c.Product)
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new ItemNotFoundException(id);

            var productDTO = (client.Product == null) ? null : new ProductSimpleDTO(client.Product);
            var accountDTO = (client.Account == null) ? null : new AccountSimpleDTO(client.Account);

            return new ProductAccountEagerDTO(client, productDTO, accountDTO);
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
            return await _context.ProductsAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
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
            var existingEntity = await _context.ProductsAccounts
                .FirstOrDefaultAsync(c => c.ProductId == postBody.ProductId && c.AccountNumber == postBody.AccountNumber);

            if (existingEntity != null)
                throw new DuplicateException(postBody.ToString()!);

            if ((postBody.ProductId != null && !await _dataAccessService.EntityExistsAsync<ProductModel>(postBody.ProductId))
                || (postBody.AccountNumber != null && !await _dataAccessService.EntityExistsAsync<AccountModel>(postBody.AccountNumber)))
            {
                throw new BadRequestException("Propiedades Invalidas, por favor revisar que el producto o la cuenta existan en la base de datos");
            }

            _context.ProductsAccounts.Add(postBody);
            await _context.SaveChangesAsync();
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
            var existingProductAccount = await GetByPkNoTracking(id)
                ?? throw new ItemNotFoundException(id);

            if ((updatedBody.ProductId != null && !await _dataAccessService.EntityExistsAsync<ProductModel>(updatedBody.ProductId))
                || (updatedBody.AccountNumber != null && !await _dataAccessService.EntityExistsAsync<AccountModel>(updatedBody.AccountNumber)))
            {
                throw new BadRequestException("Propiedades Invalidas, por favor revisar que el producto o la cuenta existan en la base de datos");
            }

            _mapper.Map(updatedBody, existingProductAccount);

            _context.Entry(existingProductAccount).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new ProductAccountSimpleDTO(existingProductAccount);
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
            var existingProductAccount = await GetByPkNoTracking(id)
                ?? throw new ItemNotFoundException(id);

            existingProductAccount.DeletedAt = DateTime.UtcNow;

            _context.ProductsAccounts.Update(existingProductAccount);

            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// The `Restore` function restores a deleted client by setting its `DeletedAt` property to
        /// null.
        /// </summary>
        /// <param name="id">The `pk` parameter in the `Restore` method is a string representing the
        /// primary key of the client that needs to be restored.</param>
        public async Task Restore(string id)
        {
            var existingProductAccount = await GetByPkNoTracking(id)
                ?? throw new ItemNotFoundException(id);

            existingProductAccount.DeletedAt = null;

            _context.ProductsAccounts.Update(existingProductAccount);

            await _context.SaveChangesAsync();
        }
    }
}
