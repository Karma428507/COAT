namespace COAT.Gamemode;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using COAT;
using UnityEngine;

/// <summary> To manage internal and EXTERNAL gamemodes for the mod. (THIS IS WIP AND WON'T BE IMPLIMENTED UNTIL AFTER RELEASE) </summary>
public class GamemodeManager
{
    // NOTE: Replace other syncs like enemies and world with registers for third parties to handle
    /// <summary> The internal and external gamemodes stored here </summary>
    public static List<Gamemode> RegisteredGamemodes = new List<Gamemode>();
    
    public static void Load()
    {
        Register(new Normal());
        Register(new OilPaint());
    }

    internal static void Register(Gamemode gamemode) => Register("COAT", gamemode);

    public static void Register(string modName, Gamemode gamemode)
    {
        gamemode.Setup(modName);
        gamemode.Initialize();
        RegisteredGamemodes.Add(gamemode);

        Log.Debug($"Gamemode: [{gamemode.GetID()}]");
    }
}
