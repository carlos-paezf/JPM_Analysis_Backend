using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class ProfileModel
    {
        private string? _profileName;

        public ProfileModel()
        {
            CompanyUsers = new HashSet<CompanyUserModel>();
            ProfilesFunctions = new HashSet<ProfileFunctionModel>();
        }

        /// <summary>
        /// Profile name in snake_case
        /// </summary>
        [Key]
        public string Id { get; private set; } = null!;

        [Required(ErrorMessage = "La propiedad `profileName` es requerida")]
        public string ProfileName
        {
            get => _profileName!;
            set
            {
                _profileName = value;
                Id ??= StringUtil.SnakeCase(value);
            }
        }

        public DateTime CreatedAt { get; }

        public DateTime UpdatedAt { get; }

        [JsonIgnore]
        public virtual ICollection<CompanyUserModel> CompanyUsers { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProfileFunctionModel> ProfilesFunctions { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, ProfileName: {ProfileName}";
        }
    }


    public static class ProfileModelExtensions
    {
        public static string GetId(this ProfileModel profile)
        {
            return EntityExtensions.GetId(profile);
        }
    }
}
