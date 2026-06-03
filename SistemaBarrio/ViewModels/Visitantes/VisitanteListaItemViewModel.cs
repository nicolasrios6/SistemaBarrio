namespace SistemaBarrio.ViewModels.Visitantes
{
    public class VisitanteListaItemViewModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public int CantidadVisitas { get; set; }
        public string? UltimaVisita { get; set; }
    }
}
