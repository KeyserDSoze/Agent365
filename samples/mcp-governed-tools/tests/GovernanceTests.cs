using System.Text.Json;
using Agent365.GovernedMcpServer.Governance;
using Xunit;

namespace Agent365.GovernedMcpServer.Tests;

public sealed class GovernanceTests
{
    [Fact]
    public void Manifest_ContainsRiskMetadata_WithoutApprovalSecret()
    {
        var service = CreateService(
            requireApproval: true,
            approvalToken: "top-secret-approval");

        var json = service.GetManifestJson();

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.Equal(
            "agic-agent365-mcp-governance/v1",
            root.GetProperty("schema").GetString());
        Assert.False(
            root.GetProperty("approvalTokenExposed").GetBoolean());

        var tools = root.GetProperty("tools").EnumerateArray().ToArray();

        Assert.Contains(
            tools,
            tool =>
                tool.GetProperty("name").GetString() ==
                    ToolGovernanceService.PolicyLookupTool &&
                tool.GetProperty("riskTier").GetString() == "Low");

        Assert.Contains(
            tools,
            tool =>
                tool.GetProperty("name").GetString() ==
                    ToolGovernanceService.DraftChangeRequestTool &&
                tool.GetProperty("riskTier").GetString() == "Medium" &&
                tool.GetProperty("requiresApproval").GetBoolean());

        Assert.DoesNotContain(
            "top-secret-approval",
            json,
            StringComparison.Ordinal);
    }

    [Fact]
    public void PolicyLookup_IsReadOnlyAndAudited()
    {
        var service = CreateService();

        var json = service.LookupPolicy("AGENT-IDENTITY");

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.True(root.GetProperty("allowed").GetBoolean());
        Assert.Equal(
            "completed",
            root.GetProperty("status").GetString());
        Assert.False(
            root.GetProperty("externalSideEffect").GetBoolean());

        using var auditDocument = JsonDocument.Parse(
            service.GetAuditSummaryJson());

        var summary = auditDocument.RootElement.GetProperty("summary");

        Assert.Equal(1, summary.GetProperty("allowed").GetInt32());
        Assert.Equal(0, summary.GetProperty("denied").GetInt32());
    }

    [Fact]
    public void WriteShapedTool_IsDeniedWithoutRequiredOperatorApproval()
    {
        var service = CreateService(
            requireApproval: true,
            approvalToken: "approved-by-human");

        var json = service.CreateDraftChangeRequest(
            "Rotate credential",
            "Controlled lab change",
            approvalToken: null);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.False(root.GetProperty("allowed").GetBoolean());
        Assert.Equal(
            "denied",
            root.GetProperty("status").GetString());
        Assert.Contains(
            "operator-provisioned",
            root.GetProperty("reason").GetString(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CorrectOperatorApproval_AllowsOnlyMockDraft()
    {
        var service = CreateService(
            requireApproval: true,
            approvalToken: "approved-by-human");

        var json = service.CreateDraftChangeRequest(
            "Rotate credential",
            "Controlled lab change",
            approvalToken: "approved-by-human");

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.True(root.GetProperty("allowed").GetBoolean());
        Assert.False(
            root.GetProperty("externalSideEffect").GetBoolean());

        var result = root.GetProperty("result");
        Assert.Equal("draft", result.GetProperty("state").GetString());
        Assert.False(
            result.GetProperty("externalSideEffect").GetBoolean());
    }

    [Fact]
    public void BlockedTool_IsDeniedRegardlessOfApproval()
    {
        var options = new ToolPolicyOptions(
            EnablePolicyLookup: true,
            EnableDraftChangeRequest: false,
            RequireApprovalForDraftChangeRequest: true,
            ApprovalToken: "approved-by-human",
            AuditCapacity: 100);

        var audit = new ToolAuditStore(options);
        var service = new ToolGovernanceService(options, audit);

        var json = service.CreateDraftChangeRequest(
            "Blocked change",
            "Must not execute",
            approvalToken: "approved-by-human");

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.False(root.GetProperty("allowed").GetBoolean());
        Assert.Contains(
            "blocked",
            root.GetProperty("reason").GetString(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Audit_IsBounded_AndDoesNotCaptureToolArguments()
    {
        var options = new ToolPolicyOptions(
            EnablePolicyLookup: true,
            EnableDraftChangeRequest: true,
            RequireApprovalForDraftChangeRequest: false,
            ApprovalToken: string.Empty,
            AuditCapacity: 10);

        var audit = new ToolAuditStore(options);
        var service = new ToolGovernanceService(options, audit);

        for (var i = 0; i < 15; i++)
        {
            service.LookupPolicy($"SECRET-ARGUMENT-{i}");
        }

        var json = service.GetAuditSummaryJson();

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.False(root.GetProperty("capturesArguments").GetBoolean());
        Assert.Equal(
            10,
            root.GetProperty("summary")
                .GetProperty("bufferedEvents")
                .GetInt32());

        Assert.DoesNotContain(
            "SECRET-ARGUMENT",
            json,
            StringComparison.Ordinal);
    }

    private static ToolGovernanceService CreateService(
        bool requireApproval = false,
        string approvalToken = "")
    {
        var options = new ToolPolicyOptions(
            EnablePolicyLookup: true,
            EnableDraftChangeRequest: true,
            RequireApprovalForDraftChangeRequest: requireApproval,
            ApprovalToken: approvalToken,
            AuditCapacity: 100);

        return new ToolGovernanceService(
            options,
            new ToolAuditStore(options));
    }
}
