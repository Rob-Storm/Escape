using Game.LevelEditor;
using Raylib_cs;

namespace Game.Graphics;

public abstract class SpriteRenderer : RenderComponent
{
    [ToolTip("Constrains the size to fit the texture dimensions")]
    public bool AutoSize { get; set; } = true;
    [ToolTip("Scales the AutoSize dimensions by a scalar value")]
    public float Scale { get; set; } = 1f;

    protected SpriteRenderer()
    {
        
    }

    protected SpriteRenderer(Texture2D texture) : base(texture)
    {
        
    }
}
