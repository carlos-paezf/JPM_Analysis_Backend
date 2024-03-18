using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class ProfileFunctionModel
    {
        private string? _functionId;

        /// <summary>
        /// ProfileID and FunctionID in snake_case
        /// </summary>
        [Key]
        public string Id { get; private set; } = null!;

        [Required(ErrorMessage = "La propiedad `profileId` es requerida")]
        public string ProfileId { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `functionId` es requerida")]
        public string FunctionId
        {
            get => _functionId!;
            set
            {
                _functionId = value;
                Id ??= StringUtil.SnakeCase(ProfileId + '_' + FunctionId);
            }
        }

        [JsonIgnore]
        public virtual FunctionModel? Function { get; set; }

        [JsonIgnore]
        public virtual ProfileModel? Profile { get; set; }
    }
}
