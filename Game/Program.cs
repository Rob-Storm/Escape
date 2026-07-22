using Game;

class Program
{
    public static void Main(string[] args)
    {
        bool editorMode = args.Contains("--editor");

        string? levelPath = null;

        int levelIndex = Array.IndexOf(args, "--level");
        if (levelIndex >= 0 && levelIndex + 1 < args.Length)
        {
            levelPath = args[levelIndex + 1];
        }

        Engine engine = new Engine();
        engine.Init(editorMode, levelPath);
    }
}