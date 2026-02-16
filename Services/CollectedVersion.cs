namespace VersionTracker.Api.Services;

public record CollectedVersion(
    string Version,
    DateTime? ReleaseDate,
    string SourceUrl
);