using Agent365.GoldenAgent.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace Agent365.GoldenAgent.Mcp;

public sealed class GovernedMcpToolProvider : IAsyncDisposable
{
    private static readonly HashSet<string> AdministrativeTools =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "get_governance_manifest",
            "get_tool_audit_summary"
        };

    private readonly McpBridgeOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GovernedMcpToolProvider> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private McpClient? _client;
    private IReadOnlyList<AITool> _exposedTools = [];
    private int _discoveredToolCount;
    private bool _initialized;
    private string? _error;

    public GovernedMcpToolProvider(
        IOptions<McpBridgeOptions> options,
        IHostEnvironment environment,
        ILogger<GovernedMcpToolProvider> logger)
    {
        _options = options.Value;
        _environment = environment;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AITool>> GetAgentToolsAsync(
        CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return [];
        }

        await EnsureInitializedAsync(cancellationToken);

        if (_client is null)
        {
            throw new InvalidOperationException(
                $"Governed MCP bridge is enabled but unavailable: {_error ?? "unknown error"}");
        }

        return _exposedTools;
    }

    public async Task<McpBridgeStatus> GetStatusAsync(
        bool initialize,
        CancellationToken cancellationToken)
    {
        if (_options.Enabled && initialize && !_initialized)
        {
            try
            {
                await EnsureInitializedAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Governed MCP bridge initialization failed while reading status.");
            }
        }

        return BuildStatus();
    }

    private async Task EnsureInitializedAsync(
        CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            if (_client is null)
            {
                throw new InvalidOperationException(
                    $"Governed MCP bridge initialization previously failed: {_error ?? "unknown error"}");
            }

            return;
        }

        await _gate.WaitAsync(cancellationToken);

        try
        {
            if (_initialized)
            {
                if (_client is null)
                {
                    throw new InvalidOperationException(
                        $"Governed MCP bridge initialization previously failed: {_error ?? "unknown error"}");
                }

                return;
            }

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);
            timeout.CancelAfter(
                TimeSpan.FromSeconds(_options.InitializationTimeoutSeconds));

            try
            {
                var projectPath = ResolveProjectPath();

                if (!File.Exists(projectPath))
                {
                    throw new FileNotFoundException(
                        "Governed MCP project was not found.",
                        projectPath);
                }

                var environmentVariables =
                    StdioClientTransportOptions.GetDefaultEnvironmentVariables();

                environmentVariables["AGIC_MCP_ENABLE_POLICY_LOOKUP"] =
                    _options.EnablePolicyLookup.ToString().ToLowerInvariant();
                environmentVariables["AGIC_MCP_ENABLE_DRAFT_CHANGE_REQUEST"] =
                    _options.EnableDraftChangeRequest.ToString().ToLowerInvariant();
                environmentVariables["AGIC_MCP_REQUIRE_APPROVAL_FOR_DRAFT"] =
                    _options.RequireApprovalForDraft.ToString().ToLowerInvariant();
                environmentVariables["AGIC_MCP_AUDIT_CAPACITY"] =
                    _options.AuditCapacity.ToString(
                        System.Globalization.CultureInfo.InvariantCulture);

                if (!string.IsNullOrWhiteSpace(_options.ApprovalToken))
                {
                    environmentVariables["AGIC_MCP_APPROVAL_TOKEN"] =
                        _options.ApprovalToken;
                }

                var arguments = new List<string>
                {
                    "run",
                    "--project",
                    projectPath,
                    "--configuration",
                    _options.Configuration
                };

                if (_options.NoBuild)
                {
                    arguments.Add("--no-build");
                }

                arguments.Add("--no-launch-profile");

                var transport = new StdioClientTransport(
                    new StdioClientTransportOptions
                    {
                        Name = "AGIC Agent 365 Governed MCP Bridge",
                        Command = _options.Command,
                        Arguments = arguments,
                        InheritEnvironmentVariables = false,
                        EnvironmentVariables = environmentVariables,
                        ShutdownTimeout = TimeSpan.FromSeconds(
                            _options.ShutdownTimeoutSeconds),
                        StandardErrorLines = line =>
                            _logger.LogDebug(
                                "Governed MCP stderr: {Line}",
                                line)
                    });

                _client = await McpClient.CreateAsync(
                    transport,
                    cancellationToken: timeout.Token);

                var discovered = await _client.ListToolsAsync(
                    cancellationToken: timeout.Token);

                _discoveredToolCount = discovered.Count;
                _exposedTools = discovered
                    .Where(tool =>
                        _options.ExposeAdministrativeTools ||
                        !AdministrativeTools.Contains(tool.Name))
                    .Cast<AITool>()
                    .ToArray();

                _error = null;
            }
            catch (Exception ex)
            {
                _error = ex.Message;

                if (_client is not null)
                {
                    await _client.DisposeAsync();
                    _client = null;
                }

                throw;
            }
            finally
            {
                _initialized = true;
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private string ResolveProjectPath()
    {
        if (string.IsNullOrWhiteSpace(_options.ProjectPath))
        {
            return string.Empty;
        }

        if (Path.IsPathRooted(_options.ProjectPath))
        {
            return Path.GetFullPath(_options.ProjectPath);
        }

        return Path.GetFullPath(
            Path.Combine(
                _environment.ContentRootPath,
                _options.ProjectPath));
    }

    private McpBridgeStatus BuildStatus() =>
        new(
            Enabled: _options.Enabled,
            Required: _options.Required,
            Initialized: _initialized,
            Connected: _client is not null,
            ReplaceBuiltInTools: _options.ReplaceBuiltInTools,
            ExposeAdministrativeTools: _options.ExposeAdministrativeTools,
            ProjectConfigured: !string.IsNullOrWhiteSpace(_options.ProjectPath),
            ApprovalRequired: _options.RequireApprovalForDraft,
            ApprovalTokenConfigured:
                !string.IsNullOrWhiteSpace(_options.ApprovalToken),
            DiscoveredToolCount: _discoveredToolCount,
            ExposedToolNames: _exposedTools.Select(x => x.Name).ToArray(),
            Error: _error);

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync();
            _client = null;
        }

        _gate.Dispose();
    }
}
