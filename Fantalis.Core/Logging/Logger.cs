using System;
using System.Collections.Generic;
using System.Threading;

namespace Fantalis.Core.Logging;

public class Logger : ILogDestination
{
    private readonly string _name;
    private readonly string _subName;
    private readonly Lock _lock = new();
    private readonly List<ILogDestination> _destinations = [];

    public Logger(string name, params ILogDestination[] destinations)
        : this(name, string.Empty, destinations) { }
    public Logger(
        string name,
        string subName,
        params ILogDestination[] destinations
    ) {
        _name = name;
        _subName = subName;
        _destinations.AddRange(destinations);
    }

    public void AddDestination(ILogDestination destination)
    {
        lock (_lock)
        {
            _destinations.Add(destination);
        }
    }

    public Logger WithName(string name)
    {
        return new Logger(name, this);
    }

    public Logger SubLogger(string subName)
    {
        return new Logger(_name, subName, this);
    }

    public void Log(string message)
    {
        Log(new LogMessage()
        {
            LoggerName = _name,
            SubName = _subName,
            Message = message
        });
    }

    public void Log(LogMessage message)
    {
        LogToDestinations(message);
    }

    private void LogToDestinations(LogMessage message)
    {
        lock (_lock)
        {
            foreach (ILogDestination destination in _destinations)
            {
                destination.Log(message);
            }
        }
    }
}