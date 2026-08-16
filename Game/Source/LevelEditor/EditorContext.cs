using Game.LevelEditor.Panels;
using Game.LevelEditor.Services;
using Game.Objects;
using Raylib_cs;
using System.Numerics;
using System.Xml.Linq;

namespace Game.LevelEditor;

public class EditorContext
{
    public event Action<bool>? OnDirtyChanged;

    public World World { get; }
    public EditorCamera Camera { get; }

    public object? SelectedObject;
    public object? SelectedAsset;

    public bool SelectedAnything => SelectedObject != null || SelectedAsset != null;

    public Cell? SelectedCell => World.GetCell(SelectedX, SelectedY);
    public ToolMode ToolMode = ToolMode.Select;
    public PaintWallSettings ToolSettings;
    public Type EntitySpawnClass = typeof(Door);

    public int SelectedX;
    public int SelectedY;

    public string LevelName;
    public Texture2D Skybox;
    public Music BackgroundMusic;
    public Vector2 PlayerStart;
    public float StartRotation;

    public string? DraggedAssetPath;

    public PlayModeService PlayModeService;
    public AssetService AssetService;
    public LevelFileService LevelFileService;

    public EditorLayout Layout = new EditorLayout();

    public bool HasSavedThisSession = false;
    public string LastLevelSavePath = string.Empty;

    /// <summary>
    /// 'Dirty' refers to unsaved changes
    /// </summary>
    public bool IsDirty { get; private set; } = false;


    public EditorContext(World world, EditorCamera camera)
    {
        World = world;
        Camera = camera;

        LevelName = "Level";
        PlayerStart = Vector2.Zero;
        Skybox = AssetManager.Load<Texture2D>(@"Assets\Texture\Default.png");
        BackgroundMusic = AssetManager.Load<Music>(@"Assets\Music\Default.ogg");
        StartRotation = 0f;

        ToolSettings = new PaintWallSettings();

        PlayModeService = new PlayModeService();
        LevelFileService = new LevelFileService();
        AssetService = new AssetService();

        LevelFileService.OnCreateNewLevel += (name) =>
        {
            IsDirty = false;
            Raylib.SetWindowTitle($"Editor - {name}");
            OnDirtyChanged?.Invoke(IsDirty);
        };

        LevelFileService.OnSaveLevel += () => 
        {
            IsDirty = false;
            Raylib.SetWindowTitle($"Editor - {LevelName}");
            OnDirtyChanged?.Invoke(IsDirty);
        };

        LevelFileService.OnLoadLevel += (name) =>
        {
            IsDirty = false;
            Raylib.SetWindowTitle($"Editor - {name}");
            OnDirtyChanged?.Invoke(IsDirty);
        };
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

    public void MarkDirty()
    {
        IsDirty = true;
        Raylib.SetWindowTitle($"Editor - {LevelName}*");
        OnDirtyChanged?.Invoke(IsDirty);
    }
}