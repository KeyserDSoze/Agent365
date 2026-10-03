using System.Threading.RateLimiting;
using Agent365.GoldenAgent.Configuration;
using Agent365.GoldenAgent.Mcp;
using Agent365.GoldenAgent.Runtime;
using Agent365.GoldenAgent.Reliability;
using Agent365.GoldenAgent.Security;
using Agent365.GoldenAgent.Telemetry;
using Agent365.GoldenAgent.Tools;
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

builder.Services
    .AddOptions<RunEvidenceOptions>()
    .Bind(builder.Configuration.GetSection(RunEvidenceOptions.SectionName))
    .Validate(
        options => options.Capacity is >= 10 and <= 10_000,
        "Evidence:Capacity must be between 10 and 10000.")
    .ValidateOnStart();

builder.Services
    .AddOptions<ReliabilityOptions>()
    .Bind(builder.Configuration.GetSection(ReliabilityOptions.SectionName))
    .Validate(
        options => options.WindowRuns is >= 5 and <= 500,
        "Reliability:WindowRuns must be between 5 and 500.")
    .Validate(
        options => options.MinimumRuns is >= 1 and <= 500 &&
                   options.MinimumRuns <= options.WindowRuns,
        "Reliability:MinimumRuns must be between 1 and WindowRuns.")
    .Validate(
        options => options.FailureRateWarning is >= 0 and <= 1 &&
                   options.FailureRateCritical is >= 0 and <= 1 &&
                   options.FailureRateWarning < options.FailureRateCritical,
        "Reliability failure-rate thresholds must be valid ratios and warning must be below critical.")
    .Validate(
        options => options.ToolDenyRateWarning is >= 0 and <= 1 &&
                   options.ToolDenyRateCritical is >= 0 and <= 1 &&
                   options.ToolDenyRateWarning < options.ToolDenyRateCritical,
        "Reliability tool-deny thresholds must be valid ratios and warning must be below critical.")
    .Validate(
        options => options.AverageLatencyWarningMs > 0 &&
                   options.AverageLatencyCriticalMs > options.AverageLatencyWarningMs,
        "Reliability latency warning must be positive and below critical.")
    .Validate(
        options => options.ConsecutiveFailuresCritical >= 1,
        "Reliability:ConsecutiveFailuresCritical must be greater than zero.")
    .ValidateOnStart();

builder.Services
    .AddOptions<McpBridgeOptions>()
    .Bind(builder.Configuration.GetSection(McpBridgeOptions.SectionName))
    .Validate(
        options => options.InitializationTimeoutSeconds is >= 5 and <= 120,
        "Mcp:InitializationTimeoutSeconds must be between 5 and 120.")
    .Validate(
        options => options.ShutdownTimeoutSeconds is >= 1 and <= 60,
        "Mcp:ShutdownTimeoutSeconds must be between 1 and 60.")
    .Validate(
        options => options.AuditCapacity is >= 10 and <= 10_000,
        "Mcp:AuditCapacity must be between 10 and 10000.")
    .Validate(
        options => !options.Enabled ||
                   (!string.IsNullOrWhiteSpace(options.Command) &&
                    !string.IsNullOrWhiteSpace(options.ProjectPath)),
        "Mcp:Command and Mcp:ProjectPath are required when MCP is enabled.")
    .Validate(
        options => !options.RequireApprovalForDraft ||
                   !string.IsNullOrWhiteSpace(options.ApprovalToken),
        "Mcp:ApprovalToken is required when MCP draft approval is enabled.")
    .ValidateOnStart();

builder.Services
    .AddOptions<ToolGovernanceOptions>()
    .Bind(builder.Configuration.GetSection(ToolGovernanceOptions.SectionName))
    .Validate(
        options => options.ApprovalTtlMinutes is >= 1 and <= 60,
        "Tools:ApprovalTtlMinutes must be between 1 and 60.")
    .Validate(
        options => options.AuditCapacity is >= 10 and <= 10_000,
        "Tools:AuditCapacity must be between 10 and 10000.")
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
            cancellationToken: cancellationToken);
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

