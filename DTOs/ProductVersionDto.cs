namespace VersionTracker.Api.DTOs;

public record ProductVersionDto(string Version, DateTime? ReleaseDate, string SourceUrl);