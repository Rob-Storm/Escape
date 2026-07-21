namespace Game.LevelEditor.Panels;

public abstract class EditorPanel
{
    private readonly Editor _editor;

    protected EditorPanel(Editor editor)
    {
        _editor = editor;
    }

    public abstract void Draw();
}
