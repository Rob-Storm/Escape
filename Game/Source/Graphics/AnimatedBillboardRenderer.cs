using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Game.Graphics;

public class AnimatedBillboardRenderer : SpriteRenderer, IJsonOnDeserialized
{
    public event Action? OnEndReached;

    [ToolTip("The size of one frame of the animation")]
    public Vector2 FrameSize { get; set; }

    [ToolTip("The time each frame is held")]
    public float PlaybackSpeed { get; set; }

    [ToolTip("The time each frame is held")]
    public bool Loop { get; set; } = true;

    private int _currentFrame = 0;
    private int _totalFrames;

    public void Play()
    {
        TimerManager.SetTimer(PlaybackSpeed, AdvanceFrame, true);
    }

    public void AdvanceFrame()
    {
        _totalFrames = (int)(Texture.Width / FrameSize.X) - 1;

        if (_currentFrame >= _totalFrames)
        {
            if(Loop)
            {
                _currentFrame = -1;
            }
            else
            {
                OnEndReached?.Invoke();
                return;
            }
        }

        _currentFrame++;

    }

    public override void Render(Camera camera, Transform transform)
    {
        Rectangle source = new Rectangle()
        {
            X = _currentFrame * FrameSize.X,
            Y = 0,
            Width = FrameSize.X,
            Height = FrameSize.Y
        };

        Raylib.DrawBillboardRec(camera, Texture, source, transform.Position, Vector2.One, Tint);
    }

    public override void DebugRender(Camera camera, Transform transform)
    {

    }

    public void OnDeserialized()
    {
        if (!Engine.IsEditor)
        {
            Play();
        }
    }
}
