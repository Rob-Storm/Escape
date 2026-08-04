using Game.Objects;
using Raylib_cs;
using System.Numerics;
using System.Text.Json;

namespace Game;

public class Level
{
    public string Name { get; set; }
    public Vector2 PlayerStart { get; set; }
    public Texture2D SkyboxTexure { get; set; }
    public Music BackgroundMusic { get; set; }
    public float StartRotation { get; set; }
    public List<Entity> EntityList { get; set; }
    public int SizeX { get; set; }
    public int SizeY { get; set; }
    public Cell[,] Cells { get; set; }


    private static JsonSerializerOptions _options = new JsonSerializerOptions();


    static Level()
    {
        _options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true,
        };


        _options.Converters.Add(new CellArrayConverter());
        _options.Converters.Add(new AssetConverter<Texture2D>());
        _options.Converters.Add(new AssetConverter<Sound>());
        _options.Converters.Add(new AssetConverter<Music>());
    }

    public Level()
    {
        EntityList = new List<Entity>();
        Cells = new Cell[SizeX, SizeY];
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
        if (!Path.Exists(path))
        {
            throw new Exception($"Could not find level at path: {path}");
        }

        string contents = File.ReadAllText(path);

        Level? level = JsonSerializer.Deserialize<Level>(contents, _options);

        if (level == null)
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
