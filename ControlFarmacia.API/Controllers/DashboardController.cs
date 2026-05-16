using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly string _connectionString;

    public DashboardController(IConfiguration configuration)
    {
        // UNIFICADO CON PROGRAM.CS: Busca de forma robusta en Docker o variables de entorno
        _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") 
                            ?? configuration.GetConnectionString("DB_CONNECTION") 
                            ?? configuration["DB_CONNECTION"] 
                            ?? string.Empty;
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> GetDashboardResumen()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            return StatusCode(500, new { message = "Error de configuración: Cadena de conexión no encontrada." });
        }

        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // Abrimos de forma explícita y asíncrona para MySQL

                // Ejecución directa compatible y fluida para el driver de MySQL
                using (var multi = await connection.QueryMultipleAsync(
                    "CALL sp_ObtenerDashboardFEFO()", // Usamos sintaxis explícita CALL
                    commandType: System.Data.CommandType.Text)) 
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