namespace VersionTracker.Api.DTOs;

public record PagedResult<T>(int Total, int Page, int PageSize, IEnumerable<T> Items);