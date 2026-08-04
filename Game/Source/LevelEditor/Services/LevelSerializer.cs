using NativeFileDialogSharp;
using System.Numerics;

namespace Game.LevelEditor.Services;

public class LevelFileService
{
    private readonly LevelSerializer _serializer = new LevelSerializer();

    public (DialogResult result, string? path) Save(EditorContext context)
    {
        var result = Dialog.FileSave("hdl", Paths.MapsFolder);

        if (!result.IsOk)
        {
            return (result, null);
        }

        Level level = _serializer.Serialize(context);

        string path = Level.SaveToFile(level, result.Path);

        return (result, path);
    }

    public bool Load(EditorContext context)
    {
        var result = Dialog.FileOpen("hdl", Paths.MapsFolder);

        if (!result.IsOk)
        {
            return false;
        }

        Level level = Level.LoadFromFile(result.Path);

        _serializer.Deserialize(context, level);

        context.LevelName = Path.GetFileNameWithoutExtension(result.Path);

        return true;
    }

    public void NewLevel(EditorContext context, string name, int sizeX, int sizeY)
    {
        //context.EntityList.Clear();
        context.World.SizeX = sizeX;
        context.World.SizeY = sizeY;
        context.World.Cells = new Cell[sizeX, sizeY];
        context.LevelName = name;
        context.PlayerStart = Vector2.Zero;
        context.StartRotation = 0f;

        Debug.Log("New level");
    }

    public class LevelSerializer
    {
        public void NewLevel(EditorContext context)
        {
            context.World.EntityList.Clear();
            context.World.Cells = new Cell[context.World.SizeX, context.World.SizeY];
            context.LevelName = "New Level";
            context.PlayerStart = Vector2.Zero;
            context.StartRotation = 0f;

            Debug.Log("New level");
        }

        public Level Serialize(EditorContext context)
        {
            Level level = Level.FromWorld(context.World);
            level.PlayerStart = context.PlayerStart;
            level.StartRotation = context.StartRotation;

            return level;
        }

        public void Deserialize(EditorContext context, Level level)
        {
            context.World.EntityList = level.EntityList;
            context.World.Cells = level.Cells;

            context.PlayerStart = level.PlayerStart;
            context.LevelName = level.Name;
        }
    }
}
