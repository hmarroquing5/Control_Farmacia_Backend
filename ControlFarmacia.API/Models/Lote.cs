namespace ControlFarmacia.API.Models
{
    public class Lote
    {
        public int LoteID { get; set; }
        public int ProductoID { get; set; }
        public string CodigoLote { get; set; } = string.Empty; // Código del fabricante (US 01)
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
        public DateTime FechaVencimiento { get; set; } // Para semaforización (US 02)
        public int CantidadActual { get; set; }
        
        public Producto? Producto { get; set; }
    }
}