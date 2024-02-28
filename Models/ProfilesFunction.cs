using System.ComponentModel.DataAnnotations;


namespace BackendJPMAnalysis.Models
{
    public partial class ProfilesFunction
    {
        /// <summary>
        /// Auto-incremental ID
        /// </summary>
        [Key]
        public int Id { get; }

        [Required(ErrorMessage = "La propiedad `profileId` es requerida")]
        public string ProfileId { get; set; } = null!;

        [Required(ErrorMessage = "La propiedad `functionId` es requerida")]
        public string FunctionId { get; set; } = null!;

        public virtual Function? Function { get; set; }
        public virtual Profile? Profile { get; set; }
    }
}
