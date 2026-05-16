using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly string _connectionString;

    public DashboardController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> GetDashboardResumen()
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(
                    "sp_ObtenerDashboardFEFO", 
                    commandType: System.Data.CommandType.StoredProcedure))
                {
                    var kpis = await multi.ReadFirstOrDefaultAsync<DashboardKpiDto>();

                    var lotes = (await multi.ReadAsync<LoteDashboardDto>()).ToList();

                    return Ok(new
                    {
                        kpis = new
                        {
                            stockTotal = kpis?.StockTotal ?? 0,
                            vencenPronto = kpis?.VencenPronto ?? 0,
                            vencidos = kpis?.Vencidos ?? 0,
                            valorEnRiesgo = kpis?.ValorEnRiesgo ?? 0
                        },
                        lotes = lotes
                    });
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno al obtener métricas", detail = ex.Message });
        }
    }
}