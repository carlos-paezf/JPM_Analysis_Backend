using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class ProfileFunctionDTO
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `profileId` es requerida")]
        public string ProfileId { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `functionId` es requerida")]
        public string FunctionId { get; set; } = null!;
    }


    public class ProfileFunctionSimpleDTO : ProfileFunctionDTO
    {
        public ProfileFunctionSimpleDTO() { }

        public ProfileFunctionSimpleDTO(ProfileFunctionModel profilesFunction)
        {
            Id = profilesFunction.Id;
            ProfileId = profilesFunction.ProfileId;
            FunctionId = profilesFunction.FunctionId;
        }
    }


    public class ProfileFunctionEagerDTO : ProfileFunctionDTO
    {
        public FunctionSimpleDTO Function { get; set; }
        public ProfileSimpleDTO Profile { get; set; }

        public ProfileFunctionEagerDTO(ProfileFunctionModel profilesFunction, FunctionSimpleDTO functionDTO, ProfileSimpleDTO profileDTO)
        {
            Id = profilesFunction.Id;
            ProfileId = profilesFunction.ProfileId;
            FunctionId = profilesFunction.FunctionId;
            Function = functionDTO;
            Profile = profileDTO;
        }
    }
}