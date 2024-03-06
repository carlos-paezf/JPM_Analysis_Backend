using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class ProfilesFunctionDTO
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La propiedad `profileId` es requerida")]
        public string ProfileId { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `functionId` es requerida")]
        public string FunctionId { get; set; } = null!;
    }


    public class ProfilesFunctionSimpleDTO : ProfilesFunctionDTO
    {
        public ProfilesFunctionSimpleDTO() { }

        public ProfilesFunctionSimpleDTO(ProfilesFunction profilesFunction)
        {
            Id = profilesFunction.Id;
            ProfileId = profilesFunction.ProfileId;
            FunctionId = profilesFunction.FunctionId;
        }
    }


    public class ProfilesFunctionEagerDTO : ProfilesFunctionDTO
    {
        public FunctionSimpleDTO Function { get; set; }
        public ProfileSimpleDTO Profile { get; set; }

        public ProfilesFunctionEagerDTO(ProfilesFunction profilesFunction, FunctionSimpleDTO functionDTO, ProfileSimpleDTO profileDTO)
        {
            Id = profilesFunction.Id;
            ProfileId = profilesFunction.ProfileId;
            FunctionId = profilesFunction.FunctionId;
            Function = functionDTO;
            Profile = profileDTO;
        }
    }
}