namespace VersionTracker.Api.Models;

public class Product
{
	// инкапсуляция данных продукта
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Vendor { get; set; } = string.Empty;
	public string SourceUrl { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	// навигация для связи с версиями продукта
	public ICollection<ProductVersion> Versions { get; set; } = new List<ProductVersion>();
}