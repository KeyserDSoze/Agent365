using Agent365.GoldenAgent.Configuration;
using Agent365.GoldenAgent.Reliability;
using Agent365.GoldenAgent.Telemetry;
using Agent365.GoldenAgent.Tools;
using Microsoft.Extensions.Options;
using Xunit;

namespace Agent365.GoldenAgent.Tests;

public sealed class ReliabilityAssessmentTests
{
    [Fact]
    public void Assessment_IsInsufficient_WhenMinimumSampleIsNotReached()
    {
        var fixture = CreateFixture(minimumRuns: 5);

        fixture.Store.Add(Run("run-1", "completed", 1000));

        var result = fixture.Service.Assess();

        Assert.Equal("insufficient-data", result.Status);
        Assert.Null(result.Score);
        Assert.Single(result.Findings);
        Assert.Equal("minimum-sample-not-reached", result.Findings[0].Code);
    }

    [Fact]
    public void Assessment_IsDegraded_WhenWarningThresholdIsCrossed()
    {
        var fixture = CreateFixture(
            minimumRuns: 4,
            failureWarning: 0.20,
            failureCritical: 0.50,
            latencyWarningMs: 10000,
            latencyCriticalMs: 20000);

        fixture.Store.Add(Run("run-1", "completed", 1000));
        fixture.Store.Add(Run("run-2", "completed", 1000));
        fixture.Store.Add(Run("run-3", "completed", 1000));
        fixture.Store.Add(Run("run-4", "failed", 1000, errorType: "TestFailure"));

        var result = fixture.Service.Assess();

        Assert.Equal("degraded", result.Status);
        Assert.Equal(85, result.Score);
        Assert.Equal(0.25, result.FailureRate);
        Assert.Contains(result.Findings, x =>
            x.Code == "failure-rate" &&
            x.Severity == "warning");
    }

    [Fact]
    public void Assessment_IsCritical_ForFailureRateAndConsecutiveFailures()
    {
        var fixture = CreateFixture(
            minimumRuns: 5,
            failureWarning: 0.10,
            failureCritical: 0.25,
            consecutiveFailuresCritical: 3);

        fixture.Store.Add(Run("run-old-success", "completed", 1000));
        fixture.Store.Add(Run("run-old-failure", "failed", 1000, errorType: "ProviderError"));
        fixture.Store.Add(Run("run-new-1", "failed", 1000, errorType: "ProviderError"));
        fixture.Store.Add(Run("run-new-2", "failed", 1000, errorType: "ProviderError"));
        fixture.Store.Add(Run("run-new-3", "failed", 1000, errorType: "ProviderError"));

        var result = fixture.Service.Assess();

        Assert.Equal("critical", result.Status);
        Assert.Equal(40, result.Score);
        Assert.Equal(4, result.ConsecutiveFailures);
        Assert.Equal(4, result.ErrorTypes["ProviderError"]);
        Assert.Contains(result.Findings, x =>
            x.Code == "failure-rate" &&
            x.Severity == "critical");
        Assert.Contains(result.Findings, x =>
            x.Code == "consecutive-failures" &&
            x.Severity == "critical");
    }

    [Fact]
    public void IncidentSnapshot_ContainsOnlySuspectRunsAndCorrelatedToolDecisions()
    {
        var fixture = CreateFixture(
            minimumRuns: 2,
            failureWarning: 0.10,
            failureCritical: 0.40);

        fixture.Store.Add(Run("run-success", "completed", 900));
        fixture.Store.Add(Run(
            "run-failure",
            "failed",
            900,
            toolInvocations: 1,
            toolDenied: 1,
            errorType: "Failure"));

        using (fixture.Context.Begin(
            "conversation",
            "run-failure",
            "trace-failure"))
        {
            fixture.Tools.SetEnabled(
                ToolGovernanceService.PolicyLookupTool,
                enabled: false);

            fixture.Tools.LookupPolicy("AGENT-IDENTITY");
        }

        var json = System.Text.Json.JsonSerializer.Serialize(
            fixture.Service.CreateIncidentSnapshot());

        Assert.Contains("agent365-golden-agent-incident/v1", json);
        Assert.Contains("run-failure", json);
        Assert.DoesNotContain("run-success", json);
        Assert.Contains("LookupPolicy", json);
        Assert.DoesNotContain("AGENT-IDENTITY", json);
    }

    private static Fixture CreateFixture(
        int minimumRuns,
        double failureWarning = 0.10,
        double failureCritical = 0.25,
        double latencyWarningMs = 2500,
        double latencyCriticalMs = 5000,
        int consecutiveFailuresCritical = 3)
    {
        var evidenceOptions = Options.Create(new RunEvidenceOptions
        {
            Enabled = true,
            Capacity = 100
        });

        var store = new RunEvidenceStore(evidenceOptions);
        var context = new ToolInvocationContext();
        var tools = new ToolGovernanceService(
            Options.Create(new ToolGovernanceOptions
            {
                AllowRuntimePolicyChanges = true,
                AuditCapacity = 100
            }),
            context);

        var service = new ReliabilityAssessmentService(
            Options.Create(new ReliabilityOptions
            {
                WindowRuns = 50,
                MinimumRuns = minimumRuns,
                FailureRateWarning = failureWarning,
                FailureRateCritical = failureCritical,
                AverageLatencyWarningMs = latencyWarningMs,
                AverageLatencyCriticalMs = latencyCriticalMs,
                ToolDenyRateWarning = 0.15,
                ToolDenyRateCritical = 0.30,
                ConsecutiveFailuresCritical = consecutiveFailuresCritical
            }),
            store,
            tools);

        return new Fixture(store, context, tools, service);
    }

    private static RunEvidenceRecord Run(
        string runId,
        string status,
        double durationMs,
        int toolInvocations = 0,
        int toolDenied = 0,
        string? errorType = null) =>
        new(
            Timestamp: DateTimeOffset.UtcNow,
            RunId: runId,
            ConversationId: "conversation",
            TraceId: $"trace-{runId}",
            Provider: "foundry-local",
            Model: "test-model",
            Status: status,
            DurationMs: durationMs,
            RequestCharacters: 100,
            ResponseCharacters: status == "completed" ? 200 : 0,
            ToolInvocations: toolInvocations,
            ToolAllowed: Math.Max(0, toolInvocations - toolDenied),
            ToolDenied: toolDenied,
            ErrorType: errorType);

    private sealed record Fixture(
        RunEvidenceStore Store,
        ToolInvocationContext Context,
        ToolGovernanceService Tools,
        ReliabilityAssessmentService Service);
}
