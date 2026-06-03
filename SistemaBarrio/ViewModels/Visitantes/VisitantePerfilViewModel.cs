namespace SistemaBarrio.ViewModels.Visitantes
{
    public class VisitantePerfilViewModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public int CantidadVisitas { get; set; }
        public string? PrimeraVisita { get; set; }
        public string? UltimaVisita { get; set; }

        public List<VisitaHistorialItemViewModel> Visitas { get; set; } = new();
    }

    public class VisitaHistorialItemViewModel
    {
        public string FechaIngreso { get; set; } = string.Empty;
        public string? FechaEgreso { get; set; }
        public string Domicilio { get; set; } = string.Empty;
        public string Propietario { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
