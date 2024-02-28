using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class FunctionDTO
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `functionName` es requerida")]
        public string FunctionName { get; set; } = null!;
    }


    public class FunctionSimpleDTO : FunctionDTO
    {
        public FunctionSimpleDTO(Function function)
        {
            Id = function.Id;
            FunctionName = function.FunctionName;
        }
    }


    public class FunctionEagerDTO : FunctionDTO
    {
        public ICollection<ProfilesFunctionSimpleDTO> ProfilesFunctions { get; set; }
        public ICollection<UserEntitlementSimpleDTO> UserEntitlements { get; set; }

        public FunctionEagerDTO(Function function, ICollection<ProfilesFunctionSimpleDTO> profilesFunctionDTOs, ICollection<UserEntitlementSimpleDTO> userEntitlementDTOs)
        {
            Id = function.Id;
            FunctionName = function.FunctionName;
            ProfilesFunctions = profilesFunctionDTOs;
            UserEntitlements = userEntitlementDTOs;
        }
    }
}