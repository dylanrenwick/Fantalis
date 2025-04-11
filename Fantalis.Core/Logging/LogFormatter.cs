using System;

namespace Fantalis.Core.Logging;

public class LogFormatter
{
    private readonly Func<LogMessage, string> _formatter;
    
    public LogFormatter(Func<LogMessage, string> formatter)
    {
        _formatter = formatter;
    }

    public string Format(LogMessage message)
        => _formatter(message);

    public static implicit operator LogFormatter(Func<LogMessage, string> formatter)
    {
        return new(formatter);
    }
}