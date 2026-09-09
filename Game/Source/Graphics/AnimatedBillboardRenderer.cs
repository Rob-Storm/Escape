using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;

namespace Game.Graphics;

public class AnimatedBillboardRenderer : SpriteRenderer
{
    public event Action? OnEndReached;

    [ToolTip("The size of one frame of the animation")]
    public Vector2 FrameSize { get; set; }

    [ToolTip("The time each frame is held")]
    public float PlaybackSpeed = 1;

    private int _currentFrame = 1;
    private int _totalFrames;

    public AnimatedBillboardRenderer()
    {
        if(!Engine.IsEditor)
        {
            TimerManager.SetTimer(PlaybackSpeed, AdvanceFrame, true);
        }
    }

    public AnimatedBillboardRenderer(Texture2D texture) : base(texture)
    {
        //_totalFrames = (int)FrameSize.X / texture.Width;

        //if (!Engine.IsEditor)
        //{
        //    TimerManager.SetTimer(PlaybackSpeed, AdvanceFrame, true);
        //}
    }

    public void AdvanceFrame()
    {
        _totalFrames = (int)(Texture.Width / FrameSize.X);

        if (_currentFrame >= _totalFrames)
        {
            _currentFrame = 1;
            OnEndReached?.Invoke();
        }
        else
        {
            _currentFrame++;
        }
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

        Raylib.DrawBillboardRec(camera, Texture, source, transform.Position, Vector2.One, Color.White);
    }

    public override void DebugRender(Camera camera, Transform transform)
    {

    }
}
