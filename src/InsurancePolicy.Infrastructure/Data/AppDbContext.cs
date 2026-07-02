using InsurancePolicy.Domain;
using Microsoft.EntityFrameworkCore;

namespace InsurancePolicy.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Apolice> Apolices => Set<Apolice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apolice>(entity =>
        {
            entity.ToTable("Apolices");

            entity.HasKey(a => a.Id);

            entity.HasIndex(a => a.NumeroApolice)
                .IsUnique();

            entity.Property(a => a.NumeroApolice)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(a => a.CpfCnpjSegurado)
                .HasMaxLength(14)
                .IsRequired();

            entity.Property(a => a.PlacaVeiculo)
                .HasMaxLength(8)
                .IsRequired();

            entity.Property(a => a.ValorPremio)
                .HasPrecision(18, 2);

            entity.Property(a => a.Status)
                .HasConversion<int>();
        });
    }
}
