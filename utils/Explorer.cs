using System;
using System.IO;
using System.Text.Json;

namespace Simple_Key_Remapper.utils;
public static class Explorer
{
    public static T? LoadJsonFile<T>(string filepath, JsonSerializerOptions? options = null)
    {
        try
        {
            if(!File.Exists(filepath))
                return default;

            string json = File.ReadAllText(filepath);

            if(string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json, options);
        }
        catch(Exception ex)
        {
            // Debug.WriteLine($"Error al cargar el archivo JSON: {filepath}: {ex.Message}");
            return default;
        }
    }

    public static bool SaveJsonFile<T>(T model, string filePath, JsonSerializerOptions? options = null)
    {
        var defaultIndent = new JsonSerializerOptions { WriteIndented = true };

        try
        {
            string json = JsonSerializer.Serialize(model, options ?? defaultIndent);

            Directory.CreateDirectory(Global.AppDataFolder); // crea la carpeta si no existe

            File.WriteAllText(filePath, json);

            return true;
        }
        catch(Exception ex)
        {
            // Console.Error.WriteLine($"Error al serializar el archivo '{filePath}': {ex.Message}");
            return false;
        }
    }
}
