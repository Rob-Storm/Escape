using System.Numerics;
using System.Text.Json;

namespace Game;

public class Level
{
    public string Name { get; set; }
    public Vector2 PlayerStart { get; set; }
    public float StartRotation { get; set; }
    public List<Entity> EntityList { get; set; }

    public Cell[,] Cells { get; set; }


    private static JsonSerializerOptions _options = new JsonSerializerOptions();


    static Level()
    {
        _options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true,
        };

        _options.Converters.Add(new TextureConverter());
        _options.Converters.Add(new CellArrayConverter());
    }

    public Level()
    {
        EntityList = new List<Entity>();
        Cells = new Cell[World.WORLD_WIDTH, World.WORLD_HEIGHT];
    }

    public static string SaveToFile(Level level, string fileName)
    {
        if (!fileName.EndsWith(".hdl", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".hdl";
        }

        string contents = JsonSerializer.Serialize(level, options: _options);

        Debug.Log($"Saving level '{Path.GetFileName(fileName)}' to '{fileName}'");

        File.WriteAllText(fileName, contents);

        return fileName;
    }

    public static Level LoadFromFile(string path)
    {
        if(!Path.Exists(path))
        {
            throw new Exception("Could not find level");
        }

        string contents = File.ReadAllText(path);

        Level? level = JsonSerializer.Deserialize<Level>(contents, _options);

        if(level == null)
        {
            throw new Exception("Could not load level");
        }

        return level;
    }

    public static Level FromWorld(World world)
    {
        Level level = new Level();
        level.EntityList = world.EntityList.Where(e => e is not Player).ToList();
        level.Cells = world.Cells;

        return level;
    }
}
