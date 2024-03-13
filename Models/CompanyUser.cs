using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class CompanyUserModel : BaseModel
    {
        public CompanyUserModel()
        {
            UserEntitlements = new HashSet<UserEntitlementModel>();
        }

        [Key]
        [Required(ErrorMessage = "La propiedad `accessId` es requerida")]
        public string AccessId { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `username` es requerida")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `userStatus` es requerida")]
        public bool UserStatus { get; set; } = false;

        [Required(ErrorMessage = "La propiedad `userType` es requerida")]
        public string UserType { get; set; } = null!;

        public string? EmployeeId { get; set; }

        [Required(ErrorMessage = "La propiedad `emailAddress` es requerida")]
        public string EmailAddress { get; set; } = null!;

        public string? UserLocation { get; set; }

        [Required(ErrorMessage = "La propiedad `userCountry` es requerida")]
        public string UserCountry { get; set; } = null!;

        /// <summary>
        /// RSA Token or Password
        /// </summary>
        [Required(ErrorMessage = "La propiedad `userLogonType` es requerida")]
        public string UserLogonType { get; set; } = null!;

        public DateTime? UserLastLogonDt { get; set; }

        [Required(ErrorMessage = "La propiedad `userLogonStatus` es requerida")]
        public string UserLogonStatus { get; set; } = null!;

        public string? UserGroupMembership { get; set; }

        public string? UserRole { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `profileId` es requerida")]
        public string ProfileId { get; set; } = null!;

        [JsonIgnore]
        public virtual ProfileModel Profile { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<UserEntitlementModel> UserEntitlements { get; set; }
    }
}
