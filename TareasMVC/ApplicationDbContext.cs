using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;

namespace TareasMVC
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tarea>().HasQueryFilter(t => t.FechaEliminacion == null);

            modelBuilder.Entity<Tarea>()
                .HasMany(t => t.Subtareas)
                .WithOne(t => t.TareaPadre)
                .HasForeignKey(t => t.TareaPadreId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Paso> Pasos { get; set; }
        public DbSet<ArchivoAdjunto> ArchivosAdjuntos { get; set; }
    }
}
