using System.ComponentModel.DataAnnotations;

namespace BackendJPMAnalysis.DTO
{
    public class UserDepartmentAssignmentDTO
    {
        [Required(ErrorMessage = "La propiedad accessId es requerida")]
        public string AccessId { get; set; }

        [Required(ErrorMessage = "La propiedad departmentInitials es requerida")]
        public string DepartmentInitials { get; set; }

        public UserDepartmentAssignmentDTO(string accessId, string departmentInitials)
        {
            AccessId = accessId;
            DepartmentInitials = departmentInitials;
        }
    }
}