using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SistemaBarrio.ViewModels.Propietarios
{
    public class PropietarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El domicilio es obligatorio")]
        [Display(Name = "Domicilio")]
        public int? DomicilioId { get; set; }

        // Lista para el dropdown — no se valida, solo se usa en la vista
        public List<SelectListItem> Domicilios { get; set; } = new();
    }
}
