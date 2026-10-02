using System.Collections.Concurrent;
using Agent365.GoldenAgent.Configuration;
using Microsoft.Extensions.Options;

namespace Agent365.GoldenAgent.Evidence;

public sealed class RunEvidenceService
{
    private readonly EvidenceOptions _options;
    private readonly ConcurrentQueue<RunEvidenceRecord> _runs = new();

    private long _totalRuns;
    private long _successfulRuns;
    private long _failedRuns;
    private long _toolInvocations;
    private long _toolDenied;
    private long _durationMicroseconds;

    public RunEvidenceService(IOptions<EvidenceOptions> options)
    {
        _options = options.Value;
    }

    public void Record(RunEvidenceRecord record)
    {
        _runs.Enqueue(record);

        while (_runs.Count > _options.Capacity &&
               _runs.TryDequeue(out _))
        {
        }

        Interlocked.Increment(ref _totalRuns);

        if (string.Equals(record.Status, "success", StringComparison.OrdinalIgnoreCase))
        {
            Interlocked.Increment(ref _successfulRuns);
        }
        else
        {
            Interlocked.Increment(ref _failedRuns);
        }

        Interlocked.Add(ref _toolInvocations, record.ToolInvocations);
        Interlocked.Add(ref _toolDenied, record.ToolDenied);
        Interlocked.Add(
            ref _durationMicroseconds,
            (long)Math.Round(record.DurationMs * 1000d));
    }

    public IReadOnlyList<RunEvidenceRecord> GetRuns(int limit = 50)
    {
        var bounded = Math.Clamp(limit, 1, Math.Min(_options.Capacity, 500));

        return _runs
            .Reverse()
            .Take(bounded)
            .ToArray();
    }

    public RunEvidenceSummary GetSummary()
    {
        var total = Interlocked.Read(ref _totalRuns);
        var success = Interlocked.Read(ref _successfulRuns);
        var failed = Interlocked.Read(ref _failedRuns);
        var durationMicros = Interlocked.Read(ref _durationMicroseconds);

        return new RunEvidenceSummary(
            StoredRuns: _runs.Count,
            TotalRuns: total,
            SuccessfulRuns: success,
            FailedRuns: failed,
            ToolInvocations: Interlocked.Read(ref _toolInvocations),
            ToolDenied: Interlocked.Read(ref _toolDenied),
            AverageDurationMs: total == 0
                ? 0
                : Math.Round((durationMicros / 1000d) / total, 2),
            SuccessRate: total == 0
                ? 0
                : Math.Round(success * 100d / total, 2));
    }

    public void Clear()
    {
        while (_runs.TryDequeue(out _))
        {
        }

        Interlocked.Exchange(ref _totalRuns, 0);
        Interlocked.Exchange(ref _successfulRuns, 0);
        Interlocked.Exchange(ref _failedRuns, 0);
        Interlocked.Exchange(ref _toolInvocations, 0);
        Interlocked.Exchange(ref _toolDenied, 0);
        Interlocked.Exchange(ref _durationMicroseconds, 0);
    }
}
