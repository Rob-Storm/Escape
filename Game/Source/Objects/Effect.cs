using Game.Graphics;
using Raylib_cs;

namespace Game.Objects;

public class Effect : Decoration
{
    public float Lifetime { get; set; } = 1f;
    public bool DestroyOnFinish { get; set; } = false;

    public Effect()
    {
        Renderer = new AnimatedBillboardRenderer()
        {
            AutoSize = true,
            Texture = AssetManager.Load<Texture2D>(@"Assets\Textures\SpriteSheetText.png")
        };

        if(DestroyOnFinish)
        {
            ((AnimatedBillboardRenderer)Renderer).OnEndReached += Destroy;
        }
    }
}
