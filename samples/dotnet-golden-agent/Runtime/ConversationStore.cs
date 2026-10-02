using System.Collections.Concurrent;
using Microsoft.Agents.AI;

namespace Agent365.GoldenAgent.Runtime;

public sealed class ConversationStore
{
    private readonly ConcurrentDictionary<string, ConversationState> _conversations = new();

    public async Task<string> RunAsync(
        string conversationId,
        string message,
        AgentRuntime runtime,
        CancellationToken cancellationToken)
    {
        var state = _conversations.GetOrAdd(
            conversationId,
            static _ => new ConversationState());

        await state.Gate.WaitAsync(cancellationToken);

        try
        {
            state.Session ??= await runtime.CreateSessionAsync(cancellationToken);

            return await runtime.RunAsync(
                message,
                state.Session,
                cancellationToken);
        }
        finally
        {
            state.Gate.Release();
        }
    }

    public bool Remove(string conversationId) =>
        _conversations.TryRemove(conversationId, out _);

    private sealed class ConversationState
    {
        public SemaphoreSlim Gate { get; } = new(1, 1);
        public AgentSession? Session { get; set; }
    }
}
