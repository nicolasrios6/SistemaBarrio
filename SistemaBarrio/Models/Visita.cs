namespace SistemaBarrio.Models
{
    public class Visita
    {
        public int Id { get; set; }
        public int VisitanteId { get; set; }
        public Visitante Visitante { get; set; }
        public DateTime FechaHoraIngreso { get; set; }
        public DateTime? FechaHoraSalida { get; set; }
        public int DomicilioId { get; set; }
        public Domicilio Domicilio { get; set; }
        public int PropietarioId { get; set; }
        public Propietario Propietario { get; set; }
        public EstadoVisita EstadoVisita{ get; set; } = EstadoVisita.EnCurso;
    }

    public enum EstadoVisita
    {
        EnCurso,
        Finalizada
    }
}
