using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class FunctionModel : BaseModel
    {
        public FunctionModel()
        {
            Id = StringUtil.SnakeCase(FunctionName);

            ProfilesFunctions = new HashSet<ProfilesFunctionModel>();
            UserEntitlements = new HashSet<UserEntitlementModel>();
        }

        /// <summary>
        /// Function name in snake_case
        /// </summary>
        [Key]
        public string Id { get; } = null!;

        [Required(ErrorMessage = "La propiedad `functionName` es requerida")]
        public string FunctionName { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<ProfilesFunctionModel> ProfilesFunctions { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserEntitlementModel> UserEntitlements { get; set; }
    }
}
