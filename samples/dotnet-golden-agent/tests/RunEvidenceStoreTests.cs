using Agent365.GoldenAgent.Configuration;
using Agent365.GoldenAgent.Telemetry;
using Microsoft.Extensions.Options;
using Xunit;

namespace Agent365.GoldenAgent.Tests;

public sealed class RunEvidenceStoreTests
{
    [Fact]
    public void Store_IsBounded_AndSummaryTracksStatusAndToolCounts()
    {
        var store = new RunEvidenceStore(
            Options.Create(new RunEvidenceOptions
            {
                Enabled = true,
                Capacity = 2
            }));

        store.Add(Create(
            runId: "run-1",
            status: "completed",
            durationMs: 100,
            toolInvocations: 1,
            toolAllowed: 1,
            toolDenied: 0));

        store.Add(Create(
            runId: "run-2",
            status: "failed",
            durationMs: 200,
            toolInvocations: 2,
            toolAllowed: 1,
            toolDenied: 1));

        store.Add(Create(
            runId: "run-3",
            status: "completed",
            durationMs: 300,
            toolInvocations: 3,
            toolAllowed: 2,
            toolDenied: 1));

        var items = store.Get(limit: 10);
        var summary = store.GetSummary();

        Assert.Equal(2, items.Count);
        Assert.DoesNotContain(items, x => x.RunId == "run-1");
        Assert.Equal("run-3", items[0].RunId);
        Assert.Equal("run-2", items[1].RunId);

        Assert.Equal(2, summary.BufferedRuns);
        Assert.Equal(1, summary.SuccessfulRuns);
        Assert.Equal(1, summary.FailedRuns);
        Assert.Equal(250, summary.AverageDurationMs);
        Assert.Equal(5, summary.ToolInvocations);
        Assert.Equal(3, summary.ToolAllowed);
        Assert.Equal(2, summary.ToolDenied);
    }

    [Fact]
    public void DisabledStore_DoesNotRetainEvidence()
    {
        var store = new RunEvidenceStore(
            Options.Create(new RunEvidenceOptions
            {
                Enabled = false,
                Capacity = 100
            }));

        store.Add(Create(
            runId: "run-disabled",
            status: "completed",
            durationMs: 42,
            toolInvocations: 0,
            toolAllowed: 0,
            toolDenied: 0));

        Assert.Empty(store.Get());
        Assert.Equal(0, store.GetSummary().BufferedRuns);
    }

    private static RunEvidenceRecord Create(
        string runId,
        string status,
        double durationMs,
        int toolInvocations,
        int toolAllowed,
        int toolDenied) =>
        new(
            Timestamp: DateTimeOffset.UtcNow,
            RunId: runId,
            ConversationId: "conversation",
            TraceId: "trace",
            Provider: "foundry-local",
            Model: "test-model",
            Status: status,
            DurationMs: durationMs,
            RequestCharacters: 10,
            ResponseCharacters: 20,
            ToolInvocations: toolInvocations,
            ToolAllowed: toolAllowed,
            ToolDenied: toolDenied,
            ErrorType: status == "failed" ? "TestError" : null);
}
