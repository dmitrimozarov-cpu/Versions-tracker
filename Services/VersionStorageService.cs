using Microsoft.EntityFrameworkCore;
using VersionTracker.Api.Data;
using VersionTracker.Api.Models;

namespace VersionTracker.Api.Services;

public class VersionStorageService(AppDbContext db)
{
    public async Task StoreVersionsAsync(string productName, IReadOnlyCollection<CollectedVersion> versions)
    {
        // Находим продукт по имени, если его нет - создаем новый
        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Name == productName);

        if (product == null)
        {
            product = new Product { Name = productName, Vendor = string.Empty, SourceUrl = string.Empty };
            db.Products.Add(product);
            await db.SaveChangesAsync();
        }

        // Собираем все существующие версии для данного продукта, чтобы не добавлять дубликаты
        var existingVersions = await db.ProductVersions
            .Where(v => v.ProductId == product.Id)
            .Select(v => v.Version)
            .ToHashSetAsync();

        // Добавляем только новые версии, которых еще нет в базе
        var newVersions = versions
            .Where(v => !existingVersions.Contains(v.Version))
            .Select(v => new ProductVersion
            {
                ProductId = product.Id,
                Version = v.Version,
                ReleaseDate = v.ReleaseDate,
                SourceUrl = v.SourceUrl
            });

        db.ProductVersions.AddRange(newVersions);
        await db.SaveChangesAsync();
    }
}