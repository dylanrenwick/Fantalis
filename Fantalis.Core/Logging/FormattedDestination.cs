namespace Fantalis.Core.Logging;

public abstract class FormattedDestination : ILogDestination
{
    private readonly LogFormatter _formatter;

    public FormattedDestination(LogFormatter formatter)
    {
        _formatter = formatter;
    }
    
    public void Log(LogMessage message)
    {
        Log(Format(message));
    }

    protected string Format(LogMessage message)
        => _formatter.Format(message);
    
    protected abstract void Log(string message);
}