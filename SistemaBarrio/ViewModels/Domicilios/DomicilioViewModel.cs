using System.ComponentModel.DataAnnotations;

namespace SistemaBarrio.ViewModels.Domicilios
{
    public class DomicilioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La manzana es obligatoria")]
        [Display(Name = "Manzana")]
        [StringLength(10, ErrorMessage = "Máximo 10 caracteres")]
        public string Manzana { get; set; }

        [Required(ErrorMessage = "El número de casa es obligatorio")]
        [Display(Name = "Casa")]
        [Range(1, 20, ErrorMessage = "Ingrese un número de casa válido")]
        public int? Casa { get; set; }
    }
}
