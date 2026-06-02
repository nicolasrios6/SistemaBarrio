namespace SistemaBarrio.ViewModels.Visitas
{
    public class VisitanteEncontradoDto
    {
        public bool Encontrado { get; set; }
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }

        // ✅ nuevo — datos de autorización si existe
        public bool TieneAutorizacion { get; set; }
        public string? AutorizacionDomicilio { get; set; }
        public string? AutorizacionPropietario { get; set; }
        public int? AutorizacionDomicilioId { get; set; }
        public int? AutorizacionPropietarioId { get; set; }
        public string? AutorizacionTipo { get; set; }
    }
}
