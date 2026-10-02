using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Agent365.GoldenAgent.Tests;

public sealed class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_IsAlwaysLive()
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Ready_IsUnavailable_WhenModelIsNotConfigured()
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync("/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task SafeConfig_DoesNotExposeSecrets()
    {
        using var client = _factory.CreateClient();

        var json = await client.GetStringAsync("/api/config");

        Assert.False(json.Contains("clientSecret", StringComparison.OrdinalIgnoreCase));
        Assert.False(json.Contains("\"apiKey\"", StringComparison.OrdinalIgnoreCase));
        Assert.True(json.Contains("requireApiKey", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ApiKey_CanProtectOperationalEndpoints()
    {
        using var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Api:RequireApiKey", "true");
            builder.UseSetting("Api:ApiKey", "unit-test-secret");
        });

        using var client = factory.CreateClient();

        using var unauthorized = await client.GetAsync("/api/diagnostics");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        client.DefaultRequestHeaders.Add("X-Api-Key", "unit-test-secret");

        using var authorized = await client.GetAsync("/api/diagnostics");
        Assert.Equal(HttpStatusCode.OK, authorized.StatusCode);
    }

    [Fact]
    public async Task Chat_RejectsOversizedMessages()
    {
        using var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Api:MaxMessageCharacters", "256");
        });

        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/chat",
            new
            {
                message = new string('x', 257)
            });

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
    }

    [Fact]
    public async Task Chat_IsRateLimited()
    {
        using var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Api:RequestsPerMinute", "1");
        });

        using var client = factory.CreateClient();

        using var first = await client.PostAsJsonAsync(
            "/api/chat",
            new { message = "first" });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, first.StatusCode);

        using var second = await client.PostAsJsonAsync(
            "/api/chat",
            new { message = "second" });

        Assert.Equal(HttpStatusCode.TooManyRequests, second.StatusCode);
    }

    [Fact]
    public async Task ConversationCapacity_IsBounded()
    {
        using var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Conversations:MaxConversations", "1");
        });

        using var client = factory.CreateClient();

        using var first = await client.PostAsJsonAsync(
            "/api/chat",
            new
            {
                conversationId = "conversation-a",
                message = "first"
            });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, first.StatusCode);

        using var second = await client.PostAsJsonAsync(
            "/api/chat",
            new
            {
                conversationId = "conversation-b",
                message = "second"
            });

        Assert.Equal(HttpStatusCode.TooManyRequests, second.StatusCode);
    }
}
