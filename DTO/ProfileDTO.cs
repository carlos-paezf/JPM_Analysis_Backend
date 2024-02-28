using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class ProfileDTO : BaseDTO
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `profileName` es requerida")]
        public string ProfileName { get; set; } = null!;
    }


    public class ProfileSimpleDTO : ProfileDTO
    {
        public ProfileSimpleDTO(Profile profile)
        {
            Id = profile.Id;
            ProfileName = profile.ProfileName;
        }
    }


    public class ProfileEagerDTO : ProfileDTO
    {
        public ICollection<CompanyUserSimpleDTO> CompanyUsers { get; set; }
        public ICollection<ProfilesFunctionSimpleDTO> ProfilesFunctions { get; set; }

        public ProfileEagerDTO(Profile profile, ICollection<CompanyUserSimpleDTO> companyUserDTOs, ICollection<ProfilesFunctionSimpleDTO> profilesFunctionDTOs)
        {
            Id = profile.Id;
            ProfileName = profile.ProfileName;
            CompanyUsers = companyUserDTOs;
            ProfilesFunctions = profilesFunctionDTOs;
        }
    }
}