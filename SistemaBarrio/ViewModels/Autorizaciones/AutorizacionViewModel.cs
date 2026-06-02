using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBarrio.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaBarrio.ViewModels.Autorizaciones
{
    public class AutorizacionViewModel
    {
        // Datos del visitante
        public int? VisitanteId { get; set; }
        public string? Dni { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        // Datos de la autorización
        [Required(ErrorMessage = "El domicilio es obligatorio")]
        [Display(Name = "Domicilio")]
        public int? DomicilioId { get; set; }

        [Required(ErrorMessage = "El propietario es obligatorio")]
        [Display(Name = "Propietario")]
        public int? PropietarioId { get; set; }

        [Required(ErrorMessage = "El tipo de autorización es obligatorio")]
        [Display(Name = "Tipo de autorización")]
        public TipoAutorizacion? TipoAutorizacion { get; set; }

        // Listas para los dropdowns
        public List<SelectListItem> Domicilios { get; set; } = new();
        public List<SelectListItem> Propietarios { get; set; } = new();
    }
}
