namespace Game.LevelEditor;


/// <summary>
/// Hides the field/property in the editor
/// </summary>

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class HidePropertyAttribute : Attribute
{

}

/// <summary>
/// Does not add this entity type to the "Spawn entity" menu
/// </summary>
/// 
[AttributeUsage(AttributeTargets.Class)]
public class HideFromSpawnMenuAttribute : Attribute
{

}

/// <summary>
/// Displays a tooltip in the editor when hovering over the property or class
/// </summary>
public class ToolTipAttribute : Attribute
{
    public string Tip { get; private set; }
    public ToolTipAttribute(string tip)
    {
        Tip = tip;
    }
}