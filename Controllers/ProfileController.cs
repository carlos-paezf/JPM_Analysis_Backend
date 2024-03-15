using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using BackendJPMAnalysis.Services;
using Microsoft.AspNetCore.Mvc;


namespace BackendJPMAnalysis.Controllers
{
    [ApiController]
    [Route("profiles", Name = "Profiles")]
    [Produces("application/json")]
    public class ProfileController : ControllerBase, IBaseApiController<ProfileModel, ProfileSimpleDTO>
    {
        private readonly JPMDatabaseContext _context;

        private readonly ILogger<ProfileController> _logger;

        private readonly ProfileService _service;

        private readonly IErrorHandlingService _errorHandlingService;

        public ProfileController(
            JPMDatabaseContext context,
            ProfileService service,
            ILogger<ProfileController> logger,
            ErrorHandlingService errorHandlingService
        )
        {
            _context = context;
            _service = service;
            _logger = logger;
            _errorHandlingService = errorHandlingService;
        }


        /// <summary>
        /// This endpoint retrieves a list of profiles with the total number of results
        /// </summary>
        /// <returns>
        /// A paginated response containing a list of profiles is being returned. If there are no
        /// profiles found in the database, a "Not Found" response with a message indicating that no
        /// records were found is returned.
        /// </returns>
        /// <response code="200">Returns the profiles list. If there are no records in the DB, it returns an empty array/// </response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpGet(Name = "GetProfiles")]
        public async Task<ActionResult<ListResponseDTO<ProfileModel>>> GetAll()
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
                    className: nameof(ProfileController), methodName: nameof(GetAll));
            }
        }


        /// <summary>
        /// This endpoint retrieves an Profile entity by its id along with related profilesProfiles
        /// and companyUsers details, or a 404 code status if not found.
        /// </summary>
        /// <param name="id">
        /// The id is the unique identifier for each Profile entry.
        /// </param>
        /// <returns>
        /// The GetById method is returning an ActionResult of type Profile. If the entity is found in
        /// the database based on the provided id, it returns the entity. If the entity is
        /// not found (entity is null), it returns a NotFound result with a message indicating that the
        /// function was not found for the specified id.
        /// </returns>
        /// <response code="200">Returns an function by its id</response>
        /// <response code="404">Returns an error message by not found item</response>
        /// <response code="500">Returns an alert by Internal Server Error </response>
        [HttpGet("{id}", Name = "GetProfileById")]
        public async Task<ActionResult<ProfileModel>> GetByPk([FromRoute] string id)
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
                    className: nameof(ProfileController), methodName: nameof(GetByPk));
            }
        }


        /// <summary>
        /// This endpoint handles a POST request to create a new profile, checking for duplicate
        /// ids and returning appropriate responses.
        /// </summary>
        /// <param name="body">
        /// The code you provided is a POST method in a controller that receives
        /// an CompanyUser object in the request body. The method calls a service to save the profile
        /// information and returns an appropriate response based on the outcome.
        /// </param>
        /// <returns>
        /// The Post method in the ProfileController is returning different types of ActionResult based
        /// on the outcome of the operation:
        /// </returns>
        /// <response code="201">Returns the new profile</response>
        /// <response code="409">Returns an error message by duplicate id</response>
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPost(Name = "PostProfile")]
        public async Task<ActionResult> Post([FromBody] ProfileModel body)
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
                    className: nameof(ProfileController), methodName: nameof(Post));
            }
        }


        /// <summary>
        /// This endpoint updates an profile by its id, handling error cases and
        /// returning appropriate responses.
        /// </summary>
        /// <param name="id">The identifier for this item.</param>
        /// <param name="body">The data to update the entity</param>
        /// <returns>
        /// The code is returning an `ActionResult` asynchronously. If the update operation is
        /// successful, it returns an `Ok` response with the updated `body` object. If the
        /// `body.Id` does not match the `id` in the route, it returns a
        /// `BadRequest` response. If the existing profile is not found, it returns a `NotFound`
        /// response. If an exception occurs during the execution throw an internal server error.
        /// </returns>
        /// <response code="200">Returns the updated profile information when successful</response>
        /// <response code="400">Alerts the user about incorrect input data</response>
        /// <response code="404">The profile was not found in the database</response>  
        /// <response code="500">Returns an alert by Internal Server Error</response>
        [HttpPut("update/{id}", Name = "UpdateProfile")]
        public async Task<ActionResult> UpdateByPK([FromRoute] string id, [FromBody] ProfileSimpleDTO body)
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
                    className: nameof(ProfileController), methodName: nameof(UpdateByPK));
            }
        }
    }
}