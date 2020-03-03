using CasaShowAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaShowAPI.Data
{
    public class ApplicationDbContext : DbContext
    {  
        public DbSet<CasaDeShow> CasaShow {get; set;}
        public DbSet<Evento> Eventos {get; set;}
        public DbSet<Categoria> Categorias {get; set;}
        public DbSet<Usuario> Usuarios {get; set;}
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
