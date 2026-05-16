using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Requerido para [Table]

namespace ControlFarmacia.API.Models
{
    [Table("Detalle_Ventas")] // <--- Aquí es donde le dices a EF que la tabla en SQL tiene guion bajo
    public class DetalleVenta
    {
        [Key]
        public int DetalleID { get; set; }

        [Required]
        public int VentaID { get; set; }

        [Required]
        public int LoteID { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioVentaUnitario { get; set; }

        // Propiedades de navegación
        [ForeignKey("VentaID")]
        public Venta? Venta { get; set; }

        [ForeignKey("LoteID")]
        public Lote? Lote { get; set; }
    }
}