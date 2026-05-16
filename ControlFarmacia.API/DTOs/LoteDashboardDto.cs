public class LoteDashboardDto
{
    public int LoteID { get; set; }
    public required string Producto { get; set; }
    public required string CodigoLote { get; set; }
    public int CantidadActual { get; set; }
    public decimal PrecioCosto { get; set; }      // Nuevo
    public decimal CostoTotalLote { get; set; }   // Nuevo
    public DateTime FechaVencimiento { get; set; }
    public int DiasParaVencer { get; set; }
}