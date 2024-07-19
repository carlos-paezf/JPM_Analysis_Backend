using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Helpers;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.DTO
{
    public abstract class DepartmentDTO : BaseDTO
    {
        public string Initials { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `departmentName` es requerida")]
        public string DepartmentName { get; set; } = null!;
    }


    public class DepartmentSimpleDTO : DepartmentDTO
    {
        public DepartmentSimpleDTO() { }

        public DepartmentSimpleDTO(DepartmentModel department)
        {
            Initials = department.Initials;
            DepartmentName = department.DepartmentName;
            CreatedAt = department.CreatedAt;
            UpdatedAt = department.UpdatedAt;
            DeletedAt = department.DeletedAt;
        }
    }


    public class DepartmentEagerDTO : DepartmentDTO
    {
        public ICollection<CompanyUserSimpleDTO> CompanyUsers { get; set; } = null!;

        public DepartmentEagerDTO(DepartmentModel department, ICollection<CompanyUserSimpleDTO> companyUserSimpleDTO)
        {
            Initials = department.Initials;
            DepartmentName = department.DepartmentName;
            CompanyUsers = companyUserSimpleDTO;
            CreatedAt = department.CreatedAt;
            UpdatedAt = department.UpdatedAt;
            DeletedAt = department.DeletedAt;
        }
    }
}