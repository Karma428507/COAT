namespace COAT.Gamemode;

using COAT;
using UnityEngine;

/// <summary> The class involved for different gamemodes. </summary>
public abstract class Gamemode
{
    public virtual string Name { get; set; }

    public string Mod { get; private set; }

    public abstract void Initialize();

    public void Setup(string modName)
    {
        if (Mod != null && Mod != "")
        {
            Log.Error("Gamemode has already been set up");
            return;
        }

        Mod = modName;
    }

    public string GetID() => $"{Mod.ToUpper()}:{Name}";
}
