using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class ProfileModel : BaseModel
    {
        public ProfileModel()
        {
            Id = StringUtil.SnakeCase(ProfileName);

            CompanyUsers = new HashSet<CompanyUserModel>();
            ProfilesFunctions = new HashSet<ProfilesFunctionModel>();
        }

        /// <summary>
        /// Profile name in snake_case
        /// </summary>
        [Key]
        public string Id { get; } = null!;

        [Required(ErrorMessage = "La propiedad `profileName` es requerida")]
        public string ProfileName { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<CompanyUserModel> CompanyUsers { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProfilesFunctionModel> ProfilesFunctions { get; set; }
    }
}
