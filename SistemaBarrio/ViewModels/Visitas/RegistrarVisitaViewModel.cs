using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SistemaBarrio.ViewModels.Visitas
{
    public class RegistrarVisitaViewModel
    {
        // Datos del visitante — si ya existe viene el Id, si es nuevo viene el resto
        public int? VisitanteId { get; set; }

        [StringLength(8, ErrorMessage = "Máximo 8 caracteres")]
        public string? Dni { get; set; }

        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string? Nombre { get; set; }

        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string? Apellido { get; set; }

        // Datos de la visita
        [Required(ErrorMessage = "El domicilio es obligatorio")]
        [Display(Name = "Domicilio")]
        public int? DomicilioId { get; set; }

        [Required(ErrorMessage = "El propietario es obligatorio")]
        [Display(Name = "Propietario")]
        public int? PropietarioId { get; set; }

        // Listas para los dropdowns
        public List<SelectListItem> Domicilios { get; set; } = new();
        public List<SelectListItem> Propietarios { get; set; } = new();
    }
}
