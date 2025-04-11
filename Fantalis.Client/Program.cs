using Fantalis.Client;
using Fantalis.Core.Logging;

Logger logger = new(
    "Client",
    new ConsoleDestination(
        new LogFormatter(msg
            => $"[{msg.Time}]{msg.LoggerName,8}{(msg.SubName.Length > 0 ? "-"+msg.SubName : string.Empty)}| {msg.Message}"
        )
    )
);

using var game = new FantalisGame(logger);
game.Run();
