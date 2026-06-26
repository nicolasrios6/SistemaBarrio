namespace SistemaBarrio.ViewModels.Visitas
{
    public class VisitaActivaDto
    {
        public bool Encontrado { get; set; }
        public string? Mensaje { get; set; }
        public int? VisitaId { get; set; }
        public string? NombreVisitante { get; set; }
        public string? Patente { get; set; }
        public string? Domicilio { get; set; }
        public string? Propietario { get; set; }
        public string? HoraIngreso { get; set; }
        public string? TiempoAdentro { get; set; }
    }
}
