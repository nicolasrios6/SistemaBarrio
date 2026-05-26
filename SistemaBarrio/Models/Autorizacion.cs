namespace SistemaBarrio.Models
{
    public class Autorizacion
    {
        public int Id { get; set; }
        public int VisitanteId { get; set; }
        public Visitante Visitante { get; set; }
        public int PropietarioId { get; set; }
        public Propietario Propietario { get; set; }
        public EstadoAutorizacion EstadoAutorizacion{ get; set; }
        public DateTime FechaAlta { get; set; } = DateTime.Now;
        public DateTime? FechaVencimiento { get; set; }
    }

    public enum EstadoAutorizacion
    {
        Vigente,
        Finalizada
    }
}
