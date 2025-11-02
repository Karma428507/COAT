namespace COAT.IO;

using COAT.Gamemodes;
using COAT.UI.Menus;
using Discord;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary> To manage saved mod data </summary>
public static class SaveManager
{
    public struct ServerOptions
    {
        // General
        public string Name;
        public bool Cheats;
        public bool Mods;
        public short MaxPlayers;
        public byte ServerType;
        public GamemodeTypes Gamemode;
    }

    private static Dictionary<string, object> lobbyGeneral = new Dictionary<string, object>()
    {
        {"name", $"{SteamClient.Name}'s Lobby"},
        {"cheats", false},
        {"mods", true},
        {"maxplayers", 8},
        {"servertype", 2},
        {"gamemode", 0},
    };

    static PrefsManager pm => PrefsManager.Instance;

    public static void Load()
    {
        LoadLobby();

        if (false)
            PortOldSave();

        // there's prob a better way of doing this :P
    }

    private static void PortOldSave()
    {
        // work on when reworking how settings is organize
    }

    #region Lobby Data
    public static void SaveLobby()
    {
        Log.Debug("Saved :3");

        pm.SetString("name", GamemodeList.Options.Name);
        pm.SetBool("cheats", GamemodeList.Options.Cheats);
        pm.SetBool("mods", GamemodeList.Options.Mods);
        pm.SetInt("maxplayers", GamemodeList.Options.MaxPlayers);
        pm.SetInt("servertype", GamemodeList.Options.ServerType);
        //pm.SetInt("gamemode", (int)GamemodeList.Options.Gamemode);
    }

    public static void LoadLobby()
    {
        GamemodeList.Options.Name = pm.GetString("name", (string)lobbyGeneral["name"]);
        GamemodeList.Options.Cheats = pm.GetBool("cheats", (bool)lobbyGeneral["cheats"]);
        GamemodeList.Options.Mods = pm.GetBool("mods", (bool)lobbyGeneral["mods"]);
        GamemodeList.Options.MaxPlayers = (short)pm.GetInt("maxplayers", (int)lobbyGeneral["maxplayers"]);
        GamemodeList.Options.ServerType = (byte)pm.GetInt("servertype", (int)lobbyGeneral["servertype"]);
        //GamemodeList.Options.Gamemode = (GamemodeTypes)pm.GetInt("gamemode", (int)lobbyGeneral["gamemode"]);
    }

    private static void LobbySetData()
    {
        //if (pm.)
    }
    #endregion
}
