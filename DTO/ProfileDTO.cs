using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class ProfileDTO
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `profileName` es requerida")]
        public string ProfileName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    public class ProfileSimpleDTO : ProfileDTO
    {
        public ProfileSimpleDTO() { }

        public ProfileSimpleDTO(ProfileModel profile)
        {
            Id = profile.Id;
            ProfileName = profile.ProfileName;
            CreatedAt = profile.CreatedAt;
            UpdatedAt = profile.UpdatedAt;
        }

        public override string ToString()
        {
            return $"Id: {Id}, ProfileName: {ProfileName}";
        }
    }


    public class ProfileEagerDTO : ProfileDTO
    {
        public ICollection<CompanyUserSimpleDTO> CompanyUsers { get; set; }
        public ICollection<ProfileFunctionSimpleDTO> ProfilesFunctions { get; set; }

        public ProfileEagerDTO(ProfileModel profile, ICollection<CompanyUserSimpleDTO> companyUserDTOs, ICollection<ProfileFunctionSimpleDTO> profilesFunctionDTOs)
        {
            Id = profile.Id;
            ProfileName = profile.ProfileName;
            CompanyUsers = companyUserDTOs;
            ProfilesFunctions = profilesFunctionDTOs;
            CreatedAt = profile.CreatedAt;
            UpdatedAt = profile.UpdatedAt;
        }
    }
}