builder.Services.AddSingleton<ToolInvocationContext>();
builder.Services.AddSingleton<ToolGovernanceService>();
builder.Services.AddSingleton<GovernedMcpToolProvider>();
builder.Services.AddSingleton<RunEvidenceStore>();
builder.Services.AddSingleton<RunEvidenceService>();
builder.Services.AddSingleton<ReliabilityAssessmentService>();
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
        context.Request.Path.StartsWithSegments("/api/diagnostics") ||
        context.Request.Path.StartsWithSegments("/api/tools") ||
        context.Request.Path.StartsWithSegments("/api/tool-audit") ||
        context.Request.Path.StartsWithSegments("/api/tool-approvals") ||
        context.Request.Path.StartsWithSegments("/api/evidence") ||
        context.Request.Path.StartsWithSegments("/api/reliability") ||
        context.Request.Path.StartsWithSegments("/api/incidents") ||
        context.Request.Path.StartsWithSegments("/api/mcp");

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
    GovernedMcpToolProvider mcp,
    IOptions<McpBridgeOptions> mcpOptions,
    CancellationToken cancellationToken) =>
{
    var readiness = await runtime.CheckReadinessAsync(cancellationToken);

    if (readiness.Ready &&
        mcpOptions.Value.Enabled &&
        mcpOptions.Value.Required)
    {
        var mcpStatus = await mcp.GetStatusAsync(
            initialize: true,
            cancellationToken);

        if (!mcpStatus.Connected)
        {
            readiness = readiness with
            {
                Ready = false,
                Detail = $"Model runtime is ready but required MCP bridge is unavailable: {mcpStatus.Error ?? "unknown error"}"
            };
        }
    }

    return Results.Json(
        readiness,
        statusCode: readiness.Ready
            ? StatusCodes.Status200OK
            : StatusCodes.Status503ServiceUnavailable);
});

app.MapGet("/api/config", (
    IConfiguration configuration,
    IOptions<ApiOptions> api,
    IOptions<ConversationStoreOptions> conversations,
    IOptions<ToolGovernanceOptions> tools,
    IOptions<RunEvidenceOptions> evidence,
    IOptions<ReliabilityOptions> reliability,
    IOptions<McpBridgeOptions> mcp) =>
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
        },
        tools = new
        {
            tools.Value.EnablePolicyLookup,
            tools.Value.EnableDraftChangeRequest,
            tools.Value.RequireApprovalForDraftChangeRequest,
            tools.Value.AllowRuntimePolicyChanges,
            tools.Value.ApprovalTtlMinutes,
            tools.Value.AuditCapacity
        },
        evidence = new
        {
            evidence.Value.Enabled,
            evidence.Value.Capacity,
            capturesContent = false
        },
        reliability = new
        {
            reliability.Value.WindowRuns,
            reliability.Value.MinimumRuns,
            reliability.Value.FailureRateWarning,
            reliability.Value.FailureRateCritical,
            reliability.Value.AverageLatencyWarningMs,
            reliability.Value.AverageLatencyCriticalMs,
            reliability.Value.ToolDenyRateWarning,
            reliability.Value.ToolDenyRateCritical,
            reliability.Value.ConsecutiveFailuresCritical
        },
        mcp = new
        {
            mcp.Value.Enabled,
            mcp.Value.Required,
            mcp.Value.ReplaceBuiltInTools,
            mcp.Value.ExposeAdministrativeTools,
            commandConfigured = !string.IsNullOrWhiteSpace(mcp.Value.Command),
            projectConfigured = !string.IsNullOrWhiteSpace(mcp.Value.ProjectPath),
            mcp.Value.RequireApprovalForDraft,
            approvalTokenConfigured = !string.IsNullOrWhiteSpace(mcp.Value.ApprovalToken),
            mcp.Value.EnablePolicyLookup,
            mcp.Value.EnableDraftChangeRequest,
            mcp.Value.AuditCapacity
        }
    });
});

app.MapGet("/api/diagnostics", async (
    AgentRuntime runtime,
    ConversationStore conversations,
    ToolGovernanceService tools,
    RunEvidenceStore evidence,
    ReliabilityAssessmentService reliability,
    GovernedMcpToolProvider mcp,
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
        },
        tools = new
        {
            registered = tools.GetCatalog().Count,
            auditEvents = tools.AuditCount,
            pendingApprovals = tools.PendingApprovalCount
        },
        evidence = evidence.GetSummary(),
        reliability = reliability.Assess(),
        mcp = await mcp.GetStatusAsync(
            initialize: false,
            cancellationToken)
    });
});

