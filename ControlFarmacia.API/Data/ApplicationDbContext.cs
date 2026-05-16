using Microsoft.EntityFrameworkCore;
using ControlFarmacia.API.Models; // Esto permite encontrar la clase Usuario

namespace ControlFarmacia.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) 
        { 
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
public DbSet<Categoria> Categorias { get; set; }
public DbSet<Lote> Lotes { get; set; }
public DbSet<Venta> Ventas { get; set; }
public DbSet<DetalleVenta> DetalleVentas { get; set; }

public DbSet<LoteSugeridoDTO> LoteSugeridoResult { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<LoteSugeridoDTO>().HasNoKey().ToView(null);
}
        
    }
}