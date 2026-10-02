namespace Agent365.GoldenAgent.Runtime;

public sealed class ConversationCapacityException : InvalidOperationException
{
    public ConversationCapacityException(string message)
        : base(message)
    {
    }
}
