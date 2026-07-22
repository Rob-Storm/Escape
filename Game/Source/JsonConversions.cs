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

            if (propertyName == "Path")
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

public class CellArrayConverter : JsonConverter<Cell[,]>
{
    public override Cell[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);

        int rowLength = jsonDoc.RootElement.GetArrayLength();
        int columnLength = jsonDoc.RootElement.EnumerateArray().First().GetArrayLength();

        Cell[,] cells = new Cell[rowLength, columnLength];

        int row = 0;
        foreach (var cellArray in jsonDoc.RootElement.EnumerateArray())
        {
            int column = 0;
            foreach (var cellElement in cellArray.EnumerateArray())
            {
                cells[row, column] = cellElement.Deserialize<Cell>(options);
                column++;
            }
            row++;
        }

        return cells;
    }

    public override void Write(Utf8JsonWriter writer, Cell[,] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        for (int x = 0; x < value.GetLength(0); x++)
        {
            writer.WriteStartArray();
            for (int y = 0; y < value.GetLength(1); y++)
            {
                JsonSerializer.Serialize(writer, value[x, y], options);
            }
            writer.WriteEndArray();
        }

        writer.WriteEndArray();
    }
}