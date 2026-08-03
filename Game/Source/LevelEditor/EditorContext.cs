using Game.LevelEditor.Panels;
using Game.LevelEditor.Services;
using Game.Objects;
using System.Numerics;

namespace Game.LevelEditor;

public class EditorContext
{
    public World World { get; }
    public EditorCamera Camera { get; }

    public object? SelectedObject;
    public object? SelectedAsset;

    public bool SelectedAnything => SelectedObject != null || SelectedAsset != null;

    public Cell SelectedCell => World.GetCell(SelectedX, SelectedY);
    public ToolMode ToolMode = ToolMode.Select;
    public PaintWallSettings ToolSettings;
    public Type EntitySpawnClass = typeof(Door);

    public int SelectedX;
    public int SelectedY;

    public string LevelName;
    public Vector2 PlayerStart;
    public float StartRotation;

    public string? DraggedAssetPath;

    public PlayModeService PlayModeService;
    public AssetService AssetService;
    public LevelFileService LevelFileService;

    public EditorContext(World world, EditorCamera camera)
    {
        World = world;
        Camera = camera;

        LevelName = "Level";
        PlayerStart = Vector2.Zero;
        StartRotation = 0f;

        ToolSettings = new PaintWallSettings();

        PlayModeService = new PlayModeService();
        LevelFileService = new LevelFileService();
        AssetService = new AssetService();
    }

    // HACK: Make cells entities to simplify this (and SetSelectedAsset) garbage
    public void SetSelectedObject(object newObject)
    {
        SelectedObject = newObject;

        SelectedAsset = null;
    }

    public void SetSelectedAsset(object asset)
    {
        SelectedAsset = asset;

        SelectedObject = null;
    }
}