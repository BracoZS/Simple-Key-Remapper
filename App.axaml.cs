using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using SharpHook;
using SharpHook.Data;
using Simple_Key_Remapper.Models;
using Simple_Key_Remapper.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Simple_Key_Remapper;
public partial class App : Application
{
    #region Heredados
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = this; //code mine
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnExplicitShutdown;

            loadApp();
        }

        base.OnFrameworkInitializationCompleted();
    }
    #endregion

    private Lang lang = Lang.Instance;
    private SimpleGlobalHook hook;
    private EventSimulator sim;
    private bool _runAtStartup = true;
    private Dictionary<KeyCode, KeyCode> keyRemaps = new();
    private MainWindow? configWindow;

    // private Dictionary<char, KeyCode> charRemaps = new();

    public bool RunAtStartup
    {
        get => _runAtStartup;
        set => _runAtStartup = value;
    }

    // funciones
    private void loadApp()
    {
        loadConfig();
        loadHook();
        loadTrayIcon();
    }

    private void loadConfig()
    {
        var config = loadConfigFromFile();

        _runAtStartup = config.RunAtStartup;
        keyRemaps = config.KeyRemaps;
    }

    private void loadHook()
    {
        hook = new(GlobalHookType.Keyboard);
        hook.KeyPressed += Hook_KeyPressed;
        hook.KeyReleased += Hook_KeyReleased;

        //hook.KeyTyped += Hook_KeyTyped;   // prox?

        sim = new EventSimulator();

        _ = Task.Run(async () =>
        {
            try
            {
                await hook.RunAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        });
    }

    private void loadTrayIcon()
    {
        TrayIcon trayIcon = new()
        {
            Icon = new WindowIcon(Global.GetImgPath("appicon_s")),
            ToolTipText = Global.AppName
        };

        // items del menú
        NativeMenuItem openConfiguration = new(lang.OpenConfig);
        openConfiguration.Icon = new Bitmap(Global.GetImgPath("settings"));
        openConfiguration.Click += openConfigurationItem_Click;

        NativeMenuItem switchRunAtStartup = new(lang.RunAtStartup);
        switchRunAtStartup.Icon = _runAtStartup
            ? new Bitmap(Global.GetImgPath("check"))
            : null;
        switchRunAtStartup.Click += switchRunAtStartup_Click;

        NativeMenuItem openWebsite = new(lang.OpenWebsiteProject);
        openWebsite.Icon = new Bitmap(Global.GetImgPath("github"));
        openWebsite.Click += goWebsite_Click;

        // --- separador ---

        NativeMenuItem exit = new(lang.Exit);
        exit.Icon = new Bitmap(Global.GetImgPath("exit"));
        exit.Click += exitItem_Click;

        var trayMenu = new NativeMenu
        {
            Items =
            {
                openConfiguration,
                switchRunAtStartup,
                openWebsite,
                new NativeMenuItemSeparator(),
                exit
            }
        };

        trayIcon.Menu = trayMenu;
        trayIcon.IsVisible = true;
    }

    private AppConfig loadConfigFromFile()
    {
        var config = Explorer.LoadJsonFile<AppConfig>(Global.ConfigFilePath);

        if(config is null)
        {
            config = new AppConfig();
            SaveConfig(config.KeyRemaps, config.RunAtStartup);
        }

        return config;
    }

    private void Hook_KeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        if(!e.IsEventSimulated && keyRemaps.TryGetValue(e.Data.KeyCode, out var cKey))
        {
            e.SuppressEvent = true;
            sim.SimulateKeyPress(cKey);
        }
    }

    private void Hook_KeyReleased(object? sender, KeyboardHookEventArgs e)
    {
        if(!e.IsEventSimulated && keyRemaps.TryGetValue(e.Data.KeyCode, out var cKey))
        {
            e.SuppressEvent = true;
            sim.SimulateKeyRelease(cKey);
        }
    }

    //private void Hook_KeyTyped(object? sender, KeyboardHookEventArgs e)
    //{
    //    if(!e.IsEventSimulated && charRemaps.TryGetValue(e.Data.KeyChar, out cKey))
    //    {
    //        e.SuppressEvent = true;
    //    }
    //}

    #region Métodos de los items del menú
    private void openConfigurationItem_Click(object? o, EventArgs e) => openConfig();

    private async void switchRunAtStartup_Click(object? o, EventArgs e)
    {
        RunAtStartup = !RunAtStartup;

        var openItem = o as NativeMenuItem;

        openItem!.Icon = RunAtStartup
            ? new Bitmap(Global.GetImgPath("check"))
            : null;

        await StartupManager.SwitchRunAtStartup();

        SaveConfig(keyRemaps, RunAtStartup);
    }

    private void goWebsite_Click(object? o, EventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = Global.GithubUrl,
            UseShellExecute = true
        });
    }

    private void exitItem_Click(object? o, EventArgs e) => Salir();
    #endregion

    private void openConfig()
    {
        if(configWindow is null || !configWindow.IsLoaded)
        {
            configWindow = new MainWindow(keyRemaps, _runAtStartup);
            configWindow.Show();
            return;
        }

        configWindow.Topmost = true;
        configWindow.Topmost = false;
        // configWindow.Focus(); // opcional
    }

    public void SaveConfig(Dictionary<KeyCode, KeyCode> newKeyRemaps, bool runAtStartup)
    {
        keyRemaps = newKeyRemaps;
        _runAtStartup = runAtStartup;

        Task.Run(() =>
        {
            _ = StartupManager.SetStartup(runAtStartup);

            Explorer.SaveJsonFile(
            new AppConfig
            {
                KeyRemaps = newKeyRemaps,
                RunAtStartup = runAtStartup
            },
            Global.ConfigFilePath
            //new JsonSerializerOptions
            //{
            //    WriteIndented = true,
            //    Converters = { new KeyCodeJsonConverter() }     // next
            //}
            );
        });
    }

    private void Salir()
    {
        hook.Dispose();
        Environment.Exit(0);
    }
}

