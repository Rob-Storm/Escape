namespace Game;
public static class Paths
{
    public static readonly string ApplicationExecutable;
    public static readonly string ApplicationRoot;
    public static readonly string AssetsRoot;
    public static readonly string TexturesFolder;
    public static readonly string SoundsFolder;
    public static readonly string MusicFolder;
    public static readonly string MapsFolder;

    static Paths()
    {
        ApplicationExecutable = Environment.ProcessPath;

        ApplicationRoot = Path.GetDirectoryName(ApplicationExecutable);
        AssetsRoot = Path.Combine(ApplicationRoot, "Assets");

        TexturesFolder = Path.Combine(AssetsRoot, "Textures");
        SoundsFolder = Path.Combine(AssetsRoot, "Sounds");
        MusicFolder = Path.Combine(AssetsRoot, "Music");
        MapsFolder = Path.Combine(AssetsRoot, "Maps");

    }
}
