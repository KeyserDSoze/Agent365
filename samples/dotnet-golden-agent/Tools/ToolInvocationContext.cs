namespace Agent365.GoldenAgent.Tools;

public sealed class ToolInvocationContext
{
    private readonly AsyncLocal<ContextState?> _state = new();

    public string ConversationId => _state.Value?.ConversationId ?? "unknown";
    public string RunId => _state.Value?.RunId ?? "unknown";
    public string TraceId => _state.Value?.TraceId ?? string.Empty;

    public IDisposable Begin(
        string conversationId,
        string runId,
        string traceId)
    {
        var previous = _state.Value;
        _state.Value = new ContextState(conversationId, runId, traceId);

        return new Scope(() => _state.Value = previous);
    }

    private sealed record ContextState(
        string ConversationId,
        string RunId,
        string TraceId);

    private sealed class Scope(Action dispose) : IDisposable
    {
        private Action? _dispose = dispose;

        public void Dispose()
        {
            Interlocked.Exchange(ref _dispose, null)?.Invoke();
        }
    }
}
