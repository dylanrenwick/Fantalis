namespace Fantalis.Core.Logging;

public interface ILogDestination
{
    public void Log(LogMessage message);
}