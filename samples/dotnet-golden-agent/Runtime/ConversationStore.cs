using System.Collections.Concurrent;
using Microsoft.Agents.AI;

namespace Agent365.GoldenAgent.Runtime;

public sealed class ConversationStore
{
    private readonly ConcurrentDictionary<string, Lazy<Task<AgentSession>>> _sessions = new();

    public Task<AgentSession> GetOrCreateAsync(
        string conversationId,
        AgentRuntime runtime,
        CancellationToken cancellationToken)
    {
        var lazy = _sessions.GetOrAdd(
            conversationId,
            _ => new Lazy<Task<AgentSession>>(
                () => runtime.CreateSessionAsync(cancellationToken),
                LazyThreadSafetyMode.ExecutionAndPublication));

        return lazy.Value;
    }

    public bool Remove(string conversationId) =>
        _sessions.TryRemove(conversationId, out _);
}
