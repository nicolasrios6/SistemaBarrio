namespace SistemaBarrio.Models
{
    public class Visitante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public ICollection<Visita> Visitas { get; set; } = new List<Visita>();
        public ICollection<Autorizacion> Autorizaciones { get; set; } = new List<Autorizacion>();
    }
}
