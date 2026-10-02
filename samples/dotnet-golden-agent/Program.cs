using Agent365.GoldenAgent.Configuration;
using Agent365.GoldenAgent.Runtime;
using Agent365.GoldenAgent.Telemetry;
using Microsoft.OpenTelemetry;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AgentRuntimeOptions>(
    builder.Configuration.GetSection(AgentRuntimeOptions.SectionName));
builder.Services.Configure<Agent365ObservabilityOptions>(
    builder.Configuration.GetSection(Agent365ObservabilityOptions.SectionName));

builder.Services.AddSingleton<AgentRuntime>();
builder.Services.AddSingleton<ConversationStore>();
builder.ConfigureAgent365Observability();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "agent365-golden-agent",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/config", (IConfiguration configuration) =>
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
            agent.Model,
            endpointConfigured = !string.IsNullOrWhiteSpace(agent.AzureOpenAIEndpoint)
        },
        observability = new
        {
            observability.ExportToConsole,
            observability.ExportToAgent365,
            tenantConfigured = !string.IsNullOrWhiteSpace(observability.TenantId),
            agentIdConfigured = !string.IsNullOrWhiteSpace(observability.AgentId)
        }
    });
});

app.MapPost("/api/chat", async (
    ChatRequest request,
    AgentRuntime runtime,
    ConversationStore conversations,
    IOptions<Agent365ObservabilityOptions> observabilityOptions,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
    {
        return Results.BadRequest(new { error = "message is required" });
    }

    var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
        ? Guid.NewGuid().ToString("n")
        : request.ConversationId.Trim();

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
    catch (InvalidOperationException ex)
    {
        return Results.Problem(
            title: "Agent configuration is incomplete",
            detail: ex.Message,
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

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
