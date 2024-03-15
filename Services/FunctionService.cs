using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class FunctionService : IBaseService<FunctionModel, FunctionEagerDTO, FunctionSimpleDTO>, ISoftDeleteService
    {
        private readonly JPMDatabaseContext _context;
        private readonly ILogger<FunctionService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IMapper _mapper;

        public FunctionService(
            JPMDatabaseContext context,
            ILogger<FunctionService> logger,
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
        /// This C# async method retrieves all FunctionModel data from the database and returns it in a
        /// ListResponseDTO along with the total number of results.
        /// </summary>
        /// <returns>
        /// This method returns a `Task` that will eventually contain a `ListResponseDTO` object
        /// containing a list of `FunctionModel` objects retrieved from the database.
        /// </returns>
        public async Task<ListResponseDTO<FunctionModel>> GetAll()
        {
            try
            {
                List<FunctionModel> data = await _context.Functions.ToListAsync();
                int totalResults = data.Count;

                var response = new ListResponseDTO<FunctionModel>
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
                    className: nameof(FunctionService), methodName: nameof(GetAll));
                throw;
            }
        }


        /// <summary>
        /// This C# function retrieves a Function entity by its primary key along with related
        /// ProfilesFunctions and UserEntitlements, and returns a FunctionEagerDTO object containing the
        /// retrieved data.
        /// </summary>
        /// <param name="id">The `pk` parameter in the `GetByPk` method is used to specify the primary
        /// key value of the Function entity that you want to retrieve from the database. This method
        /// fetches a Function entity based on the provided primary key value and includes related
        /// entities like ProfilesFunctions and UserEntitlements</param>
        /// <returns>
        /// The method `GetByPk` returns a `FunctionEagerDTO` object wrapped in a `Task` that may be
        /// null.
        /// </returns>
        public async Task<FunctionEagerDTO?> GetByPk(string id)
        {
            try
            {
                var function = await _context.Functions
                                        .Where(f => f.Id == id)
                                        .Include(f => f.ProfilesFunctions)
                                        .Include(f => f.UserEntitlements)
                                        .FirstOrDefaultAsync()
                                        ?? throw new ItemNotFoundException(id);

                var profilesFunctionsDTO = function.ProfilesFunctions.Select(pf => new ProfilesFunctionSimpleDTO(pf)).ToList();
                var userEntitlementDTOs = function.UserEntitlements.Select(ue => new UserEntitlementSimpleDTO(ue)).ToList();

                return new FunctionEagerDTO(function, profilesFunctionsDTO, userEntitlementDTOs);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionService), methodName: nameof(GetByPk));
                throw;
            }
        }


        /// <summary>
        /// This C# async function retrieves a FunctionModel by primary key without tracking changes in
        /// the database.
        /// </summary>
        /// <param name="id">The `pk` parameter in the `GetByPkNoTracking` method is used to specify the
        /// primary key value of the `FunctionModel` that you want to retrieve from the
        /// database.</param>
        /// <returns>
        /// The method `GetByPkNoTracking` returns a `Task` that will eventually yield a `FunctionModel`
        /// object or `null` (nullable `FunctionModel`).
        /// </returns>
        public async Task<FunctionModel?> GetByPkNoTracking(string id)
        {
            try
            {
                return await _context.Functions
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionService), methodName: nameof(GetByPkNoTracking));
                throw;
            }
        }


        /// <summary>
        /// The Post method in C# asynchronously adds a FunctionModel to the database while handling
        /// potential exceptions.
        /// </summary>
        /// <param name="postBody">FunctionModel is a class representing the data model for a
        /// function. It likely contains properties that define the attributes of a function, such as
        /// Id, Name, Description, etc. In the provided code snippet, an instance of FunctionModel is
        /// passed as the postBody parameter to the Post method for creating a</param>
        public async Task Post(FunctionModel postBody)
        {
            try
            {
                if (await GetByPkNoTracking(postBody.Id) != null) throw new DuplicateException(postBody.Id);

                _context.Functions.Add(postBody);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionService), methodName: nameof(Post));
                throw;
            }
        }


        /// <summary>
        /// This C# function updates a record in a database using the primary key and returns the
        /// updated record.
        /// </summary>
        /// <param name="id">The `pk` parameter in the `UpdateByPK` method is typically used to specify
        /// the primary key value of the entity that needs to be updated. It is a unique identifier for
        /// the entity in the database table.</param>
        /// <param name="updatedBody">FunctionSimpleDTO is a data transfer object (DTO) that
        /// represents a simple version of a function entity. It is used for transferring data related
        /// to a function between different layers of the application, such as between the service layer
        /// and the controller layer. In this context, it is used to update an existing</param>
        /// <returns>
        /// The method `UpdateByPK` returns a `Task` that will eventually contain a `FunctionSimpleDTO`
        /// object representing the updated function after the update operation is completed.
        /// </returns>
        public async Task<FunctionSimpleDTO> UpdateByPK(string id, FunctionSimpleDTO updatedBody)
        {
            try
            {
                var existingFunction = await _context.Functions
                                                        .FirstOrDefaultAsync(f => f.Id == id)
                                                        ?? throw new ItemNotFoundException(id); ;

                _mapper.Map(updatedBody, existingFunction);

                _context.Entry(existingFunction).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new FunctionSimpleDTO(existingFunction);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionService), methodName: nameof(UpdateByPK));
                throw;
            }
        }


        /// <summary>
        /// The Delete method asynchronously deletes a record by setting the DeletedAt property and
        /// updating the database context in a C# application.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `Delete` method is a string representing the
        /// primary key of the item that needs to be deleted from the database.</param>
        public async Task SoftDelete(string pk)
        {
            try
            {
                var existingFunction = await GetByPkNoTracking(pk) ?? throw new ItemNotFoundException(pk);

                existingFunction.DeletedAt = DateTime.UtcNow;

                _context.Functions.Update(existingFunction);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionService), methodName: nameof(SoftDelete));
                throw;
            }
        }


        /// <summary>
        /// This C# async method restores a function by setting its DeletedAt property to null in the
        /// database.
        /// </summary>
        /// <param name="pk">The `pk` parameter in the `Restore` method is a string representing the
        /// primary key of the item that needs to be restored.</param>
        public async Task Restore(string pk)
        {
            try
            {
                var existingFunction = await GetByPkNoTracking(pk) ?? throw new ItemNotFoundException(pk);

                existingFunction.DeletedAt = null;

                _context.Functions.Update(existingFunction);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionService), methodName: nameof(Restore));
                throw;
            }
        }
    }
}