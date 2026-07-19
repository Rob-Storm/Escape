using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace Escape;

public static class Cursor
{
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);

    public static Vector2 GetCursorPosition()
    {
        GetCursorPos(out POINT lpPoint);
        return new Vector2(lpPoint.X, lpPoint.Y);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int X;
    public int Y;

    public POINT(int x, int y)
    {
        X = x;
        Y = y;
    }
}
