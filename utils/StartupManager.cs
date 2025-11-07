using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoLaunch;

namespace Simple_Key_Remapper.utils;
public static class StartupManager
{
    private static readonly AutoLauncher? autoLauncher;

    static StartupManager()
    {
        try
        {
            var appName = Global.AppName;
            var appPath = Process.GetCurrentProcess().MainModule?.FileName;

            autoLauncher = new AutoLaunchBuilder()
                .SetAppName(appName)    // here is registry key name (.../Run/appName)
                .SetAppPath(appPath!)   // here is full path to app .exe 
                .SetWorkScope(WorkScope.CurrentUser)    // windows only
                .SetWindowsEngine(WindowsEngine.Registry)
                .Build();

            // Debug.WriteLine("StartupManager inicializado correctamente.");
        }
        catch(Exception ex)
        {
            throw new Exception($"StartupManager Error : {ex.Message}");
        }
    }
    public static async Task SwitchRunAtStartup()
    {
        var startupStatus = await autoLauncher!.GetStatusAsync();

        await SetStartup(!startupStatus);
    }

    public static async Task SetStartup(bool enable)
    {
        if(enable)
            await autoLauncher!.EnableAsync();
        else
            await autoLauncher!.DisableAsync();
    }

    public static async Task<bool> IsEnabledAsync() => await autoLauncher!.GetStatusAsync();
}

