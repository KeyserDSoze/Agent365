namespace Agent365.GoldenAgent.Configuration;

public sealed class ApiOptions
{
    public const string SectionName = "Api";

    public bool RequireApiKey { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public int RequestsPerMinute { get; set; } = 30;
    public int MaxMessageCharacters { get; set; } = 8000;
    public int MaxConversationIdCharacters { get; set; } = 128;
}
