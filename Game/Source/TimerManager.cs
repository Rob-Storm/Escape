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

    public static TimerHandle SetTimer(float delay, Action callback, bool loop = false)
    {
        TimerHandle handle = new TimerHandle()
        {
            Delay = delay,
            Callback = callback,
            Loop = loop
        };

        _handles.Add(handle);
        return handle;
    }

    public static void InvalidateTimerByHandle(TimerHandle handle)
    {
        handle.Invalidate();
        _invalidHandles.Add(handle);
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
                handle.Callback?.Invoke();

                if (!handle.Loop)
                {
                    handle.Invalidate();
                    _invalidHandles.Add(handle);
                }
                else
                {
                    handle.CurrentTicks = 0;
                }
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
    public Guid Handle { get; }
    public float Delay { get; set; }
    public float CurrentTicks { get; set; }
    public bool Loop { get; set; } = false;
    public bool IsValid { get; private set; } = true;
    public Action? Callback { get; set; }

    public TimerHandle()
    {
        Handle = Guid.NewGuid();
    }

    public bool Equals(TimerHandle? other) => this.Handle == other?.Handle;

    public override string ToString()
    {
        return Handle.ToString();
    }

    public bool Invalidate() => IsValid = false;
}