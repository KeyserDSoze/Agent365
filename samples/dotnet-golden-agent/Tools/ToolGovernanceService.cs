using System.Collections.Concurrent;
using System.Diagnostics;
using Agent365.GoldenAgent.Configuration;
using Microsoft.Extensions.Options;

namespace Agent365.GoldenAgent.Tools;

public sealed class ToolGovernanceService
{
    public const string PolicyLookupTool = "LookupPolicy";
    public const string DraftChangeRequestTool = "CreateDraftChangeRequest";

    private readonly ToolGovernanceOptions _options;
    private readonly ToolInvocationContext _context;
    private readonly ConcurrentDictionary<string, bool> _runtimeEnabled = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ToolApproval> _approvals = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentQueue<ToolAuditRecord> _audit = new();

    public ToolGovernanceService(
        IOptions<ToolGovernanceOptions> options,
        ToolInvocationContext context)
    {
        _options = options.Value;
        _context = context;
    }

    public int AuditCount => _audit.Count;
    public int PendingApprovalCount
    {
        get
        {
            RemoveExpiredApprovals();
            return _approvals.Count;
        }
    }

    public IReadOnlyList<ToolDescriptor> GetCatalog() =>
    [
        BuildDescriptor(
            PolicyLookupTool,
            "Read a mock internal governance policy.",
            "read",
            "Low",
            _options.EnablePolicyLookup,
            requiresApproval: false,
            externalSideEffect: false,
            reversible: true),

        BuildDescriptor(
            DraftChangeRequestTool,
            "Create a mock draft change request without writing to an external system.",
            "write-draft",
            "Medium",
            _options.EnableDraftChangeRequest,
            _options.RequireApprovalForDraftChangeRequest,
            externalSideEffect: false,
            reversible: true)
    ];

