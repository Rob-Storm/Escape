namespace Game;

public class TimerManager
{
    private static HashSet<TimerHandle> _handles;
    private static List<TimerHandle> _invalidHandles;

    static TimerManager()
    {
        _handles = new HashSet<TimerHandle>();
        _invalidHandles = new List<TimerHandle>();
    }

    public static TimerHandle SetTimer(float delay, Action callback)
    {
        TimerHandle handle = new TimerHandle()
        {
            Delay = delay,
            Callback = callback
        };

        _handles.Add(handle);
        return handle;
    }

    public static void Update()
    {
        if (_handles.Count < 1)
        {
            return;
        }

        foreach (TimerHandle handle in _handles)
        {
            if (!handle.IsValid)
            {
                continue;
            }

            handle.CurrentTicks += (float)Time.FrameDelta;

            if (handle.CurrentTicks >= handle.Delay)
            {
                Debug.Log($"Execute timer callback {handle}", channel: LogChannel.Timer);
                handle.Callback?.Invoke();

                handle.IsValid = false;
                _invalidHandles.Add(handle);
            }
        }

        foreach (TimerHandle handle in _invalidHandles)
        {
            _handles.Remove(handle);
        }

        _invalidHandles.Clear();

    }
}

public class TimerHandle : IEquatable<TimerHandle>
{
    public int Handle { get; }
    public float Delay { get; set; }
    public float CurrentTicks { get; set; }
    public bool IsValid { get; internal set; } = true;
    public Action? Callback { get; set; }

    public TimerHandle()
    {
        Handle = Guid.NewGuid().GetHashCode();
    }

    public bool Equals(TimerHandle? other) => this.Handle == other?.Handle;
    public override int GetHashCode() => Handle.GetHashCode();

    public override string ToString()
    {
        return Handle.ToString();
    }
}