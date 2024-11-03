using Microsoft.EntityFrameworkCore;
using MosqueraAnthonny_ProyectoPropuesta1.Models;

namespace MosqueraAnthonny_ProyectoPropuesta1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } 
        public DbSet<Diario> Diarios { get; set; }    
    }

}
