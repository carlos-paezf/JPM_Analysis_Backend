using System.ComponentModel.DataAnnotations;


namespace BackendJPMAnalysis.Helpers
{
    public abstract class BaseDTO
    {
        [Required(ErrorMessage = "La propiedad `createdAt` es requerida")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "La propiedad `updatedAt` es requerida")]
        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}