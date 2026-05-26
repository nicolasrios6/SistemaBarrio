namespace SistemaBarrio.Models
{
    public class Propietario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public int DomicilioId { get; set; }
        public Domicilio Domicilio { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<Autorizacion> Autorizaciones { get; set; } = new List<Autorizacion>();
    }
}
