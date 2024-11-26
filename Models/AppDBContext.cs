using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Final_MadonnaTizianoLab4.Models
{
    public class AppDBContext : IdentityDbContext<IdentityUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<Conductor> Conductores { get; set; }
        public DbSet<Entrega> Entregas { get; set; }
        public DbSet<EntregaDetalle> EntregaDetalles { get; set; }
        public DbSet<Localidad> Localidades { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
    }
}
