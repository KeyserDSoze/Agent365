using System.Collections.Concurrent;
using Agent365.GoldenAgent.Configuration;
using Microsoft.Extensions.Options;

namespace Agent365.GoldenAgent.Telemetry;

public sealed class RunEvidenceStore
{
    private readonly RunEvidenceOptions _options;
    private readonly ConcurrentQueue<RunEvidenceRecord> _records = new();

    public RunEvidenceStore(IOptions<RunEvidenceOptions> options)
    {
        _options = options.Value;
    }

    public int Count => _records.Count;

    public void Add(RunEvidenceRecord record)
    {
        if (!_options.Enabled)
        {
            return;
        }

        _records.Enqueue(record);

        while (_records.Count > _options.Capacity &&
               _records.TryDequeue(out _))
        {
        }
    }

    public IReadOnlyList<RunEvidenceRecord> Get(
        int limit = 50,
        string? conversationId = null)
    {
        var bounded = Math.Clamp(limit, 1, Math.Min(_options.Capacity, 500));

        IEnumerable<RunEvidenceRecord> query = _records.Reverse();

        if (!string.IsNullOrWhiteSpace(conversationId))
        {
            query = query.Where(x =>
                string.Equals(
                    x.ConversationId,
                    conversationId,
                    StringComparison.Ordinal));
        }

        return query.Take(bounded).ToArray();
    }

    public RunEvidenceSummary GetSummary()
    {
        var records = _records.ToArray();

        if (records.Length == 0)
        {
            return new RunEvidenceSummary(
                BufferedRuns: 0,
                SuccessfulRuns: 0,
                FailedRuns: 0,
                AverageDurationMs: 0,
                ToolInvocations: 0,
                ToolAllowed: 0,
                ToolDenied: 0);
        }

        return new RunEvidenceSummary(
            BufferedRuns: records.Length,
            SuccessfulRuns: records.Count(x => x.Status == "completed"),
            FailedRuns: records.Count(x => x.Status == "failed"),
            AverageDurationMs: Math.Round(records.Average(x => x.DurationMs), 2),
            ToolInvocations: records.Sum(x => x.ToolInvocations),
            ToolAllowed: records.Sum(x => x.ToolAllowed),
            ToolDenied: records.Sum(x => x.ToolDenied));
    }
}
