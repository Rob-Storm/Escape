using System.Numerics;

namespace Game;

public class Transform
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
    }

    public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }

    public Transform Copy() => (Transform)MemberwiseClone();

    public void Translate(Vector3 translation) => Position += translation;
    public void Rotate(Vector3 rotation)
    {
        Rotation *= Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
        Rotation = Quaternion.Normalize(Rotation);
    }
}

public static class Directions
{
    /// <summary>
    /// (0, 0, -1)
    /// </summary>
    public static readonly Vector3 Forward = -Vector3.UnitZ;

    /// <summary>
    /// (0, 0, 1)
    /// </summary>
    public static readonly Vector3 Backward = Vector3.UnitZ;

    /// <summary>
    /// (0, 1, 0)
    /// </summary>
    public static readonly Vector3 Up = Vector3.UnitY;

    /// <summary>
    /// (0, -1, 0)
    /// </summary>
    public static readonly Vector3 Down = -Vector3.UnitY;

    /// <summary>
    /// (1, 0, 0)
    /// </summary>
    public static readonly Vector3 Left = Vector3.UnitX;

    /// <summary>
    /// (-1, 0, 0)
    /// </summary>
    public static readonly Vector3 Right = -Vector3.UnitX;
}
