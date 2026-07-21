using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public delegate void ViewportControlChangedSignature(bool newControl);

public class Viewport : EditorPanel
{
    public event ViewportControlChangedSignature ViewportControlChanged;

    public bool ViewportControlled { get; private set; } = false;

    public RenderTexture2D ViewportRenderTarget { get; private set; }
    private Vector2 _viewportSize = new Vector2(960, 540);

    private bool _previousControlState;
    EditorCamera _camera;

    public Viewport(EditorContext context) : base(context)
    {         
        _camera = context.Camera;
        CreateViewportRenderTarget();
    }

    private void CreateViewportRenderTarget()
    {
        ViewportRenderTarget = Raylib.LoadRenderTexture
            (
                (int)_viewportSize.X,
                (int)_viewportSize.Y
            );
    }


    private void ResizeViewport(Vector2 newSize)
    {
        if (newSize.X <= 0 || newSize.Y <= 0)
        {
            return;
        }

        _camera.SetAspectRatio(newSize);

        if (newSize != _viewportSize)
        {
            _viewportSize = newSize;

            Raylib.UnloadRenderTexture(ViewportRenderTarget);
            CreateViewportRenderTarget();
        }
    }


    public override void Draw()
    {
        ImGui.Begin("Viewport");

        bool viewportHovered = ImGui.IsItemHovered();

        Vector2 size = ImGui.GetContentRegionAvail();
        ResizeViewport(size);

        ViewportControlled = Raylib.IsMouseButtonDown(MouseButton.Right) && ImGui.IsWindowHovered();

        if (ViewportControlled != _previousControlState)
        {
            ViewportControlChanged?.Invoke(ViewportControlled);
        }

        if (ViewportControlled)
        {
            Vector2 viewportCenter = size;

            viewportCenter.X *= 0.5f;
            viewportCenter.Y *= 0.5f;

            Raylib.SetMousePosition((int)viewportCenter.X, (int)viewportCenter.Y);
        }

        rlImGui.ImageRenderTexture(ViewportRenderTarget);

        ImGui.End();

        _previousControlState = ViewportControlled;
    }
}
