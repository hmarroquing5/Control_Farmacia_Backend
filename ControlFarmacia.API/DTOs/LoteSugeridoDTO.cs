public class LoteSugeridoDTO
{
    public int LoteID { get; set; }
    public string CodigoLote { get; set; }
    public string FechaVencimiento { get; set; }
    public int Disponible { get; set; }
    public string ProductoNombre { get; set; }
    public decimal PrecioCosto { get; set; }
    public decimal PrecioVentaSugerido { get; set; }
    public int BloquearVenta { get; set; }
}