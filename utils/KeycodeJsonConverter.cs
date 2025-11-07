using SharpHook.Data;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Simple_Key_Remapper.utils;
public class KeyCodeJsonConverter : JsonConverter<KeyCode>
{
    public override KeyCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? keyCodeName = reader.GetString();

        // verificacion previa
        if(string.IsNullOrEmpty(keyCodeName))
            throw new JsonException("KeyCode name cannot be null or empty.");

        // conversion
        string enumName = keyCodeName.StartsWith("Vc", StringComparison.OrdinalIgnoreCase) ? keyCodeName : "Vc" + keyCodeName;

        if(Enum.TryParse<KeyCode>(enumName, out var result))
            return result;

        throw new JsonException($"No se pudo convertir '{keyCodeName}' a KeyCode.");
    }

    public override void Write(Utf8JsonWriter writer, KeyCode value, JsonSerializerOptions options)
    {
        string keyName = value.ToString().Replace("Vc", "", StringComparison.OrdinalIgnoreCase);
        writer.WriteStringValue(keyName);
    }
}

