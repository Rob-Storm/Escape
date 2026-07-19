using Raylib_cs;

namespace Game;

public static class AssetManager
{
    public static Dictionary<string, object> Assets { get; private set; }
    private static Dictionary<object, string> _assetPaths;
    private static Dictionary<Type, Delegate> _resouceLoaders;

    static AssetManager()
    {
        Assets = new Dictionary<string, object>();

        _assetPaths = new Dictionary<object, string>();
        _resouceLoaders = new Dictionary<Type, Delegate>
        {
            {  typeof(Texture2D), LoadTexture }
        };
    }

    public static string GetPath<T>(object asset)
    {
        if (_assetPaths.TryGetValue(asset, out string path))
        {
            return path;
        }

        return "BAD PATH!";
    }

    public static T Load<T>(string path)
    {
        if (Assets.TryGetValue(path, out object cachedObject))
        {
            return (T)cachedObject;
        }

        Func<string, T> loader = (Func<string, T>)_resouceLoaders[typeof(T)];
        object loadedObject = loader(path);

        if (loadedObject != null)
        {
            Assets.Add(path, loadedObject);
            _assetPaths.Add(loadedObject, path);
        }
        else
        {
            Console.WriteLine("Could not load resource");
        }
        
        return (T)loadedObject;
    }

    private static Texture2D LoadTexture(string path)
    {
        return Raylib.LoadTexture(path);
    }
}

