using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BackendJPMAnalysis.Helpers;

namespace BackendJPMAnalysis.Models
{
    public class DepartmentModel : BaseModel
    {
        public DepartmentModel()
        {
            CompanyUsers = new HashSet<CompanyUserModel>();
        }

        [Key]
        public string Initials { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `departmentName` es requerida")]
        public string DepartmentName { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<CompanyUserModel> CompanyUsers { get; set; }

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