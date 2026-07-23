using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game;

public static class AssetManager
{
    public static Dictionary<string, object> Assets { get; private set; }
    private static Dictionary<object, string> _assetPaths;
    private static Dictionary<Type, AssetTypeInfo> _types;

    private static Dictionary<Type, Delegate> _resourceLoaders;
    private static Dictionary<Type, string> _fallbackPaths;

    static AssetManager()
    {
        Assets = new Dictionary<string, object>();
        _assetPaths = new Dictionary<object, string>();
        _types = new Dictionary<Type, AssetTypeInfo>();

        string soundIcon = @"Assets\Editor\SoundIcon.png";
        string musicIcon = @"Assets\Editor\MusicIcon.png";

        Register<Texture2D>
            (
                Raylib.LoadTexture, 
                @"Assets\Textures\Default.png",
                "Texture",
                new Vector4(1f, 0f, 0f, 1f),
                "asset",
                texture => { return rlImGui.ImageButtonSize("##assetButton", (Texture2D)texture, new Vector2(96)); },
                texture => 
                {
                    Texture2D textureObject = (Texture2D)texture;
                    rlImGui.ImageSize(textureObject, new Vector2(textureObject.Width, textureObject.Height)); 
                },
                ".png"
            );
        
        Register<Sound>
            (
                Raylib.LoadSound,
                @"Assets\Textures\Default.png",
                "Sound",
                new Vector4(0f, 1f, 0f, 1f),
                "asset",
                sound => { return rlImGui.ImageButtonSize("##assetButton", Load<Texture2D>(soundIcon), new Vector2(96)); },
                sound => 
                {
                    if(ImGui.Button("Play Sound"))
                    {
                        Raylib.PlaySound((Sound)sound);
                    }
                },
                ".wav"
            );

        Register<Music>
            (
                Raylib.LoadMusicStream,
                @"Assets\Textures\Default.png",
                "Music",
                new Vector4(0f, 1f, 1f, 1f),
                "asset",
                music => { return rlImGui.ImageButtonSize("##assetButton", Load<Texture2D>(musicIcon), new Vector2(96)); },
                music => 
                {
                    
                },
                ".ogg"
            );
    }

    public static bool IsRegisteredAssetType(Type type) => _types.ContainsKey(type);

    public static void Register<T>
        (
            Func<string, T> loader, 
            string fallback, 
            string displayName, 
            Vector4 color, 
            string dragPayload, 
            Func<object, bool> drawPreview, 
            Action<object> drawInspector, 
            params string[] extensions
        )
    {
        _types[typeof(T)] = new AssetTypeInfo
        {
            Type = typeof(T),
            Loader = path => loader(path)!,
            FallbackPath = fallback,
            DisplayName = displayName,
            Color = color,
            DragDropPayload = dragPayload,
            DrawPreview = drawPreview,
            DrawInspector = drawInspector,
            Extensions = extensions
        };
    }

    public static void ScanRegistries()
    {
        foreach (string file in Directory.GetFiles(Paths.AssetsRoot, "*.*", SearchOption.AllDirectories))
        {
            string extension = Path.GetExtension(file);

            AssetTypeInfo? type = _types.Values.FirstOrDefault(t => t.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase));

            if(type == null)
            {
                continue;
            }

            string relative = Path.Combine("Assets", Path.GetRelativePath(Paths.AssetsRoot, file));

            Load(relative, type.Type);
        }
    }

    public static IEnumerable<KeyValuePair<string, object>> GetAssets() => Assets;

    public static IEnumerable<KeyValuePair<string, T>> GetAssets<T>()
    {
        foreach (var asset in Assets)
        {
            if (asset.Value is T value)
            {
                yield return new KeyValuePair<string, T>(asset.Key, value);
            }
        }
    }

    public static AssetTypeInfo GetAssetTypeInfo(Type type)
    {
        return _types[type];
    }
    public static AssetTypeInfo GetAssetTypeInfo<T>()
    {
        return _types[typeof(T)];
    }
    public static AssetTypeInfo GetAssetTypeInfo(object asset)
    {
        return _types[asset.GetType()];
    }

    public static AssetTypeInfo GetAssetTypeInfo(string path)
    {
        if(Assets.TryGetValue(path, out object value))
        {
            return GetAssetTypeInfo(value);
        }

        return null;
    }

    public static string GetPath<T>(object asset) =>_assetPaths.TryGetValue(asset, out var path) ? path : string.Empty;
    public static string GetPath(object asset) =>_assetPaths.TryGetValue(asset, out var path) ? path : string.Empty;
    public static object Load(string path, Type type)
    {
        AssetTypeInfo info = _types[type];

        if (Assets.TryGetValue(path, out object? cached))
        {
            return cached;
        }

        if(!File.Exists(path))
        {
            path = info.FallbackPath;
        }

        object asset = info.Loader(path);

        Assets[path] = asset;
        _assetPaths[asset] = path;

        return asset;
    }

    public static T Load<T>(string path)
    {
        return (T)Load(path, typeof(T));
    }
}

