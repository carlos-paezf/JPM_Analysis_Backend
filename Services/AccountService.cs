using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class AccountService : IBaseService<AccountModel, AccountEagerDTO, AccountSimpleDTO>
    {
        private readonly JPMDatabaseContext _context;
        private readonly ILogger<AccountService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IMapper _mapper;

        public AccountService(
            JPMDatabaseContext context,
            ILogger<AccountService> logger,
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
        /// The function GetAll retrieves all accounts from the database and returns them in a
        /// ListResponse object.
        /// </summary>
        /// <returns>
        /// The method `GetAll` is returning a `Task` that will eventually resolve to a
        /// `ListResponse` of `Account`. The `ListResponse` of `Account` object contains the total number of
        /// results (`TotalResults`) and a list of `Account` objects (`Data`).
        /// </returns>
        public async Task<ListResponseDTO<AccountModel>> GetAll()
        {
            try
            {
                List<AccountModel> data = await _context.Accounts.ToListAsync();
                int totalResults = data.Count;

                var response = new ListResponseDTO<AccountModel>
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
                    className: nameof(AccountService), methodName: nameof(GetAll));
                throw;
            }
        }


        /// <summary>
        /// The function retrieves account details along with associated clients and user entitlements
        /// based on the provided account number.
        /// </summary>
        /// <param name="accountNumber">
        /// The Account Number is the unique identifier for each account
        /// </param>
        /// <returns>
        /// The `GetByPk` method is returning an `AccountDetailsDTO` object or `null` if no matching
        /// account is found in the database. The `AccountDetailsDTO` object contains details about the
        /// account, including account number, account name, account type, a list of clients associated
        /// with the account (represented as `ClientDTO` objects), and a list of user entitlements
        /// associated with the account (represented as `UserEntitlementsDTO` objects).
        /// </returns>
        public async Task<AccountEagerDTO?> GetByPk(string accountNumber)
        {
            try
            {
                var account = await _context.Accounts
                                        .Where(a => a.AccountNumber == accountNumber)
                                        .Include(a => a.Clients)
                                        .Include(a => a.UserEntitlements)
                                        .FirstOrDefaultAsync()
                                        ?? throw new ItemNotFoundException(accountNumber);

                var clientDTOs = account.Clients.Select(c => new ClientSimpleDTO(c)).ToList();
                var userEntitlementDTOs = account.UserEntitlements.Select(ue => new UserEntitlementSimpleDTO(ue)).ToList();

                return new AccountEagerDTO(account, clientDTOs, userEntitlementDTOs);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountService), methodName: nameof(GetByPk));
                throw;
            }
        }


        /// <summary>
        /// This C# async method retrieves an Account entity by its primary key without tracking changes
        /// in the context.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `GetByPkNoTracking` method is a string
        /// representing the primary key value used to retrieve an `Account` entity from the
        /// database.</param>
        /// <returns>
        /// The method `GetByPkNoTracking` is returning a `Task` that may contain an `Account` object or
        /// `null`.
        /// </returns>
        public async Task<AccountModel?> GetByPkNoTracking(string pk)
        {
            try
            {
                return await _context.Accounts
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(a => a.AccountNumber == pk);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountService), methodName: nameof(GetByPk));
                throw;
            }
        }


        /// <summary>
        /// The function `Post` asynchronously adds a new account to the database if it doesn't already
        /// exist and logs any errors that occur.
        /// </summary>
        /// <param name="postBody">
        /// The `Post` method you provided is an asynchronous method that attempts
        /// to create a new account in a database. Here's a breakdown of the method:</param>
        /// <returns>
        /// The `Post` method returns a `Task` of `bool`. If a new account is successfully added to the
        /// database, it returns `true`. If an account with the same account number already exists, it
        /// returns `false`. If an exception occurs during the process, it will log the error and
        /// rethrow the exception.
        /// </returns>
        public async Task Post(AccountModel postBody)
        {
            try
            {
                if (await GetByPkNoTracking(postBody.AccountNumber) != null) throw new DuplicateException(postBody.AccountNumber);

                _context.Accounts.Add(postBody);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountService), methodName: nameof(Post));
                throw;
            }
        }


        /// <summary>
        /// This C# function updates an account entity by its primary key using asynchronous operations
        /// and error handling.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `UpdateByPK` method likely stands for the primary
        /// key of the account entity that you want to update. It is used to identify the specific
        /// account record that needs to be updated in the database.</param>
        /// <param name="updatedBody">AccountSimpleDTO is a data transfer object (DTO) that
        /// represents a simplified version of an account entity. It is used for transferring
        /// account-related data between different parts of the application, such as between the client
        /// and the server or between different layers of the application. In the provided code snippet,
        /// the Update</param>
        /// <returns>
        /// The method `UpdateByPK` returns an `AccountSimpleDTO` object after updating an existing
        /// account entity in the database.
        /// </returns>
        public async Task<AccountSimpleDTO> UpdateByPK(string pk, AccountSimpleDTO updatedBody)
        {
            try
            {
                var existingAccount = await GetByPkNoTracking(pk) ?? throw new ItemNotFoundException(pk);

                _mapper.Map(updatedBody, existingAccount);

                _context.Entry(existingAccount).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new AccountSimpleDTO(existingAccount);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountService), methodName: nameof(UpdateByPK));
                throw;
            }
        }


        /// <summary>
        /// The Delete method asynchronously deletes an account by setting the DeletedAt property to the
        /// current time and saving changes to the database, handling exceptions and logging errors if
        /// they occur.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `Delete` method is a string representing the
        /// primary key of the account that needs to be deleted.</param>
        public async Task Delete(string pk)
        {
            try
            {
                var existingAccount = await GetByPkNoTracking(pk) ?? throw new ItemNotFoundException(pk);

                existingAccount.DeletedAt = DateTime.UtcNow;

                _context.Accounts.Update(existingAccount);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountService), methodName: nameof(Delete));
                throw;
            }
        }


        /// <summary>
        /// The `Restore` function restores a deleted account by setting its `DeletedAt` property to
        /// null.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `Restore` method is a string representing the
        /// primary key of the account that needs to be restored.</param>
        public async Task Restore(string pk)
        {
            try
            {
                var existingAccount = await GetByPkNoTracking(pk) ?? throw new ItemNotFoundException(pk);

                existingAccount.DeletedAt = null;

                _context.Accounts.Update(existingAccount);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(AccountService), methodName: nameof(Restore));
                throw;
            }
        }
    }
}