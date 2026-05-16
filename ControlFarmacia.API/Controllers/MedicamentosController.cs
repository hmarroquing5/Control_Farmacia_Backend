using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlFarmacia.API.Data;
using ControlFarmacia.API.Models; 
using MySql.Data.MySqlClient; 

namespace ControlFarmacia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicamentosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MedicamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("nuevo-lote")]
        public async Task<IActionResult> AgregarLote([FromBody] IngresoLoteDTO request)
        {
            try 
            {
                // MySQL usa CALL en lugar de EXEC
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_InsertarLoteMedicamento({0}, {1}, {2}, {3}, {4})", 
                    request.Nombre,
                    request.PrecioCosto,
                    request.CategoriaID,
                    request.CantidadActual,
                    request.FechaVencimiento
                );

                return Ok(new { message = "Medicamento y Lote registrados exitosamente." });
            }
            catch (Exception ex) 
            {
                return BadRequest(new { message = "Error al procesar el ingreso: " + ex.Message });
            }
        }

        [HttpGet("siguiente-correlativo")]
        public async Task<IActionResult> GetSiguienteCorrelativo()
        {
            // Corregido a tus propiedades reales en Mayúsculas
            var ultimoProductoId = await _context.Productos.MaxAsync(p => (int?)p.ProductoID) ?? 0;
            var ultimoLoteId = await _context.Lotes.MaxAsync(l => (int?)l.LoteID) ?? 0;

            return Ok(new {
                siguienteCodigoBarras = $"CODBAR-{(ultimoProductoId + 1).ToString("D5")}",
                siguienteLote = $"LOTE-{(ultimoLoteId + 1).ToString("D5")}"
            });
        }

        [HttpGet("buscar-lote/{codigo}")]
        public async Task<IActionResult> BuscarLote(string codigo)
        {
            var lote = await _context.Lotes
                .Include(l => l.Producto) 
                .Where(l => l.CodigoLote == codigo)
                .Select(l => new {
                    l.LoteID,                 // Propiedad en Mayúscula
                    Nombre = l.Producto.Nombre, // Propiedad en Mayúscula
                    Lote = l.CodigoLote,
                    CantidadActual = l.CantidadActual,
                    Vence = l.FechaVencimiento.ToString("yyyy-MM-dd")
                })
                .FirstOrDefaultAsync();

            if (lote == null) return NotFound(new { message = "El lote no existe." });

            return Ok(lote);
        }

        [HttpPut("actualizar-lote/{id}")]
        public async Task<IActionResult> ActualizarLote(int id, [FromBody] ActualizarLoteRequest request)
        {
            try
            {
                // MySQL usa CALL
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_ActualizarLoteMedicamento({0}, {1}, {2})",
                    id,
                    request.Cantidad,
                    request.Vencimiento
                );

                return Ok(new { message = "Lote actualizado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductosPorCategoria(int categoriaId)
        {
            var productos = await _context.Productos
                .Where(p => p.CategoriaID == categoriaId) // Propiedad en Mayúscula
                .GroupBy(p => p.Nombre)                   // Propiedad en Mayúscula
                .Select(g => g.First())
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("sugerido-fefo/{productoId}")]
        public async Task<ActionResult> GetLoteSugerido(int productoId)
        {
            var lote = await _context.Lotes
                .Where(l => l.ProductoID == productoId && l.CantidadActual > 0) // Propiedad en Mayúscula
                .OrderBy(l => l.FechaVencimiento) 
                .Select(l => new {
                    l.LoteID,
                    l.CodigoLote,
                    l.FechaVencimiento,
                    l.CantidadActual,
                    ProductoNombre = l.Producto.Nombre,      // Propiedad en Mayúscula
                    PrecioCosto = l.Producto.PrecioCosto     // Propiedad en Mayúscula
                })
                .FirstOrDefaultAsync();

            if (lote == null)
            {
                return NotFound("No hay lotes disponibles con stock para este producto.");
            }
            return Ok(lote);
        }

        [HttpGet("sugerido-fefo-sp")]
        public async Task<IActionResult> GetLoteSugeridoSP([FromQuery] string nombre)
        {
            try 
            {
                if (string.IsNullOrEmpty(nombre)) return BadRequest("El nombre es requerido");

                var param = new MySqlParameter("@p0", nombre);
                
                var lotes = await _context.LoteSugeridoResult
                    .FromSqlRaw("CALL sp_ObtenerLoteSugeridoFEFO(@p0)", param)
                    .ToListAsync();

                if (lotes == null || lotes.Count == 0)
                    return NotFound(new { message = "No hay stock para este producto." });

                return Ok(lotes);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

[HttpPost("finalizar")]
public async Task<IActionResult> FinalizarVenta([FromBody] FinalizarVentaDTO request)
{
    try
    {
        // 1. Creamos los parámetros de forma explícita asegurando su tipo
        var paramUsuario = new MySqlParameter("@p_UsuarioId", MySqlDbType.VarChar) { Value = request.UsuarioID };
        var paramJson = new MySqlParameter("@p_JsonDetalles", MySqlDbType.LongText) { Value = request.JsonDetalles };

        // 2. Ejecutamos pasando los parámetros tipados
        var resultado = await _context.Database
            .SqlQueryRaw<FacturaResult>(
                "CALL sp_FinalizarDespacho_JSON(@p_UsuarioId, @p_JsonDetalles)", 
                paramUsuario, 
                paramJson
            )
            .ToListAsync();

        var factura = resultado.FirstOrDefault();

        if (factura == null)
        {
            return BadRequest(new { message = "El servidor no devolvió un número de factura." });
        }

        // Si el procedimiento devolvió un código de error personalizado interno
        if (factura.NumeroFactura.StartsWith("ERR-"))
        {
            return BadRequest(new { message = $"Error en BD: {factura.NumeroFactura}" });
        }

        return Ok(new { success = true, numeroFactura = factura.NumeroFactura });
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR REAL EN FINALIZAR: " + ex.ToString());
        return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message, stack = ex.StackTrace });
    }
}
    }
}