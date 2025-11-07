using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpHook.Data;

namespace Simple_Key_Remapper.Models;
public class AppConfig
{
    public bool RunAtStartup { get; set; } = true;
    public Dictionary<KeyCode, KeyCode> KeyRemaps { get; set; } = new();
}
