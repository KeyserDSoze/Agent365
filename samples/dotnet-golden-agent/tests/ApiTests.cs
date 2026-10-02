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
    public async Task ToolRegistry_IsAvailable()
    {
        using var client = _factory.CreateClient();

        var json = await client.GetStringAsync("/api/tools");

        Assert.True(json.Contains("LookupPolicy", StringComparison.Ordinal));
        Assert.True(json.Contains("CreateDraftChangeRequest", StringComparison.Ordinal));
        Assert.True(json.Contains("riskTier", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ToolRegistry_IsProtected_WhenApiKeyIsRequired()
    {
        using var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Api:RequireApiKey", "true");
            builder.UseSetting("Api:ApiKey", "unit-test-secret");
        });

        using var client = factory.CreateClient();

        using var unauthorized = await client.GetAsync("/api/tools");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        client.DefaultRequestHeaders.Add("X-Api-Key", "unit-test-secret");

        using var authorized = await client.GetAsync("/api/tools");
        Assert.Equal(HttpStatusCode.OK, authorized.StatusCode);
    }

    [Fact]
    public async Task RuntimeToolState_CanBeChanged()
    {
        using var client = _factory.CreateClient();

        using var blocked = await client.PutAsJsonAsync(
            "/api/tools/LookupPolicy/state",
            new { enabled = false });

        Assert.Equal(HttpStatusCode.OK, blocked.StatusCode);

        var json = await blocked.Content.ReadAsStringAsync();
        Assert.True(json.Contains("\"enabled\":false", StringComparison.OrdinalIgnoreCase));

        using var enabled = await client.PutAsJsonAsync(
            "/api/tools/LookupPolicy/state",
            new { enabled = true });

        Assert.Equal(HttpStatusCode.OK, enabled.StatusCode);
    }

    [Fact]
    public async Task Evidence_IsPrivacySafeAndRecordsFailedRuns()
    {
        using var client = _factory.CreateClient();

        const string privateMarker = "PRIVATE-CONTENT-MUST-NOT-BE-STORED";

        using var chat = await client.PostAsJsonAsync(
            "/api/chat",
            new
            {
                conversationId = "evidence-conversation",
                message = privateMarker
            });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, chat.StatusCode);

        var json = await client.GetStringAsync(
            "/api/evidence/runs?conversationId=evidence-conversation");

        Assert.True(json.Contains("\"capturesContent\":false", StringComparison.OrdinalIgnoreCase));
        Assert.True(json.Contains("\"status\":\"failed\"", StringComparison.OrdinalIgnoreCase));
        Assert.True(json.Contains("\"runId\"", StringComparison.OrdinalIgnoreCase));
        Assert.True(json.Contains("\"traceId\"", StringComparison.OrdinalIgnoreCase));
        Assert.False(json.Contains(privateMarker, StringComparison.Ordinal));
    }

    [Fact]
    public async Task EvidenceSummary_IsAvailable()
    {
        using var client = _factory.CreateClient();

        var json = await client.GetStringAsync("/api/evidence/summary");

        Assert.True(json.Contains("\"capturesContent\":false", StringComparison.OrdinalIgnoreCase));
        Assert.True(json.Contains("\"bufferedRuns\"", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Evidence_IsProtected_WhenApiKeyIsRequired()
    {
        using var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Api:RequireApiKey", "true");
            builder.UseSetting("Api:ApiKey", "unit-test-secret");
        });

        using var client = factory.CreateClient();

        using var unauthorized = await client.GetAsync("/api/evidence/runs");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        client.DefaultRequestHeaders.Add("X-Api-Key", "unit-test-secret");

        using var authorized = await client.GetAsync("/api/evidence/runs");
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
