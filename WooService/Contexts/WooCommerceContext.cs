namespace WooService.Contexts;
using Microsoft.EntityFrameworkCore;
using WooService.Models;

public class WooCommerceContext(AppSettings appsettings) : DbContext
{
    readonly string connectionString = appsettings.WooCommerceConnectionString;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(
                connectionString,
                providerOptions => { providerOptions.EnableRetryOnFailure(); });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WooPedidoProducto>()
            .HasKey(wp => new { wp.DetalleId, wp.PedidoId });
        modelBuilder.Entity<ClienteDireccion>()
            .HasKey(cd => new { cd.TipoDireccion, cd.ClienteId });
        modelBuilder.Entity<ParametrosWooCommerce>()
            .HasNoKey();
        modelBuilder.Entity<DepartamentosYMunicipios>().HasNoKey();
    }
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<ClienteDireccion> ClienteDirecciones { get; set; } = null!;
    public DbSet<WooPedido> WooPedidos { get; set; } = null!;
    public DbSet<WooPedidoProducto> WooPedidosProductos { get; set; } = null!;
    public DbSet<Pedido> Pedidos { get; set; } = null!;
    public DbSet<WooPedidoProducto> WooPedidoProductos { get; set; } = null!;
    public DbSet<ParametrosWooCommerce> ParametrosWooCommerce { get; set; } = null!;
    public DbSet<DepartamentosYMunicipios> DepartamentosYMunicipios { get; set; } = null!;
}