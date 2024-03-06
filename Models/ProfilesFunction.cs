using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace BackendJPMAnalysis.Models
{
    public partial class ProfilesFunctionModel
    {
        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        [Key]
        public int Id { get; }

        [Required(ErrorMessage = "La propiedad `profileId` es requerida")]
        public string ProfileId { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `functionId` es requerida")]
        public string FunctionId { get; set; } = null!;

        [JsonIgnore]
        public virtual FunctionModel? Function { get; set; }

        [JsonIgnore]
        public virtual ProfileModel? Profile { get; set; }
    }
}
