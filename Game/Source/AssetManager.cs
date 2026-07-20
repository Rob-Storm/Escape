using Raylib_cs;

namespace Game;

public static class AssetManager
{
    public static Dictionary<string, object> Assets { get; private set; }
    private static Dictionary<object, string> _assetPaths;
    private static Dictionary<Type, Delegate> _resourceLoaders;

    static AssetManager()
    {
        Assets = new Dictionary<string, object>();

        _assetPaths = new Dictionary<object, string>();
        _resourceLoaders = new Dictionary<Type, Delegate>
        {
            {  typeof(Texture2D), LoadTexture },
            {  typeof(Sound), LoadSound },
            {  typeof(Music), LoadMusic }
        };
    }

    public static void ScanRegistries()
    {
        string assetPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");

        string[] files = Directory.GetFiles(assetPath, "*.*", SearchOption.AllDirectories);

        foreach(string file in files)
        {
            string relativePath = Path.Combine("Assets", Path.GetRelativePath(assetPath, file));

            switch (Path.GetExtension(file).ToLowerInvariant())
            {
                case ".png":
                    Load<Texture2D>(relativePath);
                    break;
                case ".wav":
                    Load<Sound>(relativePath);
                    break;
                case ".ogg":
                    Load<Music>(relativePath);
                    break;
                default:
                    continue;
            }
        }
    }

    public static IEnumerable<KeyValuePair<string, T>> GetAssets<T>()
    {
        foreach (var asset in Assets)
        {
            if(asset.Value is T value)
            {
                yield return new KeyValuePair<string, T>(asset.Key, value);
            }
        }
    }

    public static string GetAssetType(object asset)
    {
        return asset switch
        {
            Texture2D => "Texture",
            Sound => "Sound",
            Music => "Music",
            _ => asset.GetType().Name
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

        Func<string, T> loader = (Func<string, T>)_resourceLoaders[typeof(T)];
        object loadedObject = loader(path);

        if (loadedObject != null)
        {
            Assets.Add(path, loadedObject);
            _assetPaths.Add(loadedObject, path);
        }
        else
        {
            Debug.Log("Could not load resource", LogLevel.Warning, LogChannel.Asset);
        }
        
        return (T)loadedObject;
    }

    private static Texture2D LoadTexture(string path)
    {
        return Raylib.LoadTexture(path);
    }

    private static Sound LoadSound(string path)
    {
        return Raylib.LoadSound(path);
    }

    private static Music LoadMusic(string path)
    {
        return Raylib.LoadMusicStream(path);
    }
}

