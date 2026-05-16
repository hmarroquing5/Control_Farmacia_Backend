namespace ControlFarmacia.API.Models
{
    public class IngresoLoteDTO
    {
        // Datos para la tabla Producto
        public string Nombre { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
        public decimal PrecioCosto { get; set; } // Cambiado a decimal por precisión financiera
        public int CategoriaID { get; set; }

        // Datos para la tabla Lote
        public string CodigoLote { get; set; } = string.Empty; 
        public int CantidadActual { get; set; }
        public DateTime FechaVencimiento { get; set; } 
    }
}