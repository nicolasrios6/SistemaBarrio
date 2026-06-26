namespace SistemaBarrio.ViewModels.Visitas
{
    public class VisitanteEncontradoDto
    {
        public bool Encontrado { get; set; }
        public bool Vetado { get; set; }
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        public string? Patente { get; set; }

        public bool TieneAutorizacion { get; set; }
        public List<AutorizacionDto> Autorizaciones { get; set; } = new();

        public class AutorizacionDto
        {
            public int DomicilioId { get; set; }
            public int PropietarioId { get; set; }
            public string Domicilio { get; set; } = string.Empty;
            public string Propietario { get; set; } = string.Empty;
            public string Tipo { get; set; } = string.Empty;
        }
    }
}
