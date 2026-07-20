using Raylib_cs;

namespace Game;

public delegate void LogCommittedSignature(string message, LogLevel level, LogChannel channel);

public static class Debug
{
    public static event LogCommittedSignature? OnLogCommitted;

    public static void Log(string message = "Hello", LogLevel level = LogLevel.Log, LogChannel channel = LogChannel.General, bool writeToLog = true)
    {
        string logPrefix = string.Empty;
        string timeStamp = string.Empty;
        string finalMessage = string.Empty;

        ConsoleColor lineColor = ConsoleColor.White;

        switch (level)
        {
            case LogLevel.Log:
                logPrefix = "Log";
                lineColor = ConsoleColor.White;
                break;
            case LogLevel.Warning:
                logPrefix = "Warning";
                lineColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                logPrefix = "ERROR";
                lineColor = ConsoleColor.Red;
                break;
        }

        timeStamp = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}.{DateTime.Now.Millisecond}";

        finalMessage = $"[{timeStamp}] [{logPrefix}] [{channel.ToString()}] {message}";

        OnLogCommitted?.Invoke(message, level, channel);

        Console.ForegroundColor = lineColor;
        Console.WriteLine(finalMessage);
        Console.ForegroundColor = ConsoleColor.White;
    }   
}

public enum LogChannel
{
    General,
    Input,
    Asset,
    Config,
    UserInterface,
    Timer,
    Raylib
};

public enum LogLevel
{
    Log,
    Warning,
    Error
};
