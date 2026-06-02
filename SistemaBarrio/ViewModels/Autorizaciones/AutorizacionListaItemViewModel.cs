namespace SistemaBarrio.ViewModels.Autorizaciones
{
    public class AutorizacionListaItemViewModel
    {
        public int Id { get; set; }
        public string NombreVisitante { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public string Propietario { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string FechaAlta { get; set; } = string.Empty;
        public string? FechaVencimiento { get; set; }
    }
}
