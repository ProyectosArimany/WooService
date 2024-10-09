namespace WooService.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WooService.Models;

public class AXContext(AppSettings appsettings) : DbContext
{
    readonly string connectionString = appsettings.AXConnectionString;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(
                connectionString,
                providerOptions => { providerOptions.EnableRetryOnFailure(); });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogoProductos>()
            .HasKey(cp => cp.CodigoProducto);
        modelBuilder.Entity<ClienteAX>().HasNoKey();
        modelBuilder.Entity<ParametrosClientesNuevosWEB>().HasNoKey();
        modelBuilder.Entity<ParametrosClientesNuevosWEB>().Property(e => e.Bloqueo)
                                                          .HasConversion<int>();
        modelBuilder.Entity<ParametrosClientesNuevosWEB>().Property(e => e.IncluyeImpuesto)
                                                          .HasConversion<int>();
        modelBuilder.Entity<ParametrosClientesNuevosWEB>().Property(e => e.LimitarCredito)
                                                          .HasConversion<int>();
    }
    public DbSet<CatalogoProductos> CatalogoProductos { get; set; } = null!;
    public DbSet<ClienteAX> ClientesAX { get; set; } = null!;
    public DbSet<ParametrosClientesNuevosWEB> ParametrosClientesNuevosWEB { get; set; }


}