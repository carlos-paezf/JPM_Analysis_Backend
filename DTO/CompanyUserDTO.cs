using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class CompanyUserDTO : BaseDTO
    {
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

        [Required(ErrorMessage = "La propiedad `userLogonType` es requerida")]
        public string UserLogonType { get; set; } = null!;

        public DateTime? UserLastLogonDt { get; set; }

        public string? UserLogonStatus { get; set; }

        public string? UserGroupMembership { get; set; }

        [Required(ErrorMessage = "La propiedad `userRole` es requerida")]
        public string UserRole { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `profileId` es requerida")]
        public string ProfileId { get; set; } = null!;
    }


    public class CompanyUserSimpleDTO : CompanyUserDTO
    {
        public CompanyUserSimpleDTO() { }

        public CompanyUserSimpleDTO(CompanyUserModel companyUser)
        {
            AccessId = companyUser.AccessId;
            UserName = companyUser.UserName;
            UserStatus = companyUser.UserStatus;
            UserType = companyUser.UserType;
            EmployeeId = companyUser.EmployeeId;
            EmailAddress = companyUser.EmailAddress;
            UserLocation = companyUser.UserLocation;
            UserCountry = companyUser.UserCountry;
            UserLogonType = companyUser.UserLogonType;
            UserLastLogonDt = companyUser.UserLastLogonDt;
            UserLogonStatus = companyUser.UserLogonStatus;
            UserGroupMembership = companyUser.UserGroupMembership;
            UserRole = companyUser.UserRole;
            ProfileId = companyUser.ProfileId;
            CreatedAt = companyUser.CreatedAt;
            UpdatedAt = companyUser.UpdatedAt;
            DeletedAt = companyUser.DeletedAt;
        }
    }


    public class CompanyUserEagerDTO : CompanyUserDTO
    {
        public ProfileSimpleDTO Profile { get; set; }
        public ICollection<UserEntitlementSimpleDTO> UserEntitlements { get; set; } = null!;

        public CompanyUserEagerDTO(CompanyUserModel companyUser, ProfileSimpleDTO profile, ICollection<UserEntitlementSimpleDTO> userEntitlementDTOs)
        {
            AccessId = companyUser.AccessId;
            UserName = companyUser.UserName;
            UserStatus = companyUser.UserStatus;
            UserType = companyUser.UserType;
            EmployeeId = companyUser.EmployeeId;
            EmailAddress = companyUser.EmailAddress;
            UserLocation = companyUser.UserLocation;
            UserCountry = companyUser.UserCountry;
            UserLogonType = companyUser.UserLogonType;
            UserLastLogonDt = companyUser.UserLastLogonDt;
            UserLogonStatus = companyUser.UserLogonStatus;
            UserGroupMembership = companyUser.UserGroupMembership;
            UserRole = companyUser.UserRole;
            ProfileId = companyUser.ProfileId;
            Profile = profile;
            UserEntitlements = userEntitlementDTOs;
            CreatedAt = companyUser.CreatedAt;
            UpdatedAt = companyUser.UpdatedAt;
            DeletedAt = companyUser.DeletedAt;
        }
    }
}