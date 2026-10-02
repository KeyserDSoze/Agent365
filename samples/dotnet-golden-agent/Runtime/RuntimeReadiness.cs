namespace Agent365.GoldenAgent.Runtime;

public sealed record RuntimeReadiness(
    bool Ready,
    string Provider,
    string Model,
    bool ConfigurationValid,
    bool ProviderReachable,
    string? Detail);
