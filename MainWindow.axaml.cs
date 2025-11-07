using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using SharpHook.Data;
using Simple_Key_Remapper.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Simple_Key_Remapper;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public Lang Lang { get; } = Lang.Instance;

    private ObservableCollection<RemapEntry> _keyRemaps = new()
    {
        //new RemapEntry { Origin = KeyCode.VcA, Target = KeyCode.VcB },
        //new RemapEntry { Origin = KeyCode.VcC, Target = KeyCode.VcD },
    };
    private bool _runAtStartup;

    public ObservableCollection<RemapEntry> KeyRemaps
    {
        get => _keyRemaps;
        set
        {
            if(value != _keyRemaps)
            {
                _keyRemaps = value;
                OnPropertyChanged();
            }

        }
    }

    public bool RunAtStartup
    {
        get => _runAtStartup;
        set
        {
            _runAtStartup = value;
        }
    }

    public List<KeyCode> AllKeys { get; set; }
        = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToList();

    // comandos
    public ICommand RemoveRemapCommand { get; }


    public MainWindow()
    {
        InitializeComponent();

        DataContext = this;     // la propia ventana actúa como ViewModel
    }

    public MainWindow(Dictionary<KeyCode, KeyCode> remapsList, bool runAtStatrup) : this()
    {
        KeyRemaps = new ObservableCollection<RemapEntry>(
            remapsList.Select(kvp => new RemapEntry { Origin = kvp.Key, Target = kvp.Value })   //diccionario a OC de RemapEntry
        );
        _runAtStartup = runAtStatrup;

        RemoveRemapCommand = new RelayCommand<RemapEntry>(RemoveRemap);
    }

    private void ComboBoxOrigen_SelectionChanging(object? sender, SelectionChangedEventArgs e)
    {
        if(sender is ComboBox combo && combo.SelectedItem is KeyCode selectedKey)
        {
            // busca repetidos
            var iguales = _keyRemaps.Where(entry => entry.Origin == selectedKey);

            // si hay mas de uno muestra tun tooltip de error
            if(iguales.Count() > 1)
            {
                var keyName = selectedKey.ToString()!.Replace("Vc", "");
                ToolTip.SetTip(combo, $"{{ {keyName} }} {Lang.KeyInUseWarning}");     // tooltip aplica el estilo desde xaml

                // Lo muestra manualmente
                ToolTip.SetPlacement(combo, PlacementMode.BottomEdgeAlignedLeft);
                ToolTip.SetIsOpen(combo, true);

                combo.SelectedItem = null;
                combo.BorderBrush = new SolidColorBrush(Color.Parse("#E81A26"));
                combo.Background = new SolidColorBrush(Color.Parse("#fef2f3"));

                return;
            }

            combo.ClearValue(ComboBox.BorderBrushProperty);
            combo.ClearValue(ComboBox.BackgroundProperty);
        }
    }

    private void ok_Click(object? sender, RoutedEventArgs e)
    {
        var newRemapsDict = KeyRemaps
            .Where(entry => entry.Origin.HasValue)   // filtra los nulos
            .ToDictionary(entry => entry.Origin!.Value, entry => entry.Target);

        var x = Application.Current as App;
        x!.SaveConfig(newRemapsDict, _runAtStartup);

        this.Close();
    }
    private void addRemap_Click(object? sender, RoutedEventArgs e)
    {
        KeyRemaps.Add(new RemapEntry { Origin = null, Target = KeyCode.VcUndefined });

        Dispatcher.UIThread.Post(MyScroll.ScrollToEnd, DispatcherPriority.Background);
    }

    private void RemoveRemap(RemapEntry entry)
    {
        if(entry is not null)
            KeyRemaps.Remove(entry);
    }

    // funciones de la ventana
    private void titleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // arrastra la ventana si se presiona el botón izquierdo del ratón
        if(e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            this.BeginMoveDrag(e);
    }

    private void close(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }


    #region Implementación de INotifyPropertyChanged 
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion
}

public class RemapEntry
{
    public KeyCode? Origin { get; set; }
    public KeyCode Target { get; set; }
}