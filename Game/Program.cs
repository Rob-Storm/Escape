using Game;

class Program
{
    public static void Main(string[] args)
    {
        bool editorMode = args.Contains("--editor");

        Engine engine = new Engine();
        engine.Init(editorMode);
    }
}