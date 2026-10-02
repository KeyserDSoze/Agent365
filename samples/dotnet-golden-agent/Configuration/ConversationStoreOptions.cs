namespace Agent365.GoldenAgent.Configuration;

public sealed class ConversationStoreOptions
{
    public const string SectionName = "Conversations";

    public int MaxConversations { get; set; } = 200;
    public int IdleTimeoutMinutes { get; set; } = 30;
}
