using System.Threading.RateLimiting;
using Agent365.GoldenAgent.Configuration;
using Agent365.GoldenAgent.Runtime;
using Agent365.GoldenAgent.Security;
using Agent365.GoldenAgent.Telemetry;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AgentRuntimeOptions>(
    builder.Configuration.GetSection(AgentRuntimeOptions.SectionName));
builder.Services.Configure<Agent365ObservabilityOptions>(
    builder.Configuration.GetSection(Agent365ObservabilityOptions.SectionName));

builder.Services
    .AddOptions<ApiOptions>()
    .Bind(builder.Configuration.GetSection(ApiOptions.SectionName))
    .Validate(
        options => options.RequestsPerMinute > 0,
        "Api:RequestsPerMinute must be greater than zero.")
    .Validate(
        options => options.MaxMessageCharacters is >= 256 and <= 100_000,
        "Api:MaxMessageCharacters must be between 256 and 100000.")
    .Validate(
        options => options.MaxConversationIdCharacters is >= 16 and <= 512,
        "Api:MaxConversationIdCharacters must be between 16 and 512.")
    .Validate(
        options => !options.RequireApiKey || !string.IsNullOrWhiteSpace(options.ApiKey),
        "Api:ApiKey is required when Api:RequireApiKey is true.")
    .ValidateOnStart();

builder.Services
    .AddOptions<ConversationStoreOptions>()
    .Bind(builder.Configuration.GetSection(ConversationStoreOptions.SectionName))
    .Validate(
        options => options.MaxConversations is >= 1 and <= 10_000,
        "Conversations:MaxConversations must be between 1 and 10000.")
    .Validate(
        options => options.IdleTimeoutMinutes is >= 1 and <= 1440,
        "Conversations:IdleTimeoutMinutes must be between 1 and 1440.")
    .ValidateOnStart();

var configuredApi = builder.Configuration
    .GetSection(ApiOptions.SectionName)
    .Get<ApiOptions>() ?? new ApiOptions();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                error = "rate_limit_exceeded",
                retryAfterSeconds = 60
            },
            cancellationToken);
    };

    options.AddFixedWindowLimiter("chat", limiter =>
    {
        limiter.PermitLimit = Math.Max(1, configuredApi.RequestsPerMinute);
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiter.AutoReplenishment = true;
    });
});

builder.Services.AddHttpClient(
    "provider-readiness",
    client => client.Timeout = TimeSpan.FromSeconds(3));

builder.Services.AddSingleton<AgentRuntime>();
builder.Services.AddSingleton<ConversationStore>();
builder.ConfigureAgent365Observability();

var app = builder.Build();

var apiOptions = app.Services.GetRequiredService<IOptions<ApiOptions>>().Value;

app.Use(async (context, next) =>
{
    var protectedApi =
        context.Request.Path.StartsWithSegments("/api/chat") ||
        context.Request.Path.StartsWithSegments("/api/conversations") ||
        context.Request.Path.StartsWithSegments("/api/diagnostics");

    if (protectedApi && !ApiKeyGuard.IsAuthorized(context, apiOptions))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "unauthorized",
            header = ApiKeyGuard.HeaderName
        });
        return;
    }

    await next();
});

app.UseRateLimiter();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "agent365-golden-agent",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/ready", async (
    AgentRuntime runtime,
    CancellationToken cancellationToken) =>
{
    var readiness = await runtime.CheckReadinessAsync(cancellationToken);

    return Results.Json(
        readiness,
        statusCode: readiness.Ready
            ? StatusCodes.Status200OK
            : StatusCodes.Status503ServiceUnavailable);
});

app.MapGet("/api/config", (
    IConfiguration configuration,
    IOptions<ApiOptions> api,
    IOptions<ConversationStoreOptions> conversations) =>
{
    var agent = configuration.GetSection(AgentRuntimeOptions.SectionName)
        .Get<AgentRuntimeOptions>() ?? new();
    var observability = configuration.GetSection(Agent365ObservabilityOptions.SectionName)
        .Get<Agent365ObservabilityOptions>() ?? new();

    return Results.Ok(new
    {
        agent = new
        {
            agent.Name,
            agent.Provider,
            agent.Model,
            foundryLocalEndpoint = agent.FoundryLocalEndpoint,
            azureOpenAIEndpointConfigured = !string.IsNullOrWhiteSpace(agent.AzureOpenAIEndpoint)
        },
        observability = new
        {
            observability.ExportToConsole,
            observability.ExportToAgent365,
            tenantConfigured = !string.IsNullOrWhiteSpace(observability.TenantId),
            agentIdConfigured = !string.IsNullOrWhiteSpace(observability.AgentId)
        },
        api = new
        {
            api.Value.RequireApiKey,
            api.Value.RequestsPerMinute,
            api.Value.MaxMessageCharacters,
            api.Value.MaxConversationIdCharacters
        },
        conversations = new
        {
            conversations.Value.MaxConversations,
            conversations.Value.IdleTimeoutMinutes
        }
    });
});

app.MapGet("/api/diagnostics", async (
    AgentRuntime runtime,
    ConversationStore conversations,
    CancellationToken cancellationToken) =>
{
    var removedExpired = conversations.RemoveExpired();
    var readiness = await runtime.CheckReadinessAsync(cancellationToken);

    return Results.Ok(new
    {
        utc = DateTimeOffset.UtcNow,
        readiness,
        conversations = new
        {
            active = conversations.Count,
            removedExpired
        }
    });
});

app.MapPost("/api/chat", async (
    ChatRequest request,
    AgentRuntime runtime,
    ConversationStore conversations,
    IOptions<Agent365ObservabilityOptions> observabilityOptions,
    IOptions<ApiOptions> api,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
    {
        return Results.BadRequest(new { error = "message is required" });
    }

    if (request.Message.Length > api.Value.MaxMessageCharacters)
    {
        return Results.Json(
            new
            {
                error = "message_too_large",
                maxCharacters = api.Value.MaxMessageCharacters
            },
            statusCode: StatusCodes.Status413PayloadTooLarge);
    }

    var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
        ? Guid.NewGuid().ToString("n")
        : request.ConversationId.Trim();

    if (conversationId.Length > api.Value.MaxConversationIdCharacters)
    {
        return Results.BadRequest(new
        {
            error = "conversation_id_too_large",
            maxCharacters = api.Value.MaxConversationIdCharacters
        });
    }

    var obs = observabilityOptions.Value;
    using var baggageScope = Agent365BaggageScope.Create(obs, conversationId);

    try
    {
        var response = await conversations.RunAsync(
            conversationId,
            request.Message,
            runtime,
            cancellationToken);

        return Results.Ok(new ChatResponse(
            conversationId,
            response,
            DateTimeOffset.UtcNow));
    }
    catch (ConversationCapacityException ex)
    {
        return Results.Json(
            new
            {
                error = "conversation_capacity_reached",
                detail = ex.Message
            },
            statusCode: StatusCodes.Status429TooManyRequests);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Problem(
            title: "Agent configuration is incomplete",
            detail: ex.Message,
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
})
.RequireRateLimiting("chat");

app.MapDelete("/api/conversations/{conversationId}", (
    string conversationId,
    ConversationStore conversations) =>
{
    return conversations.Remove(conversationId)
        ? Results.NoContent()
        : Results.NotFound();
});

app.Run();

public sealed record ChatRequest(string Message, string? ConversationId);
public sealed record ChatResponse(string ConversationId, string Output, DateTimeOffset Timestamp);

public partial class Program;
