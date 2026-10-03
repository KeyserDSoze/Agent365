using Agent365.GoldenAgent.Configuration;
using Agent365.GoldenAgent.Telemetry;
using Agent365.GoldenAgent.Tools;
using Microsoft.Extensions.Options;

namespace Agent365.GoldenAgent.Reliability;

public sealed class ReliabilityAssessmentService
{
    private readonly ReliabilityOptions _options;
    private readonly RunEvidenceStore _evidence;
    private readonly ToolGovernanceService _tools;

    public ReliabilityAssessmentService(
        IOptions<ReliabilityOptions> options,
        RunEvidenceStore evidence,
        ToolGovernanceService tools)
    {
        _options = options.Value;
        _evidence = evidence;
        _tools = tools;
    }

    public ReliabilityAssessment Assess()
    {
        var runs = _evidence
            .Get(_options.WindowRuns)
            .OrderByDescending(x => x.Timestamp)
            .ToArray();

        if (runs.Length < _options.MinimumRuns)
        {
            return new ReliabilityAssessment(
                GeneratedAt: DateTimeOffset.UtcNow,
                Status: "insufficient-data",
                Score: null,
                EvaluatedRuns: runs.Length,
                FailureRate: Rate(runs.Count(x => x.Status == "failed"), runs.Length),
                AverageLatencyMs: runs.Length == 0 ? 0 : Math.Round(runs.Average(x => x.DurationMs), 2),
                ToolDenyRate: Rate(runs.Sum(x => x.ToolDenied), runs.Sum(x => x.ToolInvocations)),
                ConsecutiveFailures: CountConsecutiveFailures(runs),
                ErrorTypes: ErrorTypeCounts(runs),
                Findings:
                [
                    new ReliabilityFinding(
                        Code: "minimum-sample-not-reached",
                        Severity: "info",
                        Title: "Not enough runs for a reliability decision",
                        Detail: $"Collected {runs.Length} of {_options.MinimumRuns} required runs.",
                        ObservedValue: runs.Length,
                        Threshold: _options.MinimumRuns,
                        Unit: "runs",
                        RecommendedAction: "Generate more representative traffic before using reliability thresholds for operational decisions.",
                        RunIds: runs.Select(x => x.RunId).ToArray())
                ]);
        }

        var failureRate = Rate(
            runs.Count(x => x.Status == "failed"),
            runs.Length);

        var averageLatency = Math.Round(
            runs.Average(x => x.DurationMs),
            2);

        var toolInvocations = runs.Sum(x => x.ToolInvocations);
        var toolDenied = runs.Sum(x => x.ToolDenied);
        var toolDenyRate = Rate(toolDenied, toolInvocations);
        var consecutiveFailures = CountConsecutiveFailures(runs);

        var findings = new List<ReliabilityFinding>();

        AddRateFinding(
            findings,
            code: "failure-rate",
            title: "Run failure rate",
            observed: failureRate,
            warning: _options.FailureRateWarning,
            critical: _options.FailureRateCritical,
            unit: "ratio",
            action: "Inspect the most recent failed runs, provider readiness and error taxonomy.",
            runIds: runs.Where(x => x.Status == "failed").Select(x => x.RunId));

        AddRateFinding(
            findings,
            code: "average-latency",
            title: "Average run latency",
            observed: averageLatency,
            warning: _options.AverageLatencyWarningMs,
            critical: _options.AverageLatencyCriticalMs,
            unit: "ms",
            action: "Check provider latency, model choice, tool duration and local resource saturation.",
            runIds: runs
                .OrderByDescending(x => x.DurationMs)
                .Take(5)
                .Select(x => x.RunId));

        if (toolInvocations > 0)
        {
            AddRateFinding(
                findings,
                code: "tool-deny-rate",
                title: "Tool deny rate",
                observed: toolDenyRate,
                warning: _options.ToolDenyRateWarning,
                critical: _options.ToolDenyRateCritical,
                unit: "ratio",
                action: "Review blocked capabilities, approval requirements and whether the agent is attempting unsupported actions.",
                runIds: runs
                    .Where(x => x.ToolDenied > 0)
                    .Select(x => x.RunId));
        }

        if (consecutiveFailures >= _options.ConsecutiveFailuresCritical)
        {
            findings.Add(new ReliabilityFinding(
                Code: "consecutive-failures",
                Severity: "critical",
                Title: "Consecutive run failures",
                Detail: $"{consecutiveFailures} most recent runs failed consecutively.",
                ObservedValue: consecutiveFailures,
                Threshold: _options.ConsecutiveFailuresCritical,
                Unit: "runs",
                RecommendedAction: "Treat as an active incident: verify readiness, provider connectivity, configuration and recent changes before retrying.",
                RunIds: runs
                    .Take(consecutiveFailures)
                    .Select(x => x.RunId)
                    .ToArray()));
        }

        var critical = findings.Count(x => x.Severity == "critical");
        var warning = findings.Count(x => x.Severity == "warning");
        var score = Math.Max(0, 100 - (critical * 30) - (warning * 15));
        var status = critical > 0
            ? "critical"
            : warning > 0
                ? "degraded"
                : "healthy";

        return new ReliabilityAssessment(
            GeneratedAt: DateTimeOffset.UtcNow,
            Status: status,
            Score: score,
            EvaluatedRuns: runs.Length,
            FailureRate: failureRate,
            AverageLatencyMs: averageLatency,
            ToolDenyRate: toolDenyRate,
            ConsecutiveFailures: consecutiveFailures,
            ErrorTypes: ErrorTypeCounts(runs),
            Findings: findings);
    }

