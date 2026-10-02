namespace Agent365.GoldenAgent.Tools;

public sealed class ToolInvocationContext
{
    private readonly AsyncLocal<string?> _conversationId = new();

    public string ConversationId => _conversationId.Value ?? "unknown";

    public IDisposable Begin(string conversationId)
    {
        var previous = _conversationId.Value;
        _conversationId.Value = conversationId;
        return new Scope(() => _conversationId.Value = previous);
    }

    private sealed class Scope(Action dispose) : IDisposable
    {
        private Action? _dispose = dispose;

        public void Dispose()
        {
            Interlocked.Exchange(ref _dispose, null)?.Invoke();
        }
    }
}
