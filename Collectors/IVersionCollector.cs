using VersionTracker.Api.Services;

namespace VersionTracker.Api.Collectors;

public interface IVersionCollector
{
    string ProductName { get; }
    Task<IReadOnlyCollection<CollectedVersion>> CollectAsync();
}