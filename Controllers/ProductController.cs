using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("products", Name = "Products")]
    [Produces("application/json")]
    public class ProductController : ControllerBase, IBaseApiController<ProductModel, ProductSimpleDTO>
    {
        private readonly JPMDatabaseContext _context;

        private readonly ILogger<ProductController> _logger;

        private readonly ProductService _service;

        private readonly IErrorHandlingService _errorHandlingService;

        public ProductController(
            JPMDatabaseContext context,
            ProductService service,
            ILogger<ProductController> logger,
            ErrorHandlingService errorHandlingService
        )
        {
            _context = context;
            _service = service;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
        }


        /// <summary>
        /// This endpoint retrieves a list of products with the total number of results
        /// </summary>
        /// <returns>
        /// A paginated response containing a list of products is being returned. If there are no
        /// products found in the database, a "Not Found" response with a message indicating that no
        /// records were found is returned.
        /// </returns>
        /// <response code="200">Returns the products list. If there are no records in the DB, it returns an empty array/// </response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpGet(Name = "GetProducts")]
        public async Task<ActionResult<ListResponseDTO<ProductModel>>> GetAll()
        {
            try
            {
                var response = await _service.GetAll();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionController), methodName: nameof(GetAll));
            }
        }


        /// <summary>
        /// This endpoint retrieves an product entity by its id along with related clients
        /// and userEntitlements details, or a 404 code status if not found.
        /// </summary>
        /// <param name="id">
        /// The id is the unique identifier for each product entry.
        /// </param>
        /// <returns>
        /// The GetById method is returning an ActionResult of type product. If the entity is found in
        /// the database based on the provided id, it returns the entity. If the entity is
        /// not found (entity is null), it returns a NotFound result with a message indicating that the
        /// function was not found for the specified id.
        /// </returns>
        /// <response code="200">Returns an function by its id</response>
        /// <response code="404">Returns an error message by not found item</response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<ProductModel>> GetByPk([FromRoute] string id)
        {
            try
            {
                var response = await _service.GetByPk(id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionController), methodName: nameof(GetByPk));
            }
        }


        /// <summary>
        /// This endpoint handles a POST request to create a new product, checking for duplicate
        /// ids and returning appropriate responses.
        /// </summary>
        /// <param name="body">
        /// The code you provided is a POST method in a controller that receives
        /// an Product object in the request body. The method calls a service to save the product
        /// information and returns an appropriate response based on the outcome.
        /// </param>
        /// <returns>
        /// The Post method in the FunctionController is returning different types of ActionResult based
        /// on the outcome of the operation:
        /// </returns>
        /// <response code="201">Returns the new product</response>
        /// <response code="409">Returns an error message by duplicate id</response>
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPost(Name = "PostProduct")]
        public async Task<ActionResult> Post([FromBody] ProductModel body)
        {
            try
            {
                await _service.Post(body);

                return CreatedAtAction(
                        nameof(GetByPk),
                        new { id = body.Id },
                        body
                    );
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionController), methodName: nameof(Post));
            }
            throw new NotImplementedException();
        }


        /// <summary>
        /// This endpoint updates an product by its id, handling error cases and
        /// returning appropriate responses.
        /// </summary>
        /// <param name="id">The identifier for this item.</param>
        /// <param name="body">The data to update the entity</param>
        /// <returns>
        /// The code is returning an `ActionResult` asynchronously. If the update operation is
        /// successful, it returns an `Ok` response with the updated `body` object. If the
        /// `body.Id` does not match the `id` in the route, it returns a
        /// `BadRequest` response. If the existing product is not found, it returns a `NotFound`
        /// response. If an exception occurs during the execution throw an internal server error.
        /// </returns>
        /// <response code="200">Returns the updated product information when successful</response>
        /// <response code="400">Alerts the user about incorrect input data</response>
        /// <response code="404">The product was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPut("update/{id}", Name = "UpdateProduct")]
        public async Task<ActionResult> UpdateByPK([FromRoute] string id, [FromBody] ProductSimpleDTO body)
        {
            try
            {
                if (id != body.Id) return BadRequest();

                var response = await _service.UpdateByPK(id, body);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(FunctionController), methodName: nameof(UpdateByPK));
            }
        }


        /// <summary>
        /// This endpoint handles a DELETE request to delete an product by id, with error
        /// handling included.
        /// </summary>
        /// <param name="id">The `id` parameter is a string that represents the
        /// unique identifier of the product that needs to be deleted.</param>
        /// <returns>
        /// The Delete method is returning an ActionResult. If the deletion is successful, it returns a
        /// NoContent result (HTTP status code 204). If an exception occurs during the deletion process,
        /// it returns the result of the error handling service's HandleExceptionAsync method.
        /// </returns>
        /// <response code="204">If the delete process was success, returns an status code 204 without content</response>
        /// <response code="404">The product was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpDelete("delete/{id}", Name = "DeleteProduct")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            try
            {
                await _service.Delete(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(FunctionController), methodName: nameof(Delete));
            }
        }


        /// <summary>
        /// This endpoint handles PATCH requests to restore a specific product
        /// by its id.
        /// </summary>
        /// <param name="id">The `id` parameter is a string that represents the
        /// unique identifier of the product that needs to be deleted.</param>
        /// <returns>
        /// The `Restore` method is returning an `ActionResult`. If the restoration process is
        /// successful, it returns a `NoContent` result. If an exception occurs during the restoration
        /// process, it returns the result of the `_errorHandlingService.HandleExceptionAsync` method,
        /// which handles the exception and logs the error.
        /// </returns>
        /// <response code="204">If the restore process was success, returns an status code 204 without content</response>
        /// <response code="404">The product was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPatch("restore/{id}", Name = "RestoreProduct")]
        public async Task<ActionResult> Restore([FromRoute] string id)
        {
            try
            {
                await _service.Restore(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(FunctionController), methodName: nameof(Restore));
            }
        }
    }
}