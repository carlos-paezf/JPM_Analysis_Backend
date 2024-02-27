using System.ComponentModel.DataAnnotations;

namespace BackendJPMAnalysis.Models
{
    public partial class Function : BaseEntity
    {
        public Function()
        {
            ProfilesFunctions = new HashSet<ProfilesFunction>();
            UserEntitlements = new HashSet<UserEntitlement>();
        }

        /// <summary>
        /// Function name in snake_case
        /// </summary>
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `functionName` es requerida")]
        public string FunctionName { get; set; } = null!;

        public virtual ICollection<ProfilesFunction> ProfilesFunctions { get; set; }
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
