using Microsoft.Xna.Framework;
using System;

namespace Escape;

public static class VectorExtensions
{
    public static string ToStringTruncated(this Vector3 v)
    {
        return "{" + $"X:{(int)v.X} Y:{(int)v.Y} Z:{(int)v.Z}" + "}";
    }
}

public static class QuaternionExtensions
{
    public static Vector3 ToEulerAngles(this Quaternion q)
    {
        q = Quaternion.Normalize(q);

        float pitch, yaw, roll;

        roll = MathF.Atan2(2 * (q.W * q.X + q.Y * q.Z), 1 - 2 * (MathF.Pow(q.X, 2) + MathF.Pow(q.Y, 2)));

        pitch = MathF.Asin(2 * (q.W * q.Y - q.Z * q.X));

        yaw = MathF.Atan2(2 * (q.W * q.Z + q.X * q.Y), 1 - 2 * (MathF.Pow(q.Y, 2) + MathF.Pow(q.Z, 2)));

        return new Vector3(MathHelper.ToDegrees(yaw), MathHelper.ToDegrees(pitch), MathHelper.ToDegrees(roll));
    }
}