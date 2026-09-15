using Raylib_cs;

namespace Game;

public class MusicPlayer
{
    public Music? CurrentTrack { get; private set; } = null;

    public void Update()
    {
        if(CurrentTrack == null) return;

        Raylib.UpdateMusicStream(CurrentTrack.Value);
    }

    public void ChangeAudioTrack(Music music)
    {
        CurrentTrack = music;
    }
}
