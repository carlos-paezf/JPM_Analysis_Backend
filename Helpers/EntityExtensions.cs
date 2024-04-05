using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BackendJPMAnalysis.Helpers
{
    public static class EntityExtensions
    {
        /* public static string GetId<T>(this T entity) where T : class
        {
            var properties = typeof(T).GetProperties();
            var idProperty = properties
                .FirstOrDefault(
                    prop => prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                            || prop.Name.Equals("AccountNumber", StringComparison.OrdinalIgnoreCase)
                            || prop.Name.Equals("AccessId", StringComparison.OrdinalIgnoreCase)
                )
                ?? throw new InvalidOperationException($"No se encontró una propiedad Key en la entidad '{typeof(T).Name}'.");

            var idValue = idProperty.GetValue(entity);
            return idValue?.ToString() ?? string.Empty;
        } */

        public static string GetId<T>(this T entity) where T : class
        {
            var keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(prop => prop.GetCustomAttribute<KeyAttribute>() != null)
                ?? throw new InvalidOperationException($"No se encontró una clave primaria para la entidad '{typeof(T).Name}'");

            var idValue = keyProperty.GetValue(entity);
            return idValue?.ToString() ?? string.Empty;
        }
    }
}