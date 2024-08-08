using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;


namespace BackendJPMAnalysis.Models
{
    public partial class CompanyUserModel : BaseModel
    {
        private string _emailAddress = "";

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
        public string EmailAddress
        {
            get => _emailAddress;
            set
            {
                _emailAddress = value;
                WindowsUserId = _emailAddress.Split('@')[0].ToLower();
            }
        }

        [Required(ErrorMessage = "La propiedad `windowsUserId` es requerida")]
        public string WindowsUserId { get; set; } = null!;


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

        public string? DepartmentInitials { get; set; }

        [JsonIgnore]
        public virtual ProfileModel? Profile { get; set; } = null!;

        [JsonIgnore]
        public virtual DepartmentModel? Department { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<UserEntitlementModel> UserEntitlements { get; set; }

        public override string ToString()
        {
            return $"AccessId: {AccessId}, UserName: {UserName}, UserStatus: {UserStatus}, UserType: {UserType}, EmployeeId: {EmployeeId}, EmailAddress: {EmailAddress}, UserLocation: {UserLocation}, UserCountry: {UserCountry}, UserLogonType: {UserLogonType}, UserLastLogonDt: {UserLastLogonDt}, UserLogonStatus: {UserLogonStatus}, UserGroupMembership: {UserGroupMembership}, UserRole: {UserRole}, ProfileId: {ProfileId}";
        }
    }

    public static class CompanyUserModelExtensions
    {
        public static string GetId(this CompanyUserModel companyUser)
        {
            return EntityExtensions.GetId(companyUser);
        }
    }
}