    public object CreateIncidentSnapshot(int limit = 50)
    {
        var assessment = Assess();
        var bounded = Math.Clamp(limit, 1, 200);
        var suspectRunIds = assessment.Findings
            .SelectMany(x => x.RunIds)
            .Distinct(StringComparer.Ordinal)
            .Take(bounded)
            .ToHashSet(StringComparer.Ordinal);

        var suspectRuns = _evidence
            .Get(Math.Max(_options.WindowRuns, bounded))
            .Where(x => suspectRunIds.Contains(x.RunId))
            .Select(x => (object)x)
            .ToArray();

        var toolDecisions = _tools
            .GetAudit(Math.Max(_options.WindowRuns, bounded))
            .Where(x => suspectRunIds.Contains(x.RunId))
            .Select(x => (object)x)
            .ToArray();

        return new
        {
            schema = "agent365-golden-agent-incident/v1",
            generatedAt = DateTimeOffset.UtcNow,
            capturesContent = false,
            reliability = assessment,
            suspectRuns,
            toolDecisions
        };
    }

    private static double Rate(int numerator, int denominator) =>
        denominator <= 0
            ? 0
            : Math.Round((double)numerator / denominator, 4);

    private static int CountConsecutiveFailures(
        IReadOnlyList<RunEvidenceRecord> runs)
    {
        var count = 0;

        foreach (var run in runs)
        {
            if (run.Status != "failed")
            {
                break;
            }

            count++;
        }

        return count;
    }

    private static IReadOnlyDictionary<string, int> ErrorTypeCounts(
        IEnumerable<RunEvidenceRecord> runs) =>
        runs
            .Where(x => !string.IsNullOrWhiteSpace(x.ErrorType))
            .GroupBy(x => x.ErrorType!, StringComparer.Ordinal)
            .OrderByDescending(x => x.Count())
            .ThenBy(x => x.Key, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.Count(), StringComparer.Ordinal);

    private static void AddRateFinding(
        ICollection<ReliabilityFinding> findings,
        string code,
        string title,
        double observed,
        double warning,
        double critical,
        string unit,
        string action,
        IEnumerable<string> runIds)
    {
        if (observed < warning)
        {
            return;
        }

        var severity = observed >= critical
            ? "critical"
            : "warning";

        var threshold = severity == "critical"
            ? critical
            : warning;

        findings.Add(new ReliabilityFinding(
            Code: code,
            Severity: severity,
            Title: title,
            Detail: $"{title} is {observed:0.####} and crossed the {severity} threshold {threshold:0.####}.",
            ObservedValue: observed,
            Threshold: threshold,
            Unit: unit,
            RecommendedAction: action,
            RunIds: runIds.Distinct(StringComparer.Ordinal).Take(10).ToArray()));
    }
}
