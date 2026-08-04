using ImGuiNET;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class DeveloperConsole : EditorPanel
{
    private List<string> _consoleHistory;
    private bool _scrollToBottom = true;

    public DeveloperConsole(EditorContext context) : base(context)
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

        if (ImGui.Button("Clear History", new Vector2(120, 0))) { _consoleHistory.Clear(); }

        ImGui.SameLine();

        ImGui.Checkbox("Scroll To Bottom", ref _scrollToBottom);

        ImGui.BeginChild("Scroll", new Vector2(0, -5), ImGuiChildFlags.None);

        ImGui.Separator();

        foreach (string message in _consoleHistory)
        {
            ImGui.Text(message);
        }

        if (_scrollToBottom)
        {
            ImGui.SetScrollHereY(1);
        }

        ImGui.EndChild();

        ImGui.End();
    }
}
