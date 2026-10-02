using System.Collections.Concurrent;

namespace Agent365.GovernedMcpServer.Governance;

public sealed class ToolAuditStore
{
    private readonly ToolPolicyOptions _options;
    private readonly ConcurrentQueue<ToolAuditRecord> _records = new();

    public ToolAuditStore(ToolPolicyOptions options)
    {
        _options = options;
    }

    public void Add(ToolAuditRecord record)
    {
        _records.Enqueue(record);

        while (_records.Count > _options.AuditCapacity &&
               _records.TryDequeue(out _))
        {
        }
    }

    public IReadOnlyList<ToolAuditRecord> Get(int limit = 50)
    {
        var bounded = Math.Clamp(
            limit,
            1,
            Math.Min(_options.AuditCapacity, 500));

        return _records
            .Reverse()
            .Take(bounded)
            .ToArray();
    }

    public ToolAuditSummary GetSummary()
    {
        var records = _records.ToArray();

        return new ToolAuditSummary(
            BufferedEvents: records.Length,
            Allowed: records.Count(x => x.Decision == "allowed"),
            Denied: records.Count(x => x.Decision == "denied"),
            Failed: records.Count(x => !x.Success));
    }
}
