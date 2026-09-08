using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Models;

namespace SistemaConsultasUVV.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Consulta> Consultas => Set<Consulta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(usuario => usuario.Email)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .Property(usuario => usuario.DataCadastro)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Consulta>()
            .HasOne(consulta => consulta.Usuario)
            .WithMany(usuario => usuario.Consultas)
            .HasForeignKey(consulta => consulta.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
