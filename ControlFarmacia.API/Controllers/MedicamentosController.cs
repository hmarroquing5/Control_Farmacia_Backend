using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlFarmacia.API.Data;
using ControlFarmacia.API.Models; 

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
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_InsertarLoteMedicamento @p0, @p1, @p2, @p3, @p4", 
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
            l.LoteID,
            Nombre = l.Producto.Nombre,
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
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_ActualizarLoteMedicamento @p0, @p1, @p2",
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
        .Where(p => p.CategoriaID == categoriaId)
        .GroupBy(p => p.Nombre) 
        .Select(g => g.First())
        .ToListAsync();

    return Ok(productos);
}

[HttpGet("sugerido-fefo/{productoId}")]
public async Task<ActionResult> GetLoteSugerido(int productoId)
{
    var lote = await _context.Lotes
        .Where(l => l.ProductoID == productoId && l.CantidadActual > 0)
        .OrderBy(l => l.FechaVencimiento) 
        .Select(l => new {
            l.LoteID,
            l.CodigoLote,
            l.FechaVencimiento,
            l.CantidadActual,
            ProductoNombre = l.Producto.Nombre,
            PrecioCosto = l.Producto.PrecioCosto
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

        var param = new Microsoft.Data.SqlClient.SqlParameter("@p0", nombre);
        
        var lotes = await _context.LoteSugeridoResult
            .FromSqlRaw("EXEC sp_ObtenerLoteSugeridoFEFO @p0", param)
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
        var resultado = await _context.Database
            .SqlQueryRaw<FacturaResult>(
                "EXEC sp_FinalizarDespacho_JSON @usuarioID = {0}, @jsonDetalles = {1}", 
                request.UsuarioID, 
                request.JsonDetalles
            )
            .ToListAsync();

        var factura = resultado.FirstOrDefault();

        if (factura == null)
        {
            return BadRequest(new { message = "El servidor no devolvió un número de factura." });
        }

        return Ok(new { success = true, numeroFactura = factura.NumeroFactura });
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR EN FINALIZAR: " + ex.ToString());
        return StatusCode(500, new { message = ex.Message });
    }
}
    }
}