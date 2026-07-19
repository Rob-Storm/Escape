using System.Diagnostics;

namespace Game;

public static class Time
{
    public static double FrameDelta { get; private set; }

    private static Stopwatch _stopwatch = Stopwatch.StartNew();
    private static long _previousTicks;

    public static void Update()
    {
        long currentTicks = _stopwatch.ElapsedTicks;

        FrameDelta = (double)(currentTicks - _previousTicks) / Stopwatch.Frequency;

        _previousTicks = currentTicks;
    }
}
