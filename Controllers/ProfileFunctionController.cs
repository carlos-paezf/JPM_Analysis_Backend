using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("profilesFunctions", Name = "ProfileFunction")]
    [Produces("application/json")]
    public class ProfileFunctionController
        : ControllerBase
            , IBaseApiController<ProfileFunctionModel, ProfileFunctionSimpleDTO>
            , IHardDeleteController
            , IBulkPostController<ProfileFunctionModel>
            , IBulkHardDeleteController
    {
        private readonly JPMDatabaseContext _context;

        private readonly ILogger<ProfileFunctionController> _logger;

        private readonly ProfileFunctionService _service;

        private readonly IErrorHandlingService _errorHandlingService;

        public ProfileFunctionController(
            JPMDatabaseContext context,
            ProfileFunctionService service,
            ILogger<ProfileFunctionController> logger,
            ErrorHandlingService errorHandlingService
        )
        {
            _context = context;
            _service = service;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
        }


        /// <summary>
        /// Retrieves a list of profile functions using an HTTP GET request and handles exceptions with error logging.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing a <c>ListResponseDTO</c> of <c>ProfileFunctionModel</c> objects.
        /// If there are records in the database, it returns a list of profile functions. If there are no records
        /// in the database, it returns an empty array. If an exception occurs during the process, it returns
        /// an Internal Server Error alert.
        /// </returns>
        /// <response code="200">Returns the list of profile functions.</response>
        /// <response code="500">Returns an Internal Server Error alert if an exception occurs during the process.</response>
        [HttpGet(Name = "GetProfileFunctions")]
        public async Task<ActionResult<ListResponseDTO<ProfileFunctionModel>>> GetAll()
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
                    className: nameof(ProfileFunctionController), methodName: nameof(GetAll));
            }
        }


        /// <summary>
        /// Retrieves a profile function by its primary key and handles exceptions if any occur.
        /// </summary>
        /// <param name="id">The ID of the profile function to retrieve. It is passed as a string parameter
        /// from the route and is used to uniquely identify the profile function.</param>
        /// <returns>
        /// An <c>ActionResult</c> of type <c>ProfileFunctionModel</c>. If the operation is successful,
        /// it returns an Ok response with the retrieved data. If the profile function is not found, it
        /// returns a NotFound response with an appropriate error message. If an exception occurs during
        /// the operation, it returns the result of handling the exception using the
        /// <c>_errorHandlingService</c>.
        /// </returns>
        /// <response code="200">Returns the profile function with the specified ID.</response>
        /// <response code="404">Returns an error message if the profile function is not found.</response>
        /// <response code="500">Returns an Internal Server Error alert if an exception occurs during the process.</response>
        [HttpGet("{id}", Name = "GetProfileFunctionById")]
        public async Task<ActionResult<ProfileFunctionModel>> GetByPk([FromRoute] string id)
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
                    className: nameof(ProfileFunctionController), methodName: nameof(GetByPk));
            }
        }


        /// <summary>
        /// Retrieves the functions associated with a profile based on the provided profile ID.
        /// </summary>
        /// <param name="id">The ID of the profile.</param>
        /// <returns>
        /// ActionResult containing a list response DTO with function data associated with the specified profile ID,
        /// or an error response if the operation fails.
        /// </returns>
        [HttpGet("functions-by-profile-id/{id}", Name = "GetFunctionsByProfileId")]
        public async Task<ActionResult<ListResponseDTO<FunctionSimpleDTO>>> GetFunctionsByProfileId([FromRoute] string id)
        {
            try
            {
                var response = await _service.GetFunctionsByProfileId(id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionController), methodName: nameof(GetFunctionsByProfileId));
            }
        }


        /// <summary>
        /// Retrieves the functions not associated with a profile based on the provided profile ID.
        /// </summary>
        /// <param name="id">The ID of the profile.</param>
        /// <returns>
        /// ActionResult containing a list response DTO with function data not associated with the specified profile ID,
        /// or an error response if the operation fails.
        /// </returns>
        [HttpGet("functions-no-associated-by-profile-id/{id}", Name = "GetFunctionsNoAssociatedByProfileId")]
        public async Task<ActionResult<ListResponseDTO<FunctionSimpleDTO>>> GetFunctionsNoAssociatedByProfileId([FromRoute] string id)
        {
            try
            {
                var response = await _service.GetFunctionsNoAssociatedByProfileId(id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionController), methodName: nameof(GetFunctionsNoAssociatedByProfileId));
            }
        }



        /// <summary>
        /// This endpoint handles a POST request to create a new profile function, returning the
        /// created resource with appropriate error handling.
        /// </summary>
        /// <param name="body">A <c>ProfileFunctionModel</c> object representing the data structure for
        /// a profile function. This model class likely contains properties such as <c>Id</c>, <c>Name</c>,
        /// <c>Description</c>, etc., depending on the specific requirements of your application. In the
        /// context of the provided code snippet, it is used as the data passed in the request body.</param>
        /// <returns>
        /// An <c>ActionResult</c> representing the result of the POST operation. If the operation is
        /// successful, it returns a <c>CreatedAtAction</c> result with the newly created profile function
        /// model. If an exception occurs during the operation, it returns the result of the error handling
        /// service's <c>HandleExceptionAsync</c> method.</returns>
        /// <response code="201">Returns the newly created profile function.</response>
        /// <response code="409">Returns an error message if a duplicate ID is detected.</response>
        /// <response code="500">Returns an Internal Server Error alert if an exception occurs during the process.</response>
        [HttpPost(Name = "PostProfileFunction")]
        public async Task<ActionResult> Post([FromBody] ProfileFunctionModel body)
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
                    className: nameof(ProfileFunctionController), methodName: nameof(Post));
            }
        }


        /// <summary>
        /// This endpoint handles the update of a profile function identified by its primary key,
        /// with error handling to manage exceptions.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> parameter represents the unique identifier of the
        /// profile function to be updated. It is retrieved from the route and used to identify the specific
        /// profile function.</param>
        /// <param name="body">A <c>ProfileFunctionSimpleDTO</c> object representing a simplified version
        /// of the profile function entity. This data transfer object typically contains properties that
        /// can be modified during the update process, such as name, description, or any other relevant fields.</param>
        /// <returns>
        /// An <c>ActionResult</c> representing the result of the update operation. If the provided
        /// <paramref name="id"/> does not match the <c>Id</c> property in the <paramref name="body"/>,
        /// a <c>BadRequest</c> response is returned. Otherwise, the method calls the service to update
        /// the profile function and returns the response from the service as an <c>Ok</c> response. If
        /// an exception occurs during the process, the method handles the exception and returns the
        /// appropriate <c>ActionResult</c> using the <c>_errorHandlingService.HandleExceptionAsync</c> method.
        /// </returns>
        /// <response code="200">The operation was successful. Returns an object containing the number of affected records.</response>
        /// <response code="400">If the provided ID does not match the ID in the body.</response>
        /// <response code="500">Returns an Internal Server Error alert if an exception occurs during the process.</response>
        [HttpPut("update/{id}", Name = "UpdateProfileFunction")]
        public async Task<ActionResult> UpdateByPK([FromRoute] string id, [FromBody] ProfileFunctionSimpleDTO body)
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
                    className: nameof(ProfileFunctionController), methodName: nameof(UpdateByPK));
            }
        }


        /// <summary>
        /// This endpoint uses an HTTP DELETE request to hard delete a profile based on the provided ID.
        /// It handles exceptions and logs errors as necessary.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> parameter represents the unique identifier of the
        /// profile to be hard deleted. This parameter is passed from the route and is used to identify
        /// the specific profile or resource to be deleted.</param>
        /// <returns>
        /// An <c>ActionResult</c> representing the result of the hard delete operation. If the deletion
        /// operation is successful, it returns a <c>NoContent</c> result indicating that no content is
        /// returned. If an exception occurs during the deletion process, the method catches the exception
        /// and returns the result of the <c>_errorHandlingService.HandleExceptionAsync</c> method. This
        /// method handles the exception by logging errors and returning an appropriate <c>ActionResult</c>.
        /// </returns>
        /// <response code="204">If the delete process was success, returns an status code 204 without content</response>
        /// <response code="404">The profile_function was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpDelete("delete/{id}", Name = "DeleteProfileFunction")]
        public async Task<ActionResult> HardDelete([FromRoute] string id)
        {
            try
            {
                await _service.HardDelete(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                   ex: ex, logger: _logger,
                   className: nameof(ProfileFunctionController), methodName: nameof(HardDelete));
            }
        }


        /// <summary>
        /// The <c>BulkPost</c> method in a C# controller processes a collection of <c>ProfileFunctionModel</c>
        /// objects by removing duplicate entries based on their <c>Id</c> property. After deduplication,
        /// the method invokes a service method to perform a bulk post operation with the unique entries.
        /// </summary>
        /// <param name="collectionBody">The <paramref name="collectionBody"/> parameter represents the
        /// collection of <c>ProfileFunctionModel</c> objects received in the request body. The method
        /// first processes this collection to retain only unique entries based on the <c>Id</c> property
        /// of each <c>ProfileFunctionModel</c>.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing a <c>ListResponseDTO</c> of <c>ProfileFunctionModel</c> objects,
        /// representing the result of the bulk post operation. The response includes details of the successfully
        /// posted entities along with any error messages, if applicable.
        /// </returns>
        /// <response code="200">Returns the profiles_functions created list</response>
        /// <response code="400">If an error occurs during the deletion process</response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpPost("bulk-post", Name = "BulkPostProfileFunction")]
        public async Task<ActionResult<ListResponseDTO<ProfileFunctionModel>>> BulkPost([FromBody] ICollection<ProfileFunctionModel> collectionBody)
        {
            try
            {
                var uniqueEntries = collectionBody.GroupBy(pf => pf.Id)
                                                    .Select(group => group.First())
                                                    .ToList();

                var response = await _service.BulkPost(uniqueEntries);

                return response;
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionController), methodName: nameof(BulkPost));
            }
        }


        /// <summary>
        /// This endpoint performs a bulk hard delete operation on a collection of records and
        /// returns the count of affected records.
        /// </summary>
        /// <param name="pks">An array containing the primary keys (IDs) of the profile function records to delete.</param>
        /// <returns>
        /// The BulkHardDelete method in the controller is returning an ActionResult. If the deletion
        /// operation is successful, it returns an Ok result with the count of affected records in the
        /// response body. If an exception occurs during the deletion process, the controller calls the
        /// error handling service to handle the exception and return the appropriate response.
        /// </returns>
        /// <response code="200">The operation was successful. Returns an object containing the number of affected records.</response>
        /// <response code="400">If an error occurs during the deletion process</response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpDelete("bulk-delete", Name = "BulkDeleteProfileFunction")]
        public async Task<ActionResult> BulkHardDelete([FromBody] ICollection<string> pks)
        {
            try
            {
                var affectedRecordsCount = await _service.BulkHardDelete(pks);

                return Ok(new { affectedRecordsCount });
            }
            catch (Exception ex)
            {
                return await _errorHandlingService.HandleExceptionAsync(
                    ex: ex, logger: _logger,
                    className: nameof(ProfileFunctionController), methodName: nameof(BulkHardDelete));
            }
        }
    }
}