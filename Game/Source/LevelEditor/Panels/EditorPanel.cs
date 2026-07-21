namespace Game.LevelEditor.Panels;

public abstract class EditorPanel
{
    protected EditorContext _context;

    protected EditorPanel(EditorContext context)
    {
        _context = context;
    }

    public abstract void Draw();
}
