using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendJPMAnalysis.Services
{
    public class DataAccessService
    {
        private readonly JPMDatabaseContext _context;

        public DataAccessService(JPMDatabaseContext dbContext)
        {
            _context = dbContext;
        }


        /// <summary>
        /// Asynchronously checks for existing entities of type T in the database based on provided entity IDs.
        /// </summary>
        /// <typeparam name="T">The type of entity to check.</typeparam>
        /// <param name="entitiesIds">The list of entity IDs to check.</param>
        /// <returns>A list of existing entity IDs found in the database.</returns>
        public async Task<List<string>> CheckExistingEntitiesAsync<T>(List<string> entitiesIds) where T : class
        {
            var allEntities = await _context.Set<T>().ToListAsync();

            return allEntities
                .Where(e => entitiesIds.Contains(e.GetId()))
                .Select(e => e.GetId())
                .ToList();
        }


        /// <summary>
        /// Asynchronously checks whether an entity of type T with the specified ID exists in the database.
        /// </summary>
        /// <typeparam name="T">The type of entity to check.</typeparam>
        /// <param name="entityId">The ID of the entity to check for existence.</param>
        /// <returns>True if an entity with the specified ID exists; otherwise, false.</returns>
        public async Task<bool> EntityExistsAsync<T>(string entityId) where T : class
        {
            var result = await _context.Set<T>().ToListAsync();
            return result.Any(e => e.GetId() == entityId);
        }
    }
}