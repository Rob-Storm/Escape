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