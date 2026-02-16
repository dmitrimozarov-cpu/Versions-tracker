using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersionTracker.Api.Collectors;
using VersionTracker.Api.Data;
using VersionTracker.Api.DTOs;
using VersionTracker.Api.Services;

namespace VersionTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    AppDbContext db,
    IEnumerable<IVersionCollector> collectors,
    VersionStorageService storageService) : ControllerBase
{
    // GET /api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await db.Products
            .Select(p => new ProductDto(p.Id, p.Name, p.Vendor, p.SourceUrl, p.CreatedAt))
            .ToListAsync();
        return Ok(products);
    }

    // GET /api/products/{productId}/versions?page=1&pageSize=20
    [HttpGet("{productId}/versions")]
    public async Task<ActionResult<PagedResult<ProductVersionDto>>> GetVersions(
        int productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) return BadRequest("Page must be >= 1");
        if (pageSize < 1 || pageSize > 100) return BadRequest("PageSize must be 1-100");

        var productExists = await db.Products.AnyAsync(p => p.Id == productId);
        if (!productExists) return NotFound($"Product {productId} not found");

        var query = db.ProductVersions.Where(v => v.ProductId == productId);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(v => v.ReleaseDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new ProductVersionDto(v.Version, v.ReleaseDate, v.SourceUrl))
            .ToListAsync();

        return Ok(new PagedResult<ProductVersionDto>(total, page, pageSize, items));
    }

    // POST /api/products/collect — triggers all collectors
    [HttpPost("collect")]
    public async Task<IActionResult> TriggerCollection()
    {
        var results = new List<string>();
        foreach (var collector in collectors)
        {
            try
            {
                var versions = await collector.CollectAsync();
                await storageService.StoreVersionsAsync(collector.ProductName, versions);
                results.Add($"{collector.ProductName}: {versions.Count} versions collected");
            }
            catch (Exception ex)
            {
                results.Add($"{collector.ProductName}: ERROR - {ex.Message}");
            }
        }
        return Ok(results);
    }
}