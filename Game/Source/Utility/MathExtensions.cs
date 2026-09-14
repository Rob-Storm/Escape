using Raylib_cs;
using System.Numerics;

namespace Game.Utility;

public static class ColorExtensions
{
    public static Vector4 ToVector4(this Color color)
    {
        return new Vector4(color.R, color.G, color.B, color.A);
    }

    public static Color ToColor(this Vector4 vector4)
    {
        return new Color(vector4.X, vector4.Y, vector4.Z, vector4.W);
    }
}

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