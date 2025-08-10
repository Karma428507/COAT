namespace COAT.IO;

using COAT.Gamemodes;
using Discord;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary> To manage saved mod data </summary>
public static class SaveManager
{
    private static Dictionary<string, object> lobbyGeneral = new Dictionary<string, object>()
    {
        {"name", $"{SteamClient.Name}'s Lobby"},
        {"cheats", false},
        {"mods", true},
    };

    static PrefsManager pm => PrefsManager.Instance;

    public static void Load()
    {
        if (false)
            PortOldSave();

        // there's prob a better way of doing this :P
    }

    private static void PortOldSave()
    {
        // work on when reworking how settings is organize
    }

    #region Lobby Data
    public static void SetupData(Lobby lobby, GamemodeTypes gamemode)
    {

    }

    private static void LobbySetData()
    {
        //if (pm.)
    }
    #endregion
}
