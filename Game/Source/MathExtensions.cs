using Raylib_cs;
using System.Numerics;

namespace Game;

public static class QuaternionExtensions
{
    /// <summary>
    /// Converts a quaternion to euler angles, in degrees
    /// </summary>
    /// <param name="q">Quaternion</param>
    /// <returns></returns>
    public static Vector3 ToEulerAngles(this Quaternion q)
    {
        q = Quaternion.Normalize(q);

        // Pitch (X axis)
        float sinPitch = 2f * (q.W * q.X - q.Y * q.Z);

        float pitch;

        if (MathF.Abs(sinPitch) >= 1f)
        {
            pitch = MathF.CopySign(MathF.PI / 2f, sinPitch);
        }
        else
        {
            pitch = MathF.Asin(sinPitch);
        }

        // Yaw (Y axis)
        float siny = 2f * (q.W * q.Y + q.Z * q.X);
        float cosy = 1f - 2f * (q.X * q.X + q.Y * q.Y);

        float yaw = MathF.Atan2(siny, cosy);

        // Roll (Z axis)
        float sinr = 2f * (q.W * q.Z + q.X * q.Y);
        float cosr = 1f - 2f * (q.X * q.X + q.Z * q.Z);

        float roll = MathF.Atan2(sinr, cosr);

        return new Vector3(yaw * Raylib.RAD2DEG, pitch * Raylib.RAD2DEG, roll * Raylib.RAD2DEG);
    }
}