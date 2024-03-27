using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class FunctionModel : BaseModel
    {
        private string? _functionName;

        public FunctionModel()
        {
            ProfilesFunctions = new HashSet<ProfileFunctionModel>();
            UserEntitlements = new HashSet<UserEntitlementModel>();
        }

        /// <summary>
        /// Function name in snake_case
        /// </summary>
        [Key]
        public string Id { get; private set; } = null!;

        [Required(ErrorMessage = "La propiedad `functionName` es requerida")]
        public string FunctionName
        {
            get => _functionName!;
            set
            {
                _functionName = value;
                Id ??= StringUtil.SnakeCase(value);
            }
        }

        [JsonIgnore]
        public virtual ICollection<ProfileFunctionModel> ProfilesFunctions { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserEntitlementModel> UserEntitlements { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, FunctionName: {FunctionName}";
        }
    }


    public static class FunctionModelExtensions
    {
        public static string GetId(this FunctionModel function)
        {
            return EntityExtensions.GetId(function);
        }
    }
}
