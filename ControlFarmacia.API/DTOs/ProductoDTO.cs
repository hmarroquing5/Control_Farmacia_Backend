namespace ControlFarmacia.API.DTOs
{
    public class ProductoDTO
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
    }
}