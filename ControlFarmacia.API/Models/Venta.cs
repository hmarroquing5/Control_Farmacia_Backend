using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlFarmacia.API.Models
{
    public class Venta
    {
        [Key]
        public int VentaID { get; set; }

        [Required]
        public int UsuarioID { get; set; }

        [Required]
        public DateTime FechaVenta { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string NumeroFactura { get; set; } = string.Empty; // Requerido para Auditoría (US 06)

        // Propiedades de navegación
        public Usuario? Usuario { get; set; }
        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}