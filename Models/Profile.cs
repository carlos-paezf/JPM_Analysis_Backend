using System.ComponentModel.DataAnnotations;


namespace BackendJPMAnalysis.Models
{
    public partial class Profile : BaseEntity
    {
        public Profile()
        {
            CompanyUsers = new HashSet<CompanyUser>();
            ProfilesFunctions = new HashSet<ProfilesFunction>();
        }

        /// <summary>
        /// Profile name in snake_case
        /// </summary>
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `profileName` es requerida")]
        public string ProfileName { get; set; } = null!;

        public virtual ICollection<CompanyUser> CompanyUsers { get; set; }
        public virtual ICollection<ProfilesFunction> ProfilesFunctions { get; set; }
    }
}
