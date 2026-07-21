using ImGuiNET;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class DeveloperConsole : EditorPanel
{
    private List<string> _consoleHistory;

    public DeveloperConsole(Editor editor) : base(editor)
    {
        _consoleHistory = new List<string>();

        Debug.OnLogCommitted += (message, level, channel) =>
        {
            _consoleHistory.Add(message);
        };

    }

    public override void Draw()
    {
        ImGui.Begin("Developer Console");

        ImGui.BeginChild("Scroll", new Vector2(0, -25), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar);

        foreach (string message in _consoleHistory)
        {
            ImGui.Text(message);
        }

        ImGui.EndChild();

        string test = string.Empty;

        float buttonWidth = 120.0f;
        float spacing = ImGui.GetStyle().ItemSpacing.X;

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttonWidth - spacing);

        ImGui.InputTextWithHint("##ConsoleInput", "Enter Command", ref test, 256,
            ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.EscapeClearsAll);

        ImGui.SameLine();

        if (ImGui.Button("Clear History", new Vector2(buttonWidth, 0))) { _consoleHistory.Clear(); }

        ImGui.End();
    }
}
