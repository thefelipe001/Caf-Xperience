using CafeXperienceApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CafeXperienceApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }

        public DbSet<Campus> Campus { get; set; }

        public DbSet<Cafeteria> Cafeterias { get; set; }
        public DbSet<Marca> Marcas { get; set; }

        public DbSet<Articulo> Articulos { get; set; } 

        public DbSet<Proveedore> Proveedores { get; set; }

        public DbSet<FacturacionArticulo> FacturacionArticulos { get; set; }

    }
}
