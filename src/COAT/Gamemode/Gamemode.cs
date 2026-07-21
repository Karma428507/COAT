namespace COAT.Gamemode;

using COAT;
using COAT.Utils;

using System.Collections.Generic;

/// <summary> The class involved for different gamemodes. </summary>
public abstract class Gamemode
{
    public virtual string Name { get; set; }

    public string Mod { get; private set; }

    private Dictionary<string, object> Settings { get; set; }

    public abstract void Initialize();

    public void Setup(string modName)
    {
        if (Mod != null && Mod != "")
        {
            Log.Error("Gamemode has already been set up");
            return;
        }

        Mod = modName;
        Settings = new Dictionary<string, object>();
    }

    protected void AddSetting(string name, object obj)
    {
        if (Mapping.MainMenu)
            Settings.Add(name, obj);
    }

    public Dictionary<string, object> GetSettingsCopied()
    {
        Dictionary<string, object> copy = new Dictionary<string, object>();

        foreach (KeyValuePair<string, object> pair in Settings)
            copy[pair.Key] = pair.Value;

        return copy;
    }

    public string GetID() => $"{Mod.ToUpper()}:{Name}";
}
