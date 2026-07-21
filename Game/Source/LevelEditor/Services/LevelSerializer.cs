using NativeFileDialogSharp;
using System.Numerics;

namespace Game.LevelEditor.Services;

public class LevelFileService
{
    private readonly LevelSerializer _serializer = new LevelSerializer();

    public (DialogResult result, string? path) Save(Editor editor)
    {
        var result = Dialog.FileSave("hdl", Paths.MapsFolder);

        if (!result.IsOk)
        {
            return (result, null);
        }

        Level level = _serializer.Serialize(editor);

        string path = Level.SaveToFile(level, result.Path);

        return (result, path);
    }

    public bool Load(Editor editor)
    {
        var result = Dialog.FileOpen("hdl", Paths.MapsFolder);

        if (!result.IsOk)
        {
            return false;
        }

        Level level = Level.LoadFromFile(result.Path);

        _serializer.Deserialize(editor, level);

        editor.LevelName = Path.GetFileNameWithoutExtension(result.Path);

        return true;
    }

    public class LevelSerializer
    {
        public void NewLevel(Editor editor)
        {
            editor.EntityList.Clear();
            editor.Cells = new Cell[Editor.WORLD_WIDTH, Editor.WORLD_HEIGHT];
            editor.LevelName = "New Level";
            editor.PlayerStart = Vector2.Zero;
            editor.StartRotation = 0f;

            Debug.Log("New level");
        }

        public Level Serialize(Editor editor)
        {
            Level level = Level.FromWorld(editor);
            level.PlayerStart = editor.PlayerStart;
            level.StartRotation = editor.StartRotation;

            return level;
        }

        public void Deserialize(Editor editor, Level level)
        {
            editor.EntityList = level.EntityList;
            editor.Cells = level.Cells;

            editor.PlayerStart = level.PlayerStart;
            editor.LevelName = level.Name;
        }
    }
}
