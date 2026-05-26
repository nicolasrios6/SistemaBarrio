namespace SistemaBarrio.Models
{
    public class Domicilio
    {
        public int Id { get; set; }
        public string Manzana { get; set; }
        public int Casa { get; set; }
        public ICollection<Propietario> Propietarios { get; set; } = new List<Propietario>();
    }
}
