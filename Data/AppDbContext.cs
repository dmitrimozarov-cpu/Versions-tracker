using Microsoft.EntityFrameworkCore;
using VersionTracker.Api.Models;

namespace VersionTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVersion> ProductVersions => Set<ProductVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Уникальный индекс для комбинации ProductId и Version, чтобы предотвратить дублирование версий для одного продукта
        modelBuilder.Entity<ProductVersion>()
            .HasIndex(v => new { v.ProductId, v.Version })
            .IsUnique();

        // Индекс для оптимизации поиска по дате релиза
        modelBuilder.Entity<ProductVersion>()
            .HasIndex(v => v.ReleaseDate);
    }
}