using System.Net.Http.Headers;
using System.Text.Json;
using VersionTracker.Api.Services;

namespace VersionTracker.Api.Collectors;

public class NexusCollector(IHttpClientFactory httpClientFactory) : IVersionCollector
{
    public string ProductName => "Nexus Repository";

    public async Task<IReadOnlyCollection<CollectedVersion>> CollectAsync()
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VersionTracker", "1.0"));

        var url = "https://api.github.com/repos/sonatype/nexus-public/releases?per_page=50";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var results = new List<CollectedVersion>();
        foreach (var release in doc.RootElement.EnumerateArray())
        {
            if (release.GetProperty("prerelease").GetBoolean()) continue;
            if (release.GetProperty("draft").GetBoolean()) continue;

            var tag = release.GetProperty("tag_name").GetString() ?? string.Empty;
            var dateStr = release.GetProperty("published_at").GetString();
            var link = release.GetProperty("html_url").GetString() ?? string.Empty;

            DateTime? releaseDate = dateStr != null ? DateTime.Parse(dateStr).ToUniversalTime() : null;

            // Nexus tags look like: release-3.72.0-04
            // We normalize to: 3.72.0-04
            var version = tag.Replace("release-", string.Empty).TrimStart('v');

            results.Add(new CollectedVersion(version, releaseDate, link));
        }
        return results.AsReadOnly();
    }
}