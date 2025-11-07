using System.Runtime.CompilerServices;
using Res = Simple_Key_Remapper.Properties.Resources;


namespace Simple_Key_Remapper.utils;
public class Lang
{
    #region Implementacion de singleton
    private static Lang _instance;  // instancia privada

    public static Lang Instance => _instance ??= new Lang(); // propiedad estatica para obtener la instancia

    private Lang() { /**/ } // ctor privado
    #endregion

    public string AppName => Get();
    public string Ok => Get();
    public string Exit => Get();
    public string Description => Get();
    public string SourceKey => Get();
    public string TargetKey => Get();
    public string OpenConfig => Get();
    public string RunAtStartup => Get();
    public string OpenWebsiteProject => Get();
    public string AddNewRemap => Get();
    public string KeyInUseWarning => Get();

    private string Get([CallerMemberName] string propName = null)
    {
        var value = Res.ResourceManager.GetString(propName);

        if(value != null)
            return value;

        return $"[not found]";      // en caso de no encontrar la cadena con el nombre de la property en .resx
    }
}
