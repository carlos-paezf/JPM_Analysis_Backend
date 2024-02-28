using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class Profile : BaseModel
    {
        public Profile()
        {
            Id = StringUtil.SnakeCase(ProfileName);

            CompanyUsers = new HashSet<CompanyUser>();
            ProfilesFunctions = new HashSet<ProfilesFunction>();
        }

        /// <summary>
        /// Profile name in snake_case
        /// </summary>
        [Key]
        public string Id { get; } = null!;

        [Required(ErrorMessage = "La propiedad `profileName` es requerida")]
        public string ProfileName { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<CompanyUser> CompanyUsers { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProfilesFunction> ProfilesFunctions { get; set; }
    }
}
