namespace ControlFarmacia.API.DTOs
{
    public class LoteSemaforoDTO
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public int DiasParaVencer => (FechaVencimiento - DateTime.Now).Days;
        
        // Lógica de colores (US 02)
        public string ColorSemaforo => DiasParaVencer <= 30 ? "Rojo" : 
                                       DiasParaVencer <= 90 ? "Amarillo" : "Verde";
    }
}