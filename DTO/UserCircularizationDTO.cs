using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public class UserCircularizationDTO
    {
        public string Name { get; set; }

        public string AccessId { get; set; }

        public string DepartmentInitials { get; set; }

        public string DepartmentName { get; set; }

        public string ProfileName { get; set; }

        public bool Status { get; set; }

        public string Email { get; set; }

        public UserCircularizationDTO(CompanyUserModel companyUser, DepartmentModel? department, ProfileModel profile)
        {
            Name = companyUser.UserName;
            AccessId = companyUser.AccessId;
            DepartmentInitials = department?.Initials ?? "No Department";
            DepartmentName = department?.DepartmentName ?? "No Department";
            ProfileName = profile.ProfileName;
            Status = companyUser.UserStatus;
            Email = companyUser.EmailAddress;
        }
    }
}