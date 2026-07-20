using System.Numerics;
using System.Text.Json;

namespace Game;

public class Level
{
    public Vector2 PlayerStart { get; set; }
    public List<Entity> EntityList { get; set; }
    public Cell[,] Cells { get; protected set; }


    private static JsonSerializerOptions _options;


    static Level()
    {
        _options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true,
        };

        _options.Converters.Add(new TextureConverter());
    }

    public Level()
    {
        
    }

    public static string SaveToFile(Level level, string fileName)
    {
        string contents = JsonSerializer.Serialize(level, options: _options);
        string path = $@"C:\Users\The1Wolfcast\source\Games\Escape\Game\Assets\Maps\{fileName}";

        File.WriteAllText(path, contents);

        return path;
    }

    public static Level LoadFromFile(string path)
    {
        string contents = File.ReadAllText(path);

        Level level = JsonSerializer.Deserialize<Level>(contents, _options);

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
