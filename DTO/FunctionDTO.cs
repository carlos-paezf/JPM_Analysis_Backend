using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class FunctionDTO : BaseDTO
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `functionName` es requerida")]
        public string FunctionName { get; set; } = null!;
    }


    public class FunctionSimpleDTO : FunctionDTO
    {
        public FunctionSimpleDTO() { }

        public FunctionSimpleDTO(FunctionModel function)
        {
            Id = function.Id;
            FunctionName = function.FunctionName;
            CreatedAt = function.CreatedAt;
            UpdatedAt = function.UpdatedAt;
            DeletedAt = function.DeletedAt;
        }

        public override string ToString()
        {
            return $"Id: {Id}, FunctionName: {FunctionName}";
        }
    }


    public class FunctionEagerDTO : FunctionDTO
    {
        public ICollection<ProfileFunctionSimpleDTO> ProfilesFunctions { get; set; }
        public ICollection<UserEntitlementSimpleDTO> UserEntitlements { get; set; }

        public FunctionEagerDTO(FunctionModel function, ICollection<ProfileFunctionSimpleDTO> profilesFunctionDTOs, ICollection<UserEntitlementSimpleDTO> userEntitlementDTOs)
        {
            Id = function.Id;
            FunctionName = function.FunctionName;
            ProfilesFunctions = profilesFunctionDTOs;
            UserEntitlements = userEntitlementDTOs;
            CreatedAt = function.CreatedAt;
            UpdatedAt = function.UpdatedAt;
            DeletedAt = function.DeletedAt;
        }
    }
}