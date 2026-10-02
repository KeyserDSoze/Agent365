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

        Assert.Contains(
            ToolGovernanceService.PolicyLookupTool,
            json,
            StringComparison.Ordinal);
        Assert.Contains(
            ToolGovernanceService.DraftChangeRequestTool,
            json,
            StringComparison.Ordinal);
        Assert.Contains(
            ""riskTier"",
            json,
            StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            ""approvalTokenExposed": false",
            json,
            StringComparison.OrdinalIgnoreCase);
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

        var audit = service.GetAuditSummaryJson();
        Assert.Contains(
            ""allowed": 1",
            audit,
            StringComparison.OrdinalIgnoreCase);
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

        Assert.Contains(
            ""bufferedEvents": 10",
            json,
            StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            "SECRET-ARGUMENT",
            json,
            StringComparison.Ordinal);
        Assert.Contains(
            ""capturesArguments": false",
            json,
            StringComparison.OrdinalIgnoreCase);
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
