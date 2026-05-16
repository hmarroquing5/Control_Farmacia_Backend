namespace ControlFarmacia.API.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
        public decimal PrecioCosto { get; set; } // Requerido para US 05
        public int CategoriaID { get; set; }
        
        // Propiedades de navegación
        public Categoria? Categoria { get; set; }
        public ICollection<Lote> Lotes { get; set; } = new List<Lote>();
    }
}