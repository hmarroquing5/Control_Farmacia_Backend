namespace ControlFarmacia.API.DTOs
{
    public class HistorialVentaDTO
    {
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string CodigoLote { get; set; } = string.Empty;
        public int CantidadDespachada { get; set; }
    }
}