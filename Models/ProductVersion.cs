namespace VersionTracker.Api.Models;

public class ProductVersion
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
    public string SourceUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // навигация для связи с продуктом
    public Product Product { get; set; } = null!;
}