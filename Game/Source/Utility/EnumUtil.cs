namespace Game.Utility;

public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();
}