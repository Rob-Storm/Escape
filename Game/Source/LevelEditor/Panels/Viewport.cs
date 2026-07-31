using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public delegate void ViewportControlChangedSignature(bool newControl);
public class Viewport : EditorPanel
{
    public event ViewportControlChangedSignature? ViewportControlChanged;

    public bool ViewportControlled { get; private set; } = false;

    public RenderTexture2D ViewportRenderTarget { get; private set; }
    private Vector2 _viewportSize = new Vector2(960, 540);
    private Vector2 _viewportLocation;

    private bool _previousControlState;
    EditorCamera _camera;

    Ray mouseRay;

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

    public void Update()
    {
        if(Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            ClickViewport();
        }
    }

    public override void Draw()
    {
        Raylib.DrawRay(mouseRay, Color.Red);

        ImGui.Begin("Viewport");

        _viewportLocation = ImGui.GetWindowPos();

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

    private void ClickViewport()
    {
        Rectangle viewportRect = new Rectangle
        {
            Position = _viewportLocation,
            Size = _viewportSize
        };

        Vector2 mousePos = Raylib.GetMousePosition();
        Vector2 localMousePos = new Vector2(mousePos.X - _viewportLocation.X, mousePos.Y - _viewportLocation.Y);

        if(localMousePos.X < 0 || localMousePos.Y < 0 || localMousePos.X >= _viewportSize.X || localMousePos.Y >= _viewportSize.Y)
        {
            return;
        }

        RayCollision closest = default;
        Collider? selected = null;

        mouseRay = Raylib.GetScreenToWorldRayEx(localMousePos, _camera, (int)_viewportSize.X, (int)_viewportSize.Y);

        foreach (Collider collider in _context.World.GetCollidables())
        {
            RayCollision hit = Raylib.GetRayCollisionBox(mouseRay, collider.BoundingBox);

            if(!hit.Hit)
            { 
                continue; 
            }

            if(selected == null || hit.Distance < closest.Distance)
            {
                closest = hit;
                selected = collider;
            }
        }

        _context.SelectedObject = selected != null ? selected.Parent : null;

        Debug.Log($"Mouse Pick: {(selected != null ? selected.ToString() : "None" )}");
    }
}
