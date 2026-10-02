using Agent365.GoldenAgent.Configuration;
using Agent365.GoldenAgent.Tools;
using Microsoft.Extensions.Options;
using Xunit;

namespace Agent365.GoldenAgent.Tests;

public sealed class ToolGovernanceTests
{
    [Fact]
    public void Registry_ContainsRiskAndOperationMetadata()
    {
        var service = CreateService();

        var catalog = service.GetCatalog();

        Assert.Equal(2, catalog.Count);
        Assert.Contains(catalog, x =>
            x.Name == ToolGovernanceService.PolicyLookupTool &&
            x.Operation == "read" &&
            x.RiskTier == "Low");
        Assert.Contains(catalog, x =>
            x.Name == ToolGovernanceService.DraftChangeRequestTool &&
            x.Operation == "write-draft" &&
            x.RiskTier == "Medium");
    }

    [Fact]
    public void RuntimeBlock_DeniesInvocationAndWritesAudit()
    {
        var context = new ToolInvocationContext();
        var service = CreateService(context: context);

        using var scope = context.Begin("conversation-block", "run-block", "trace-block");
        var updated = service.SetEnabled(
            ToolGovernanceService.PolicyLookupTool,
            enabled: false);

        Assert.NotNull(updated);
        Assert.False(updated.Enabled);

        var result = service.LookupPolicy("AGENT-IDENTITY");

        Assert.False(result.Allowed);
        Assert.Equal("denied", result.Status);
        Assert.Equal("Tool is blocked by policy.", result.Reason);

        var audit = Assert.Single(service.GetAudit());
        Assert.Equal("run-block", audit.RunId);
        Assert.Equal("conversation-block", audit.ConversationId);
        Assert.Equal("trace-block", audit.TraceId);
        Assert.Equal(ToolGovernanceService.PolicyLookupTool, audit.ToolName);
        Assert.Equal("denied", audit.Decision);
        Assert.False(audit.Success);
    }

    [Fact]
    public void Approval_IsConversationBoundAndOneTime()
    {
        var options = new ToolGovernanceOptions
        {
            RequireApprovalForDraftChangeRequest = true,
            ApprovalTtlMinutes = 5,
            AuditCapacity = 100
        };

        var context = new ToolInvocationContext();
        var service = CreateService(options, context);

        using var scope = context.Begin("conversation-approved", "run-approved", "trace-approved");

        var denied = service.CreateDraftChangeRequest(
            "Enable governed tool",
            "Lab test",
            approvalId: null);

        Assert.False(denied.Allowed);

        var approval = service.CreateApproval(
            ToolGovernanceService.DraftChangeRequestTool,
            "conversation-approved",
            "Human approved the mock draft.");

        Assert.NotNull(approval);

        var allowed = service.CreateDraftChangeRequest(
            "Enable governed tool",
            "Lab test",
            approval.Id);

        Assert.True(allowed.Allowed);
        Assert.Equal("completed", allowed.Status);
        Assert.False(allowed.ExternalSideEffect);

        var replay = service.CreateDraftChangeRequest(
            "Enable governed tool",
            "Lab test",
            approval.Id);

        Assert.False(replay.Allowed);
        Assert.Equal(3, service.GetAudit(10).Count);
    }

    [Fact]
    public void Approval_CannotBeUsedByAnotherConversation()
    {
        var options = new ToolGovernanceOptions
        {
            RequireApprovalForDraftChangeRequest = true
        };

        var context = new ToolInvocationContext();
        var service = CreateService(options, context);

        var approval = service.CreateApproval(
            ToolGovernanceService.DraftChangeRequestTool,
            "conversation-a",
            "Approved.");

        Assert.NotNull(approval);

        using var scope = context.Begin("conversation-b", "run-cross", "trace-cross");

        var result = service.CreateDraftChangeRequest(
            "Cross conversation",
            "Must fail",
            approval.Id);

        Assert.False(result.Allowed);
        Assert.Equal("denied", result.Status);
    }

    private static ToolGovernanceService CreateService(
        ToolGovernanceOptions? options = null,
        ToolInvocationContext? context = null)
    {
        context ??= new ToolInvocationContext();
        options ??= new ToolGovernanceOptions();

        return new ToolGovernanceService(
            Options.Create(options),
            context);
    }
}
