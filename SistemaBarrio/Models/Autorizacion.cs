namespace SistemaBarrio.Models
{
    public class Autorizacion
    {
        public int Id { get; set; }
        public int VisitanteId { get; set; }
        public Visitante Visitante { get; set; } = null!;

        public int PropietarioId { get; set; }
        public Propietario Propietario { get; set; } = null!;

        public int DomicilioId { get; set; }
        public Domicilio Domicilio { get; set; } = null!;

        public TipoAutorizacion TipoAutorizacion { get; set; }
        public EstadoAutorizacion EstadoAutorizacion { get; set; } = EstadoAutorizacion.Vigente;

        public DateTime FechaAlta { get; set; } = DateTime.Now;
        public DateTime? FechaVencimiento { get; set; }
    }

    public enum TipoAutorizacion
    {
        Temporal,
        Frecuente
    }

    public enum EstadoAutorizacion
    {
        Vigente,
        Finalizada
    }
}
