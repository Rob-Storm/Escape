using Raylib_cs;
using RayGUI_cs;

namespace Game.LevelEditor;

public class Editor : World
{
    Button button = new Button(24, 24, 120, 30, "Show Message");

    GuiContainer container = new GuiContainer();

    bool showMessageBox = false;

    public Editor(Camera camera) : base(camera)
    {
        container.Add("button", button);
    }

    public override void Update()
    {
        base.Update();

        _camera.Update();
    }

    public override void Render()
    {
        base.Render();

        Raylib.DrawGrid(10, 1);
    }

    public override void Render2D()
    {
        container.Draw();
    }
}
