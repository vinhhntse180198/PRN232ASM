using Xunit;

namespace Gateway.Tests;

public class GatewaySmokeTests
{
    [Theory]
    [InlineData("/api/auth/login")]
    [InlineData("/api/papers")]
    [InlineData("/api/notifications")]
    [InlineData("/api/trends")]
    [InlineData("/api/sync/trigger")]
    public void Public_api_paths_are_under_api_prefix(string path)
    {
        Assert.StartsWith("/api/", path);
    }
}
