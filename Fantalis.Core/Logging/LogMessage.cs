using System;

namespace Fantalis.Core.Logging;

public record LogMessage
{
    public DateTime Time { get; init; } = DateTime.UtcNow;
    
    public required string LoggerName { get; init; }
    public string SubName { get; init; } = string.Empty;
    public required string Message { get; init; }
}