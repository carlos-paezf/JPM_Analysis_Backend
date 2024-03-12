using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class ClientService : IBaseService<ClientModel, ClientEagerDTO, ClientSimpleDTO>
    {
        private readonly JPMDatabaseContext _context;
        private readonly ILogger<ClientService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IMapper _mapper;

        public ClientService(
            JPMDatabaseContext context,
            ILogger<ClientService> logger,
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
        /// `ListResponse` of `Client`. The `ListResponse` of `Client` object contains the total number of
        /// results (`TotalResults`) and a list of `Client` objects (`Data`).
        /// </returns>
        public async Task<ListResponseDTO<ClientModel>> GetAll()
        {
            try
            {
                List<ClientModel> data = await _context.Clients.ToListAsync();
                int totalResults = data.Count;

                var response = new ListResponseDTO<ClientModel>
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
                    className: nameof(ClientService), methodName: nameof(GetAll));
                throw;
            }
        }


        /// <summary>
        /// The function retrieves client details along with associated clients and user entitlements
        /// based on the provided client number.
        /// </summary>
        /// <param name="pk">
        /// The Client Number is the unique identifier for each client
        /// </param>
        /// <returns>
        /// The `GetByPk` method is returning an `ClientDetailsDTO` object or `null` if no matching
        /// client is found in the database. The `ClientDetailsDTO` object contains details about the
        /// client, including client number, client name, client type, a list of clients associated
        /// with the client (represented as `ClientDTO` objects), and a list of user entitlements
        /// associated with the client (represented as `UserEntitlementsDTO` objects).
        /// </returns>
        public async Task<ClientEagerDTO?> GetByPk(string pk)
        {
            try
            {
                var client = await _context.Clients
                                        .Include(c => c.Product)
                                        .Include(c => c.Account)
                                        .FirstOrDefaultAsync(c => c.Id == long.Parse(pk))
                                        ?? throw new ItemNotFoundException(pk);

                _logger.LogInformation($"{client}");

                var productDTO = (client.Product == null) ? null : new ProductSimpleDTO(client.Product);
                var accountDTO = (client.Account == null) ? null : new AccountSimpleDTO(client.Account);

                return new ClientEagerDTO(client, productDTO, accountDTO);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ClientService), methodName: nameof(GetByPk));
                throw;
            }
        }


        /// <summary>
        /// This C# async method retrieves an Client entity by its primary key without tracking changes
        /// in the context.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `GetByPkNoTracking` method is a string
        /// representing the primary key value used to retrieve an `Client` entity from the
        /// database.</param>
        /// <returns>
        /// The method `GetByPkNoTracking` is returning a `Task` that may contain an `Client` object or
        /// `null`.
        /// </returns>
        public async Task<ClientModel?> GetByPkNoTracking(string pk)
        {
            try
            {
                return await _context.Clients
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == long.Parse(pk));
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ClientService), methodName: nameof(GetByPk));
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
        public async Task Post(ClientModel postBody)
        {
            try
            {
                var existingEntity = await _context.Clients
                                    .FirstOrDefaultAsync(c => c.ProductId == postBody.ProductId && c.AccountNumber == postBody.AccountNumber);

                if (existingEntity != null) throw new DuplicateException($"{postBody.ProductId}-{postBody.AccountNumber}");

                if ((postBody.ProductId != null && !await ProductExistsAsync(postBody.ProductId))
                    || (postBody.AccountNumber != null && !await AccountExistsAsync(postBody.AccountNumber)))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el producto o la cuenta existan en la base de datos");
                }

                _context.Clients.Add(postBody);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ClientService), methodName: nameof(Post));
                throw;
            }
        }


        /// <summary>
        /// This C# function updates an client entity by its primary key using asynchronous operations
        /// and error handling.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `UpdateByPK` method likely stands for the primary
        /// key of the client entity that you want to update. It is used to identify the specific
        /// client record that needs to be updated in the database.</param>
        /// <param name="updatedBody">ClientSimpleDTO is a data transfer object (DTO) that
        /// represents a simplified version of an client entity. It is used for transferring
        /// client-related data between different parts of the application, such as between the client
        /// and the server or between different layers of the application. In the provided code snippet,
        /// the Update</param>
        /// <returns>
        /// The method `UpdateByPK` returns an `ClientSimpleDTO` object after updating an existing
        /// client entity in the database.
        /// </returns>
        public async Task<ClientSimpleDTO> UpdateByPK(string pk, ClientSimpleDTO updatedBody)
        {
            try
            {
                var existingClient = await GetByPkNoTracking(pk) ?? throw new ItemNotFoundException(pk);

                if ((updatedBody.ProductId != null && !await ProductExistsAsync(updatedBody.ProductId))
                    || (updatedBody.AccountNumber != null && !await AccountExistsAsync(updatedBody.AccountNumber)))
                {
                    throw new BadRequestException("Propiedades Invalidas, por favor revisar que el producto o la cuenta existan en la base de datos");
                }

                _mapper.Map(updatedBody, existingClient);

                _context.Entry(existingClient).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new ClientSimpleDTO(existingClient);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ClientService), methodName: nameof(UpdateByPK));
                throw;
            }
        }


        /// <summary>
        /// The Delete method asynchronously deletes an client by setting the DeletedAt property to the
        /// current time and saving changes to the database, handling exceptions and logging errors if
        /// they occur.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `Delete` method is a string representing the
        /// primary key of the client that needs to be deleted.</param>
        public async Task Delete(string pk)
        {
            try
            {
                var existingClient = await GetByPkNoTracking(pk) ?? throw new ItemNotFoundException(pk);

                existingClient.DeletedAt = DateTime.UtcNow;

                _context.Clients.Update(existingClient);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ClientService), methodName: nameof(Delete));
                throw;
            }
        }


        /// <summary>
        /// The `Restore` function restores a deleted client by setting its `DeletedAt` property to
        /// null.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `Restore` method is a string representing the
        /// primary key of the client that needs to be restored.</param>
        public async Task Restore(string pk)
        {
            try
            {
                var existingClient = await GetByPkNoTracking(pk) ?? throw new ItemNotFoundException(pk);

                existingClient.DeletedAt = null;

                _context.Clients.Update(existingClient);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ClientService), methodName: nameof(Restore));
                throw;
            }
        }
    }
}
