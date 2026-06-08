namespace SistemaBarrio.ViewModels.Visitas
{
    public class HistorialVisitaItemViewModel
    {
        public string NombreVisitante { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public string Propietario { get; set; } = string.Empty;
        public string HoraIngreso { get; set; } = string.Empty;
        public string? HoraEgreso { get; set; }
        public string Duracion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaHoraIngreso { get; set; } // para agrupar por día
    }

    public class HistorialViewModel
    {
        public List<HistorialVisitaItemViewModel> Visitas { get; set; } = new();
        public DateTime? FechaFiltro { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    }
}
