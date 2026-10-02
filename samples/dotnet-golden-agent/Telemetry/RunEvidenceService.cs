using System.Diagnostics;
using Agent365.GoldenAgent.Configuration;
using Agent365.GoldenAgent.Tools;
using Microsoft.Extensions.Options;

namespace Agent365.GoldenAgent.Telemetry;

public sealed class RunEvidenceService
{
    private readonly AgentRuntimeOptions _agent;
    private readonly RunEvidenceStore _store;
    private readonly ToolGovernanceService _tools;

    public RunEvidenceService(
        IOptions<AgentRuntimeOptions> agent,
        RunEvidenceStore store,
        ToolGovernanceService tools)
    {
        _agent = agent.Value;
        _store = store;
        _tools = tools;
    }

    public async Task<RunExecutionResult> ExecuteAsync(
        string conversationId,
        string message,
        Func<string, string, CancellationToken, Task<string>> execute,
        CancellationToken cancellationToken)
    {
        var runId = Guid.NewGuid().ToString("n");
        var started = Stopwatch.GetTimestamp();

        using var activity = new Activity("invoke_agent")
            .SetIdFormat(ActivityIdFormat.W3C)
            .AddTag("gen_ai.operation.name", "invoke_agent")
            .AddTag("gen_ai.agent.name", _agent.Name)
            .AddTag("gen_ai.conversation.id", conversationId)
            .AddTag("gen_ai.request.model", _agent.Model)
            .AddTag("agent365.run.id", runId)
            .Start();

        var traceId = activity.TraceId.ToString();

        try
        {
            var output = await execute(runId, traceId, cancellationToken);
            var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
            var toolStats = _tools.GetRunStats(runId);

            activity.SetTag("agent365.run.status", "completed");
            activity.SetTag("agent365.tool.invocations", toolStats.Total);
            activity.SetTag("agent365.tool.denied", toolStats.Denied);

            _store.Add(new RunEvidenceRecord(
                DateTimeOffset.UtcNow,
                runId,
                conversationId,
                traceId,
                _agent.Provider,
                _agent.Model,
                Status: "completed",
                elapsed,
                RequestCharacters: message.Length,
                ResponseCharacters: output.Length,
                toolStats.Total,
                toolStats.Allowed,
                toolStats.Denied,
                ErrorType: null));

            return new RunExecutionResult(runId, traceId, output);
        }
        catch (Exception ex)
        {
            var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
            var toolStats = _tools.GetRunStats(runId);

            activity.SetStatus(ActivityStatusCode.Error, ex.GetType().Name);
            activity.SetTag("agent365.run.status", "failed");
            activity.SetTag("error.type", ex.GetType().FullName);
            activity.SetTag("agent365.tool.invocations", toolStats.Total);
            activity.SetTag("agent365.tool.denied", toolStats.Denied);

            _store.Add(new RunEvidenceRecord(
                DateTimeOffset.UtcNow,
                runId,
                conversationId,
                traceId,
                _agent.Provider,
                _agent.Model,
                Status: "failed",
                elapsed,
                RequestCharacters: message.Length,
                ResponseCharacters: 0,
                toolStats.Total,
                toolStats.Allowed,
                toolStats.Denied,
                ErrorType: ex.GetType().Name));

            throw;
        }
    }
}
