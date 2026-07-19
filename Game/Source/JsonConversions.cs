using Raylib_cs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Game;

public class TextureConverter : JsonConverter<Texture2D>
{
    public override Texture2D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Texture2D texture = new Texture2D();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected object");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected property name");
            }

            string propertyName = reader.GetString();

            reader.Read();

            if(propertyName == "Path")
            {
                texture = AssetManager.Load<Texture2D>(reader.GetString());
            }
            else
            {
                reader.Skip();
            }
        }

        return texture;
    }

    public override void Write(Utf8JsonWriter writer, Texture2D value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("Path", AssetManager.GetPath<Texture2D>(value));

        writer.WriteEndObject();
    }
}