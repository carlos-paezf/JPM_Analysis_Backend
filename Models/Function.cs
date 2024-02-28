using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class Function : BaseModel
    {
        public Function()
        {
            Id = StringUtil.SnakeCase(FunctionName);

            ProfilesFunctions = new HashSet<ProfilesFunction>();
            UserEntitlements = new HashSet<UserEntitlement>();
        }

        /// <summary>
        /// Function name in snake_case
        /// </summary>
        [Key]
        public string Id { get; } = null!;

        [Required(ErrorMessage = "La propiedad `functionName` es requerida")]
        public string FunctionName { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<ProfilesFunction> ProfilesFunctions { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserEntitlement> UserEntitlements { get; set; }
    }
}
