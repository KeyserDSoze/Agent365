using System.Collections.Concurrent;
using Agent365.GoldenAgent.Configuration;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;

namespace Agent365.GoldenAgent.Runtime;

public sealed class ConversationStore
{
    private readonly ConcurrentDictionary<string, ConversationState> _conversations = new();
    private readonly ConversationStoreOptions _options;

    public ConversationStore(IOptions<ConversationStoreOptions> options)
    {
        _options = options.Value;
    }

    public int Count => _conversations.Count;

    public async Task<string> RunAsync(
        string conversationId,
        string message,
        AgentRuntime runtime,
        CancellationToken cancellationToken)
    {
        RemoveExpired();

        if (!_conversations.TryGetValue(conversationId, out var state))
        {
            if (_conversations.Count >= _options.MaxConversations)
            {
                throw new ConversationCapacityException(
                    $"Conversation capacity reached ({_options.MaxConversations}).");
            }

            state = _conversations.GetOrAdd(
                conversationId,
                static _ => new ConversationState());
        }

        await state.Gate.WaitAsync(cancellationToken);

        try
        {
            state.LastAccessUtc = DateTimeOffset.UtcNow;
            state.Session ??= await runtime.CreateSessionAsync(cancellationToken);

            var result = await runtime.RunAsync(
                message,
                state.Session,
                conversationId,
                cancellationToken);

            state.LastAccessUtc = DateTimeOffset.UtcNow;
            return result;
        }
        finally
        {
            state.Gate.Release();
        }
    }

    public bool Remove(string conversationId) =>
        _conversations.TryRemove(conversationId, out _);

    public int RemoveExpired()
    {
        var cutoff = DateTimeOffset.UtcNow.AddMinutes(-_options.IdleTimeoutMinutes);
        var removed = 0;

        foreach (var item in _conversations)
        {
            if (item.Value.LastAccessUtc >= cutoff || item.Value.Gate.CurrentCount == 0)
            {
                continue;
            }

            if (_conversations.TryRemove(item.Key, out _))
            {
                removed++;
            }
        }

        return removed;
    }

    private sealed class ConversationState
    {
        public SemaphoreSlim Gate { get; } = new(1, 1);
        public AgentSession? Session { get; set; }
        public DateTimeOffset LastAccessUtc { get; set; } = DateTimeOffset.UtcNow;
    }
}
