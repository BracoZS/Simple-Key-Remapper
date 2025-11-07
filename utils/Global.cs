using System;
using System.IO;

namespace Simple_Key_Remapper.utils;
internal class Global
{
    // nombre de app 
    public static readonly string AppName;

    // carpeta base del .exe
    public static readonly string AppFolder;

    // carpeta AppData de la app, según el SO
    public static readonly string AppDataFolder;

    // ruta entera del archivo de configuración
    public static readonly string ConfigFilePath;

    // direccion de github
    public const string GithubUrl = "https://github.com/bracozs/Simple";

    static Global()
    {
        AppName = "Simple Key Remapper";
        AppFolder = AppContext.BaseDirectory;
        AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
        ConfigFilePath = Path.Combine(AppDataFolder, "config.json");
    }

    public static string GetImgPath(string imgName) => Path.Combine(AppFolder, "assets", $"{imgName}.png");
}

