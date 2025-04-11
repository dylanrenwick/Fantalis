using System;

namespace Fantalis.Core.Logging;

public class ConsoleDestination : FormattedDestination
{
    public ConsoleDestination(LogFormatter formatter)
        : base(formatter) { }

    protected override void Log(string message)
    {
        Console.WriteLine(message);
    }
}