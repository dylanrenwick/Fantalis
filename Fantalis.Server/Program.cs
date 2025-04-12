﻿using System;
using System.Threading.Tasks;

using Fantalis.Core.Logging;

namespace Fantalis.Server;

public static class Program
{
    private static readonly LogFormatter _standardFormat = new(
        msg => $"[{msg.Time}]{msg.LoggerName,8}{(msg.SubName.Length > 0 ? "-" + msg.SubName : string.Empty)}| {msg.Message}"
    );

    private static async Task Main(string[] args)
    {
        Logger logger = new(
            "Server",
            new ConsoleDestination(_standardFormat)
        );

        FantalisServer server = new(".", logger);
        _ = server.Start();
        Console.ReadLine();
        await server.Stop();
    }
}