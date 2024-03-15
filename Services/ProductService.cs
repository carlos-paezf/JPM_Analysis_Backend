using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class ProductService : IBaseService<ProductModel, ProductEagerDTO, ProductSimpleDTO>, ISoftDeleteService
    {
        private readonly JPMDatabaseContext _context;
        private readonly ILogger<ProductService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IMapper _mapper;

        public ProductService(
            JPMDatabaseContext context,
            ILogger<ProductService> logger,
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
        /// This C# async method retrieves all products from a database and returns them in a
        /// ListResponseDTO along with the total count.
        /// </summary>
        /// <returns>
        /// This method returns a `Task` that will eventually contain a `ListResponseDTO` of `ProductModel`.
        /// The `ListResponseDTO` contains the total number of results and a list of `ProductModel` data
        /// retrieved from the database.
        /// </returns>
        public async Task<ListResponseDTO<ProductModel>> GetAll()
        {
            try
            {
                List<ProductModel> data = await _context.Products.ToListAsync();
                int totalResults = data.Count;

                var response = new ListResponseDTO<ProductModel>
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
                    className: nameof(ProductService), methodName: nameof(GetAll));
                throw;
            }
        }


        /// <summary>
        /// This C# function retrieves a product by its primary key along with related clients and user
        /// entitlements, handling exceptions and logging errors.
        /// </summary>
        /// <param name="id">The `id` parameter is the unique identifier used to retrieve a specific
        /// product from the database.</param>
        /// <returns>
        /// The method `GetByPk` is returning a `ProductEagerDTO` object asynchronously. This object is
        /// created using data retrieved from the database for a specific product identified by the
        /// provided `id`. The `ProductEagerDTO` object includes information about the product, its
        /// clients, and user entitlements.
        /// </returns>
        public async Task<ProductEagerDTO?> GetByPk(string id)
        {
            try
            {
                var product = await _context.Products
                                        .Where(p => p.Id == id)
                                        .Include(p => p.ProductsAccounts)
                                        .Include(p => p.UserEntitlements)
                                        .FirstOrDefaultAsync()
                                        ?? throw new ItemNotFoundException(id);

                var productsAccountsDTO = product.ProductsAccounts.Select(pf => new ProductAccountSimpleDTO(pf)).ToList();
                var userEntitlementDTOs = product.UserEntitlements.Select(ue => new UserEntitlementSimpleDTO(ue)).ToList();

                return new ProductEagerDTO(product, productsAccountsDTO, userEntitlementDTOs);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductService), methodName: nameof(GetByPk));
                throw;
            }
        }


        /// <summary>
        /// This C# async method retrieves a product by its primary key without tracking changes in the
        /// context.
        /// </summary>
        /// <param name="id">The `id` parameter in the `GetByPkNoTracking` method is used to specify the
        /// primary key value of the product you want to retrieve from the database.</param>
        /// <returns>
        /// The method `GetByPkNoTracking` returns a `Task` that will eventually yield a `ProductModel`
        /// object or `null` if no matching product is found in the database.
        /// </returns>
        public async Task<ProductModel?> GetByPkNoTracking(string id)
        {
            try
            {
                return await _context.Products
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductService), methodName: nameof(GetByPkNoTracking));
                throw;
            }
        }


        /// <summary>
        /// This C# async function posts a product model to a database while handling duplicate
        /// exceptions and logging any errors.
        /// </summary>
        /// <param name="postBody">ProductModel is a class representing a product entity with
        /// properties like Id, Name, Price, Description, etc. It is used as the data model for creating
        /// a new product in the system.</param>
        public async Task Post(ProductModel postBody)
        {
            try
            {
                if (await GetByPkNoTracking(postBody.Id) != null) throw new DuplicateException(postBody.Id);

                _context.Products.Add(postBody);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductService), methodName: nameof(Post));
                throw;
            }
        }


        /// <summary>
        /// This C# function updates a product entity by its primary key with error handling and
        /// logging.
        /// </summary>
        /// <param name="id">The `id` parameter in the `UpdateByPK` method is the unique identifier of
        /// the product that you want to update. It is used to find the existing product in the database
        /// that matches this identifier.</param>
        /// <param name="updatedBody">ProductSimpleDTO is a data transfer object (DTO) that
        /// represents a simplified version of a product entity. It is used for transferring product
        /// data between different layers of the application, such as between the service layer and the
        /// controller layer. In this context, it is used to update an existing product entity
        /// in</param>
        /// <returns>
        /// The method `UpdateByPK` returns a `Task` of `ProductSimpleDTO`, which is a simple data transfer
        /// object representing a product.
        /// </returns>
        public async Task<ProductSimpleDTO> UpdateByPK(string id, ProductSimpleDTO updatedBody)
        {
            try
            {
                var existingProduct = await _context.Products
                                                        .FirstOrDefaultAsync(f => f.Id == id)
                                                        ?? throw new ItemNotFoundException(id); ;

                _mapper.Map(updatedBody, existingProduct);

                _context.Entry(existingProduct).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new ProductSimpleDTO(existingProduct);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductService), methodName: nameof(UpdateByPK));
                throw;
            }
        }


        /// <summary>
        /// The Delete method asynchronously deletes a product by setting its DeletedAt property and
        /// updating the database.
        /// </summary>
        /// <param name="id">The `id` parameter in the `Delete` method is the identifier of the product
        /// that needs to be deleted from the database.</param>
        public async Task SoftDelete(string id)
        {
            try
            {
                var existingProduct = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id);

                existingProduct.DeletedAt = DateTime.UtcNow;

                _context.Products.Update(existingProduct);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductService), methodName: nameof(SoftDelete));
                throw;
            }
        }


        /// <summary>
        /// The `Restore` method restores a product by setting its `DeletedAt` property to null in a C#
        /// application.
        /// </summary>
        /// <param name="id">The `id` parameter in the `Restore` method is a string that represents the
        /// unique identifier of the product that needs to be restored.</param>
        public async Task Restore(string id)
        {
            try
            {
                var existingProduct = await GetByPkNoTracking(id) ?? throw new ItemNotFoundException(id);

                existingProduct.DeletedAt = null;

                _context.Products.Update(existingProduct);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProductService), methodName: nameof(Restore));
                throw;
            }
        }
    }
}