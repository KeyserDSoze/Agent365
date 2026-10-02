using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Agent365.GovernedMcpServer.Governance;

public sealed class ToolGovernanceService
{
    public const string PolicyLookupTool = "lookup_governance_policy";
    public const string DraftChangeRequestTool = "create_draft_change_request";

    private readonly ToolPolicyOptions _options;
    private readonly ToolAuditStore _audit;

    public ToolGovernanceService(
        ToolPolicyOptions options,
        ToolAuditStore audit)
    {
        _options = options;
        _audit = audit;
    }

    public IReadOnlyList<ToolDescriptor> GetCatalog() =>
    [
        new(
            Name: PolicyLookupTool,
            Operation: "read",
            RiskTier: "Low",
            Enabled: _options.EnablePolicyLookup,
            RequiresApproval: false,
            ExternalSideEffect: false,
            Reversible: true,
            Owner: "AGIC Agent 365 Lab"),

        new(
            Name: DraftChangeRequestTool,
            Operation: "write-draft",
            RiskTier: "Medium",
            Enabled: _options.EnableDraftChangeRequest,
            RequiresApproval: _options.RequireApprovalForDraftChangeRequest,
            ExternalSideEffect: false,
            Reversible: true,
            Owner: "AGIC Agent 365 Lab")
    ];

    public string GetManifestJson() =>
        JsonSerializer.Serialize(
            new
            {
                schema = "agic-agent365-mcp-governance/v1",
                server = "AGIC Agent 365 Governed MCP",
                approvalTokenExposed = false,
                tools = GetCatalog()
            },
            JsonOptions);

    public string GetAuditSummaryJson() =>
        JsonSerializer.Serialize(
            new
            {
                capturesArguments = false,
                summary = _audit.GetSummary(),
                recent = _audit.Get(20)
            },
            JsonOptions);

    public string LookupPolicy(string policyCode)
    {
        var result = Execute(
            PolicyLookupTool,
            approvalToken: null,
            action: () =>
            {
                var normalized = policyCode.Trim().ToUpperInvariant();

                return normalized switch
                {
                    "AGENT-IDENTITY" =>
                        "Every production agent must have an explicit owner, a documented identity model, least-privilege permissions, and a tested revocation path.",
                    "TOOL-GOVERNANCE" =>
                        "Every write-capable or externally hosted tool must have an owner, risk tier, approved use case, monitoring requirement, and revocation path.",
                    "DATA-GOVERNANCE" =>
                        "Every production agent must document data sources, classification, allowed operations, controls, and evidence.",
                    _ =>
                        "Unknown mock policy. Available examples: AGENT-IDENTITY, TOOL-GOVERNANCE, DATA-GOVERNANCE."
                };
            });

        return SerializeResult(result);
    }

    public string CreateDraftChangeRequest(
        string title,
        string rationale,
        string? approvalToken)
    {
        var result = Execute(
            DraftChangeRequestTool,
            approvalToken,
            () =>
            {
                var seed = $"{title}|{rationale}".GetHashCode(StringComparison.Ordinal);
                var id = $"MCP-DRAFT-{Math.Abs(seed % 100000):D5}";

                return new
                {
                    id,
                    state = "draft",
                    title,
                    rationale,
                    externalSideEffect = false
                };
            });

        return SerializeResult(result);
    }

    private ToolExecutionResult Execute(
        string toolName,
        string? approvalToken,
        Func<object?> action)
    {
        var tool = GetCatalog().Single(x => x.Name == toolName);
        var started = Stopwatch.GetTimestamp();

        if (!tool.Enabled)
        {
            return Deny(
                tool,
                "Tool is blocked by MCP server policy.",
                started);
        }

        if (tool.RequiresApproval &&
            !IsApprovalValid(approvalToken))
        {
            return Deny(
                tool,
                "A valid operator-provisioned approval token is required.",
                started);
        }

        try
        {
            var output = action();
            var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            _audit.Add(new ToolAuditRecord(
                Timestamp: DateTimeOffset.UtcNow,
                ToolName: tool.Name,
                Operation: tool.Operation,
                RiskTier: tool.RiskTier,
                Decision: "allowed",
                Success: true,
                ExternalSideEffect: tool.ExternalSideEffect,
                Reason: null,
                DurationMs: elapsed));

            return new ToolExecutionResult(
                Allowed: true,
                Status: "completed",
                Result: output,
                Reason: null,
                ExternalSideEffect: tool.ExternalSideEffect);
        }
        catch (Exception ex)
        {
            var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            _audit.Add(new ToolAuditRecord(
                Timestamp: DateTimeOffset.UtcNow,
                ToolName: tool.Name,
                Operation: tool.Operation,
                RiskTier: tool.RiskTier,
                Decision: "allowed",
                Success: false,
                ExternalSideEffect: tool.ExternalSideEffect,
                Reason: ex.GetType().Name,
                DurationMs: elapsed));

            return new ToolExecutionResult(
                Allowed: true,
                Status: "failed",
                Result: null,
                Reason: ex.GetType().Name,
                ExternalSideEffect: tool.ExternalSideEffect);
        }
    }

    private ToolExecutionResult Deny(
        ToolDescriptor tool,
        string reason,
        long started)
    {
        var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

        _audit.Add(new ToolAuditRecord(
            Timestamp: DateTimeOffset.UtcNow,
            ToolName: tool.Name,
            Operation: tool.Operation,
            RiskTier: tool.RiskTier,
            Decision: "denied",
            Success: false,
            ExternalSideEffect: tool.ExternalSideEffect,
            Reason: reason,
            DurationMs: elapsed));

        return new ToolExecutionResult(
            Allowed: false,
            Status: "denied",
            Result: null,
            Reason: reason,
            ExternalSideEffect: tool.ExternalSideEffect);
    }

    private bool IsApprovalValid(string? supplied)
    {
        if (!_options.RequireApprovalForDraftChangeRequest)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(_options.ApprovalToken) ||
            string.IsNullOrWhiteSpace(supplied))
        {
            return false;
        }

        var expectedBytes = Encoding.UTF8.GetBytes(_options.ApprovalToken);
        var suppliedBytes = Encoding.UTF8.GetBytes(supplied);

        return expectedBytes.Length == suppliedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(
                   expectedBytes,
                   suppliedBytes);
    }

    private static string SerializeResult(ToolExecutionResult result) =>
        JsonSerializer.Serialize(result, JsonOptions);

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };
}
