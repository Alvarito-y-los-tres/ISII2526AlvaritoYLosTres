using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
    IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Fabricante> Fabricantes { get; set; }


    public DbSet<Herramienta> Herramientas { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<CompraItem> CompraItems { get; set; }
    public DbSet<Oferta> Ofertas { get; set; }
    public DbSet<OfertaItem> OfertaItems { get; set; }
    public DbSet<Alquiler> Alquileres { get; set; }
    public DbSet<AlquilerItem> AlquilerItems { get; set; }


    public DbSet<Reparacion> Reparaciones { get; set; }
    public DbSet<ReparacionItem> ReparacionItems { get; set; }
    
}

   