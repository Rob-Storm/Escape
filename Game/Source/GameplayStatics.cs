using Raylib_cs;
using System.Numerics;

namespace Game;

public static class GameplayStatics
{
    public static Camera Camera;
    public static void PlaySoundAtLocation(Sound sound, Vector3 location, float maxDist)
    {
        Vector3 direction = Vector3.Subtract(location, Camera.Transform.Position);
        float distance = direction.Length();

        float attenuation = 1f / (1f + (distance / maxDist));
        attenuation = Math.Clamp(attenuation, 0f, 1f);

        Vector3 normalizedDirection = Vector3.Normalize(direction);

        float dotProduct = Vector3.Dot(Camera.GetForwardVector(), normalizedDirection);
        if (dotProduct < 0f) attenuation *= (1.0f + dotProduct * 0.5f);

        float pan = 0.5f * Vector3.Dot(normalizedDirection, Camera.GetRightVector());
        pan = Math.Clamp(pan, -1f, 1f);

        Raylib.SetSoundVolume(sound, attenuation);
        Raylib.SetSoundPan(sound, pan);

        Raylib.PlaySound(sound);
    }

    public static void PlaySound2D(Sound sound, float volume = 1.0f)
    {
        Raylib.SetSoundVolume(sound, volume);
        Raylib.PlaySound(sound);
    }
}
