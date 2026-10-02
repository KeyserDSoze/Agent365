using Agent365.GoldenAgent.Configuration;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;

namespace Agent365.GoldenAgent.Telemetry;

public sealed class Agent365TokenProvider
{
    private readonly Agent365ObservabilityOptions _options;
    private readonly Lazy<ClientSecretCredential?> _credential;

    public Agent365TokenProvider(IOptions<Agent365ObservabilityOptions> options)
    {
        _options = options.Value;
        _credential = new Lazy<ClientSecretCredential?>(CreateCredential);
    }

    public async Task<string> GetTokenAsync(
        string agentId,
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        if (!_options.ExportToAgent365)
        {
            return string.Empty;
        }

        ValidateIdentity(agentId, tenantId);

        var credential = _credential.Value
            ?? throw new InvalidOperationException("Agent 365 credential is not configured.");

        var token = await credential.GetTokenAsync(
            new TokenRequestContext([Agent365ObservabilityOptions.ObservabilityScope]),
            cancellationToken);

        return token.Token;
    }

    private ClientSecretCredential? CreateCredential()
    {
        if (!_options.ExportToAgent365)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(_options.TenantId) ||
            string.IsNullOrWhiteSpace(_options.AgentId) ||
            string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            throw new InvalidOperationException(
                "Agent365 export requires TenantId, AgentId and ClientSecret.");
        }

        return new ClientSecretCredential(
            _options.TenantId,
            _options.AgentId,
            _options.ClientSecret);
    }

    private void ValidateIdentity(string agentId, string tenantId)
    {
        if (!string.Equals(agentId, _options.AgentId, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The baggage agent ID does not match the configured app registration Client ID.");
        }

        if (!string.Equals(tenantId, _options.TenantId, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The baggage tenant ID does not match the configured Tenant ID.");
        }
    }
}
