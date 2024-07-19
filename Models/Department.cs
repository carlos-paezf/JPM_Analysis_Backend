using System.ComponentModel.DataAnnotations;
using BackendJPMAnalysis.Helpers;

namespace BackendJPMAnalysis.Models
{
    public class DepartmentModel : BaseModel
    {
        [Key]
        public string Initials { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `departmentName` es requerida")]
        public string DepartmentName { get; set; } = null!;

        public virtual ICollection<CompanyUserModel> CompanyUsers { get; set; } = null!;

        public override string ToString()
        {
            return $"Initials: {Initials}, DepartmentName: {DepartmentName}";
        }
    }


    public static class DepartmentModelExtensions
    {
        public static string GetId(this DepartmentModel department)
        {
            return EntityExtensions.GetId(department);
        }
    }
}