app.MapGet("/api/mcp/status", async (
    bool? initialize,
    GovernedMcpToolProvider mcp,
    CancellationToken cancellationToken) =>
{
    var status = await mcp.GetStatusAsync(
        initialize ?? false,
        cancellationToken);

    return Results.Ok(status);
});

app.MapGet("/api/tools", (
    ToolGovernanceService tools,
    IOptions<ToolGovernanceOptions> options) =>
{
    return Results.Ok(new
    {
        runtimePolicyChanges = options.Value.AllowRuntimePolicyChanges,
        items = tools.GetCatalog()
    });
});

app.MapPut("/api/tools/{toolName}/state", (
    string toolName,
    ToolStateUpdate update,
    ToolGovernanceService tools,
    IOptions<ToolGovernanceOptions> options) =>
{
    if (!options.Value.AllowRuntimePolicyChanges)
    {
        return Results.Json(
            new { error = "runtime_policy_changes_disabled" },
            statusCode: StatusCodes.Status403Forbidden);
    }

    var updated = tools.SetEnabled(toolName, update.Enabled);

    return updated is null
        ? Results.NotFound(new { error = "tool_not_found", toolName })
        : Results.Ok(updated);
});

app.MapPost("/api/tool-approvals", (
    ToolApprovalRequest request,
    ToolGovernanceService tools) =>
{
    if (string.IsNullOrWhiteSpace(request.ToolName) ||
        string.IsNullOrWhiteSpace(request.ConversationId))
    {
        return Results.BadRequest(new
        {
            error = "toolName and conversationId are required"
        });
    }

    var approval = tools.CreateApproval(
        request.ToolName.Trim(),
        request.ConversationId.Trim(),
        request.Reason);

    return approval is null
        ? Results.BadRequest(new
        {
            error = "tool_not_found_or_approval_not_required",
            request.ToolName
        })
        : Results.Created($"/api/tool-approvals/{approval.Id}", approval);
});

app.MapGet("/api/tool-audit", (
    int? limit,
    string? runId,
    string? conversationId,
    ToolGovernanceService tools) =>
{
    return Results.Ok(new
    {
        items = tools.GetAudit(
            limit ?? 50,
            runId,
            conversationId)
    });
});

app.MapGet("/api/evidence/runs", (
    int? limit,
    string? conversationId,
    RunEvidenceStore evidence) =>
{
    return Results.Ok(new
    {
        capturesContent = false,
        items = evidence.Get(limit ?? 50, conversationId)
    });
});

app.MapGet("/api/evidence/summary", (
    RunEvidenceStore evidence) =>
{
    return Results.Ok(new
    {
        capturesContent = false,
        summary = evidence.GetSummary()
    });
});

app.MapGet("/api/reliability", (
    ReliabilityAssessmentService reliability) =>
{
    return Results.Ok(new
    {
        capturesContent = false,
        assessment = reliability.Assess()
    });
});

app.MapGet("/api/incidents/snapshot", (
    int? limit,
    ReliabilityAssessmentService reliability) =>
{
    return Results.Ok(reliability.CreateIncidentSnapshot(limit ?? 50));
});

app.MapGet("/api/evidence/export", (
    int? limit,
    RunEvidenceStore evidence,
    ToolGovernanceService tools,
    ReliabilityAssessmentService reliability) =>
{
    var bounded = Math.Clamp(limit ?? 200, 1, 500);

    return Results.Ok(new
    {
        schema = "agent365-golden-agent-evidence/v1",
        generatedAt = DateTimeOffset.UtcNow,
        capturesContent = false,
        summary = evidence.GetSummary(),
        reliability = reliability.Assess(),
        runs = evidence.Get(bounded),
        toolAudit = tools.GetAudit(bounded),
        tools = tools.GetCatalog()
    });
});

app.MapPost("/api/chat", async (
    ChatRequest request,
    AgentRuntime runtime,
    ConversationStore conversations,
    RunEvidenceService runEvidence,
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
        var execution = await runEvidence.ExecuteAsync(
            conversationId,
            request.Message,
            (runId, traceId, ct) => conversations.RunAsync(
                conversationId,
                request.Message,
                runId,
                traceId,
                runtime,
                ct),
            cancellationToken);

        return Results.Ok(new ChatResponse(
            conversationId,
            execution.RunId,
            execution.TraceId,
            execution.Output,
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
public sealed record ChatResponse(
    string ConversationId,
    string RunId,
    string TraceId,
    string Output,
    DateTimeOffset Timestamp);

public partial class Program { }
