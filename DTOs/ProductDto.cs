namespace VersionTracker.Api.DTOs;

public record ProductDto(int Id, string Name, string Vendor, string SourceUrl, DateTime CreatedAt);