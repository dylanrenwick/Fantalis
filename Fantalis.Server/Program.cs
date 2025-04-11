using Fantalis.Core.Logging;
using Fantalis.Server;

Logger logger = new(
    "Server",
    new ConsoleDestination(
        new LogFormatter(msg
            => $"[{msg.Time}]{msg.LoggerName,8}{(msg.SubName.Length > 0 ? "-"+msg.SubName : string.Empty)}| {msg.Message}"
        )
    )
);

FantalisServer server = new(".", logger);
await server.Start();