    public ToolDescriptor? GetTool(string name) =>
        GetCatalog().FirstOrDefault(
            x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

    public ToolDescriptor? SetEnabled(string name, bool enabled)
    {
        if (!_options.AllowRuntimePolicyChanges)
        {
            return null;
        }

        if (GetTool(name) is null)
        {
            return null;
        }

        _runtimeEnabled[name] = enabled;
        return GetTool(name);
    }

    public ToolApproval? CreateApproval(
        string toolName,
        string conversationId,
        string reason)
    {
        RemoveExpiredApprovals();

        var tool = GetTool(toolName);
        if (tool is null || !tool.RequiresApproval)
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow;
        var approval = new ToolApproval(
            Guid.NewGuid().ToString("n"),
            tool.Name,
            conversationId,
            string.IsNullOrWhiteSpace(reason) ? "Approved for this conversation." : reason.Trim(),
            now,
            now.AddMinutes(_options.ApprovalTtlMinutes));

        _approvals[approval.Id] = approval;
        return approval;
    }

    public ToolRunStats GetRunStats(string runId)
    {
        var records = _audit
            .Where(x => string.Equals(x.RunId, runId, StringComparison.Ordinal))
            .ToArray();

        return new ToolRunStats(
            Total: records.Length,
            Allowed: records.Count(x => x.Decision == "allowed"),
            Denied: records.Count(x => x.Decision == "denied"));
    }

    public IReadOnlyList<ToolAuditRecord> GetAudit(
        int limit = 50,
        string? runId = null,
        string? conversationId = null)
    {
        var bounded = Math.Clamp(limit, 1, Math.Min(_options.AuditCapacity, 500));
        IEnumerable<ToolAuditRecord> query = _audit.Reverse();

        if (!string.IsNullOrWhiteSpace(runId))
        {
            query = query.Where(x =>
                string.Equals(x.RunId, runId, StringComparison.Ordinal));
        }

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

    public ToolExecutionResult LookupPolicy(string policyCode)
    {
        return Execute(
            PolicyLookupTool,
            approvalId: null,
            () =>
            {
                var normalized = policyCode.Trim().ToUpperInvariant();
                var summary = normalized switch
                {
                    "AGENT-IDENTITY" =>
                        "Every production agent must have an explicit owner, a documented identity model, least-privilege permissions, and a tested revocation path.",
                    "TOOL-GOVERNANCE" =>
                        "Every write-capable or externally hosted tool must have an owner, risk tier, approved use case, logging requirement, and revocation path.",
                    "DATA-GOVERNANCE" =>
                        "Every production agent must document its data sources, classifications, allowed operations, applicable controls, and audit evidence.",
                    _ =>
                        "No mock policy was found for that code. Available examples: AGENT-IDENTITY, TOOL-GOVERNANCE, DATA-GOVERNANCE."
                };

                return new
                {
                    policyCode = normalized,
                    summary
                };
            });
    }

    public ToolExecutionResult CreateDraftChangeRequest(
        string title,
        string rationale,
        string? approvalId)
    {
        return Execute(
            DraftChangeRequestTool,
            approvalId,
            () =>
            {
                var seed = $"{title}|{rationale}".GetHashCode(StringComparison.Ordinal);
                var id = $"DRAFT-CR-{Math.Abs(seed % 100000):D5}";

                return new DraftChangeRequest(
                    id,
                    "draft",
                    title,
                    rationale,
                    ExternalSideEffect: false);
            });
    }

    private ToolExecutionResult Execute(
        string toolName,
        string? approvalId,
        Func<object?> action)
    {
        RemoveExpiredApprovals();

        var started = Stopwatch.GetTimestamp();
        var tool = GetTool(toolName);

        if (tool is null)
        {
            return RecordDenied(
                toolName,
                "unknown",
                "Unknown",
                approvalId,
                "Tool is not registered.",
                started);
        }

        if (!tool.Enabled)
        {
            return RecordDenied(
                tool.Name,
                tool.Operation,
                tool.RiskTier,
                approvalId,
                "Tool is blocked by policy.",
                started,
                tool.ExternalSideEffect);
        }

        if (tool.RequiresApproval &&
            !ConsumeApproval(tool.Name, _context.ConversationId, approvalId))
        {
            return RecordDenied(
                tool.Name,
                tool.Operation,
                tool.RiskTier,
                approvalId,
                "A valid one-time approval is required for this tool and conversation.",
                started,
                tool.ExternalSideEffect);
        }

        try
        {
            var result = action();
            var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            AddAudit(new ToolAuditRecord(
                DateTimeOffset.UtcNow,
                _context.RunId,
                _context.ConversationId,
                _context.TraceId,
                tool.Name,
                tool.Operation,
                tool.RiskTier,
                "allowed",
                Success: true,
                tool.ExternalSideEffect,
                Reason: null,
                approvalId,
                elapsed));

            return new ToolExecutionResult(
                Allowed: true,
                Status: "completed",
                Result: result,
                Reason: null,
                tool.ExternalSideEffect);
        }
        catch (Exception ex)
        {
            var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            AddAudit(new ToolAuditRecord(
                DateTimeOffset.UtcNow,
                _context.RunId,
                _context.ConversationId,
                _context.TraceId,
                tool.Name,
                tool.Operation,
                tool.RiskTier,
                "allowed",
                Success: false,
                tool.ExternalSideEffect,
                ex.Message,
                approvalId,
                elapsed));

            return new ToolExecutionResult(
                Allowed: true,
                Status: "failed",
                Result: null,
                Reason: ex.Message,
                tool.ExternalSideEffect);
        }
    }

    private ToolExecutionResult RecordDenied(
        string toolName,
        string operation,
        string riskTier,
        string? approvalId,
        string reason,
        long started,
        bool externalSideEffect = false)
    {
        var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

        AddAudit(new ToolAuditRecord(
            DateTimeOffset.UtcNow,
            _context.ConversationId,
            toolName,
            operation,
            riskTier,
            "denied",
            Success: false,
            externalSideEffect,
            reason,
            approvalId,
            elapsed));

        return new ToolExecutionResult(
            Allowed: false,
            Status: "denied",
            Result: null,
            Reason: reason,
            externalSideEffect);
    }

    private ToolDescriptor BuildDescriptor(
        string name,
        string description,
        string operation,
        string riskTier,
        bool configuredEnabled,
        bool requiresApproval,
        bool externalSideEffect,
        bool reversible)
    {
        var enabled = _runtimeEnabled.TryGetValue(name, out var runtimeValue)
            ? runtimeValue
            : configuredEnabled;

        return new ToolDescriptor(
            name,
            description,
            operation,
            riskTier,
            enabled,
            requiresApproval,
            externalSideEffect,
            reversible,
            Owner: "AGIC Agent 365 Lab",
            Source: "BuiltIn");
    }

    private bool ConsumeApproval(
        string toolName,
        string conversationId,
        string? approvalId)
    {
        if (string.IsNullOrWhiteSpace(approvalId))
        {
            return false;
        }

        if (!_approvals.TryRemove(approvalId, out var approval))
        {
            return false;
        }

        return approval.ExpiresAt >= DateTimeOffset.UtcNow &&
               string.Equals(approval.ToolName, toolName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(approval.ConversationId, conversationId, StringComparison.Ordinal);
    }

    private void RemoveExpiredApprovals()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var item in _approvals)
        {
            if (item.Value.ExpiresAt < now)
            {
                _approvals.TryRemove(item.Key, out _);
            }
        }
    }

    private void AddAudit(ToolAuditRecord record)
    {
        _audit.Enqueue(record);

        while (_audit.Count > _options.AuditCapacity &&
               _audit.TryDequeue(out _))
        {
        }
    }
}
