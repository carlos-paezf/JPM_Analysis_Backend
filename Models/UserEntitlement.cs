using System.ComponentModel.DataAnnotations;


namespace BackendJPMAnalysis.Models
{
    public partial class UserEntitlement : BaseEntity
    {
        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        public int Id { get; }

        [Required(ErrorMessage = "La propiedad `accessId` es requerida")]
        public string AccessId { get; set; } = null!;

        public string? FunctionType { get; set; }

        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "La propiedad `productId` es requerida")]
        public string ProductId { get; set; } = null!;

        public string? FunctionId { get; set; }

        public virtual CompanyUser? Access { get; set; }
        public virtual Account? AccountNumberNavigation { get; set; }
        public virtual Function? Function { get; set; }
        public virtual Product? Product { get; set; }
    }
}
